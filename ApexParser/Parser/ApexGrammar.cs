﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexParser.MetaClass;
using Sprache;

namespace ApexParser.Parser
{
    public class ApexGrammar
    {
        // examples: a, Apex, code123
        protected internal virtual Parser<string> Identifier =>
        (
            from identifier in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit.Or(Parse.Char('_')))
            where !ApexKeywords.ReservedWords.Contains(identifier)
            select identifier
        )
        .Token().Named("Identifier");

        // examples: System.debug
        protected internal virtual Parser<IEnumerable<string>> QualifiedIdentifier =>
            Identifier.DelimitedBy(Parse.Char('.').Token())
                .Named("QualifiedIdentifier");

        // examples: /* default settings are OK */ //
        protected internal virtual CommentParser CommentParser { get; } = new CommentParser();

        // example: @isTest
        protected internal virtual Parser<AnnotationSyntax> Annotation =>
            from at in Parse.Char('@').Token()
            from identifier in Identifier
            from parameters in GenericExpressionInBraces.Optional()
            select new AnnotationSyntax
            {
                Identifier = identifier,
                Parameters = parameters.GetOrDefault(),
            };

        // examples: int, void
        protected internal virtual Parser<TypeSyntax> PrimitiveType =>
            Parse.IgnoreCase(ApexKeywords.Boolean).Or(
            Parse.IgnoreCase(ApexKeywords.Byte)).Or(
            Parse.IgnoreCase(ApexKeywords.Char)).Or(
            Parse.IgnoreCase(ApexKeywords.Double)).Or(
            Parse.IgnoreCase(ApexKeywords.Float)).Or(
            Parse.IgnoreCase(ApexKeywords.Int)).Or(
            Parse.IgnoreCase(ApexKeywords.Long)).Or(
            Parse.IgnoreCase(ApexKeywords.Short)).Or(
            Parse.IgnoreCase(ApexKeywords.Void))
                .Text().Then(n => Parse.Not(Parse.LetterOrDigit).Return(n.ToLower()))
                .Token().Select(n => new TypeSyntax(n))
                .Named("PrimitiveType");

        // examples: int, String, System.Collections.Hashtable
        protected internal virtual Parser<TypeSyntax> NonGenericType =>
            PrimitiveType.Or(QualifiedIdentifier.Select(qi => new TypeSyntax(qi)));

        // examples: string, int, char
        protected internal virtual Parser<IEnumerable<TypeSyntax>> TypeParameters =>
            from open in Parse.Char('<').Token()
            from types in TypeReference.DelimitedBy(Parse.Char(',').Token())
            from close in Parse.Char('>').Token()
            select types;

        // example: string, List<string>, Map<string, List<boolean>>
        protected internal virtual Parser<TypeSyntax> TypeReference =>
            from type in NonGenericType
            from parameters in TypeParameters.Optional()
            from arraySpecifier in Parse.Char('[').Token().Then(_ => Parse.Char(']').Token()).Optional()
            select new TypeSyntax(type)
            {
                TypeParameters = parameters.GetOrElse(Enumerable.Empty<TypeSyntax>()).ToList(),
                IsArray = arraySpecifier.IsDefined,
            };

        // example: string name
        protected internal virtual Parser<ParameterSyntax> ParameterDeclaration =>
            from type in TypeReference
            from name in Identifier
            select new ParameterSyntax(type, name);

        // example: int a, Boolean flag
        protected internal virtual Parser<IEnumerable<ParameterSyntax>> ParameterDeclarations =>
            ParameterDeclaration.DelimitedBy(Parse.Char(',').Token());

        // example: (string a, char delimiter)
        protected internal virtual Parser<List<ParameterSyntax>> MethodParameters =>
            from openBrace in Parse.Char('(').Token()
            from param in ParameterDeclarations.Optional()
            from closeBrace in Parse.Char(')').Token()
            select param.GetOrElse(Enumerable.Empty<ParameterSyntax>()).ToList();

        // examples: public, private, with sharing
        protected internal virtual Parser<string> Modifier =>
            Parse.IgnoreCase(ApexKeywords.Public).Or(
            Parse.IgnoreCase(ApexKeywords.Protected)).Or(
            Parse.IgnoreCase(ApexKeywords.Private)).Or(
            Parse.IgnoreCase(ApexKeywords.Static)).Or(
            Parse.IgnoreCase(ApexKeywords.Abstract)).Or(
            Parse.IgnoreCase(ApexKeywords.Final)).Or(
            Parse.IgnoreCase(ApexKeywords.Global)).Or(
            Parse.IgnoreCase(ApexKeywords.WebService)).Or(
            Parse.IgnoreCase(ApexKeywords.Override)).Or(
            Parse.IgnoreCase(ApexKeywords.Virtual)).Or(
            Parse.IgnoreCase(ApexKeywords.TestMethod)).Or(
            Parse.IgnoreCase(ApexKeywords.With).Token().Then(_ => Parse.IgnoreCase(ApexKeywords.Sharing)).Return($"{ApexKeywords.With} {ApexKeywords.Sharing}")).Or(
            Parse.IgnoreCase(ApexKeywords.Without).Token().Then(_ => Parse.IgnoreCase(ApexKeywords.Sharing)).Return($"{ApexKeywords.Without} {ApexKeywords.Sharing}")).Or(
            Parse.IgnoreCase("transient"))
                .Text().Token().Select(t => t.ToLower()).Named("Modifier");

        // examples:
        // @isTest void Test() {}
        // public static void Hello() {}
        protected internal virtual Parser<MethodDeclarationSyntax> MethodDeclaration =>
            from heading in MemberDeclarationHeading
            from typeAndName in TypeAndName
            from methodBody in MethodParametersAndBody
            select new MethodDeclarationSyntax(heading)
            {
                Identifier = typeAndName.Identifier ?? typeAndName.Type.Identifier,
                ReturnType = typeAndName.Type,
                Parameters = methodBody.Parameters,
                Body = methodBody.Body,
            };

        // examples: string Name, void Test
        protected internal virtual Parser<ParameterSyntax> TypeAndName =>
            from type in TypeReference
            from name in Identifier.Optional()
            select new ParameterSyntax(type, name.GetOrDefault());

        // examples:
        // void Test() {}
        // string Hello(string name) {}
        protected internal virtual Parser<MethodDeclarationSyntax> MethodParametersAndBody =>
            from parameters in MethodParameters
            from methodBody in Block.Token()
            select new MethodDeclarationSyntax
            {
                Parameters = parameters,
                Body = methodBody,
            };

        // example: @required public String name { get; set; }
        protected internal virtual Parser<PropertyDeclarationSyntax> PropertyDeclaration =>
            from heading in MemberDeclarationHeading
            from typeAndName in TypeAndName
            from accessors in PropertyAccessors
            select new PropertyDeclarationSyntax(heading)
            {
                Type = typeAndName.Type,
                Identifier = typeAndName.Identifier,
                GetterStatement = accessors.GetterStatement,
                SetterStatement = accessors.SetterStatement,
            };

        // example: { get; set; }
        protected internal virtual Parser<PropertyDeclarationSyntax> PropertyAccessors =>
            from openBrace in Parse.Char('{').Token()
            from accessors in PropertyAccessor.Many()
            from closeBrace in Parse.Char('}').Token()
            select new PropertyDeclarationSyntax(accessors);

        // examples: get; set; get { ... }
        protected internal virtual Parser<Tuple<string, StatementSyntax>> PropertyAccessor =>
            from getOrSet in Parse.IgnoreCase("get").Or(Parse.IgnoreCase("set")).Token().Text()
            from block in Parse.Char(';').Token().Return(new StatementSyntax()).Or(Block)
            select Tuple.Create(getOrSet, block);

        // example: private int width;
        protected internal virtual Parser<FieldDeclarationSyntax> FieldDeclaration =>
            from heading in MemberDeclarationHeading
            from typeAndName in TypeAndName
            from initializer in FieldInitializer
            select new FieldDeclarationSyntax(heading)
            {
                Type = typeAndName.Type,
                Identifier = typeAndName.Identifier,
                Expression = initializer.Expression,
            };

        // example: = DateTime.Now();
        protected internal virtual Parser<FieldDeclarationSyntax> FieldInitializer =>
            from expression in Parse.Char('=').Token().Then(c => Parse.CharExcept(';').Many().Text().Token()).Optional()
            from semicolon in Parse.Char(';').Token()
            select new FieldDeclarationSyntax
            {
                Expression = expression.GetOrDefault(),
            };

        // examples: return true; if (false) return; etc.
        protected internal virtual Parser<StatementSyntax> Statement =>
            from comments in CommentParser.AnyComment.Token().Many()
            from statement in Block.Select(s => s as StatementSyntax)
                .Or(IfStatement)
                .Or(DoStatement)
                .Or(WhileStatement)
                .Or(ForEachStatement)
                .Or(ForStatement)
                .Or(BreakStatement)
                .Or(VariableDeclaration)
                .Or(UnknownGenericStatement)
            select statement.WithComments(comments);

        // examples: {}, { /* empty block */ }, { int a = 0; return; }
        protected internal virtual Parser<BlockSyntax> Block =>
            from openBrace in Parse.Char('{').Token()
            from statements in Statement.Many()
            from trailingComment in CommentParser.AnyComment.Token().Many()
            from closeBrace in Parse.Char('}').Token()
            select new BlockSyntax
            {
                Statements = statements.ToList(),
                CodeComments = trailingComment.ToList(),
            };

        // example: int x, y, z = 3;
        protected internal virtual Parser<VariableDeclarationSyntax> VariableDeclaration =>
            from type in TypeReference
            from declarators in VariableDeclarator.DelimitedBy(Parse.Char(',').Token())
            from semicolon in Parse.Char(';').Token()
            select new VariableDeclarationSyntax
            {
                Type = type,
                Variables = declarators.ToList(),
            };

        // example: now = DateTime.Now()
        protected internal virtual Parser<VariableDeclaratorSyntax> VariableDeclarator =>
            from identifier in Identifier
            from expression in Parse.Char('=').Token().Then(c => GenericExpression).Optional()
            select new VariableDeclaratorSyntax
            {
                Identifier = identifier,
                Expression = expression.GetOrDefault(),
            };

        // dummy generic parser for any unknown statement ending with a semicolon
        protected internal virtual Parser<StatementSyntax> UnknownGenericStatement =>
            from contents in Parse.CharExcept("{};").Many().Text().Token()
            from semicolon in Parse.Char(';').Token()
            select new StatementSyntax
            {
                Body = contents.Trim(),
            };

        // dummy generic parser for any expressions with matching braces
        protected internal virtual Parser<string> GenericExpressionInBraces =>
            from openBrace in Parse.Char('(').Token()
            from expression in GenericExpression.Optional()
            from closeBrace in Parse.Char(')').Token()
            select expression.GetOrDefault();

        // dummy generic parser for expressions with matching braces
        protected internal virtual Parser<string> GenericExpression =>
            from subExpressions in Parse.CharExcept("();,").Many().Text().Token()
                .Or(GenericExpressionInBraces.Select(x => $"({x})")).Many()
            let expr = string.Join(string.Empty, subExpressions)
            where !string.IsNullOrWhiteSpace(expr)
            select expr;

        // example: break;
        protected internal virtual Parser<BreakStatementSyntax> BreakStatement =>
            from @break in Parse.IgnoreCase(ApexKeywords.Break).Token()
            from semicolon in Parse.Char(';').Token()
            select new BreakStatementSyntax();

        // simple if statement without the expressions support
        protected internal virtual Parser<IfStatementSyntax> IfStatement =>
            from ifKeyword in Parse.IgnoreCase(ApexKeywords.If).Token()
            from expression in GenericExpressionInBraces
            from thenBranch in Statement
            from elseBranch in Parse.IgnoreCase(ApexKeywords.Else).Token().Then(_ => Statement).Optional()
            select new IfStatementSyntax
            {
                Expression = expression,
                ThenStatement = thenBranch,
                ElseStatement = elseBranch.GetOrDefault(),
            };

        // simple foreach statement without the expression support
        protected internal virtual Parser<ForEachStatementSyntax> ForEachStatement =>
            from forKeyword in Parse.IgnoreCase(ApexKeywords.For).Token()
            from openBrace in Parse.Char('(').Token()
            from typeReference in TypeReference
            from identifier in Identifier
            from colon in Parse.Char(':').Token()
            from expression in GenericExpression
            from closeBrace in Parse.Char(')').Token()
            from loopBody in Statement
            select new ForEachStatementSyntax
            {
                Type = typeReference,
                Identifier = identifier,
                Expression = expression,
                Statement = loopBody,
            };

        // simple for statement without the expression support
        protected internal virtual Parser<ForStatementSyntax> ForStatement =>
            from forKeyword in Parse.IgnoreCase(ApexKeywords.For).Token()
            from openBrace in Parse.Char('(').Token()
            from declaration in VariableDeclaration.Or(Parse.Char(';').Token().Return(default(VariableDeclarationSyntax)))
            from condition in GenericExpression.Optional()
            from semicolon in Parse.Char(';').Token()
            from incrementors in GenericExpression.DelimitedBy(Parse.Char(',').Token()).Optional()
            from closeBrace in Parse.Char(')').Token()
            from loopBody in Statement
            select new ForStatementSyntax
            {
                Declaration = declaration,
                Condition = condition.GetOrDefault(),
                Incrementors = incrementors.GetOrElse(Enumerable.Empty<string>()).ToList(),
                Statement = loopBody,
            };

        // simple do-while statement without the expression support
        protected internal virtual Parser<DoStatementSyntax> DoStatement =>
            from doKeyword in Parse.IgnoreCase(ApexKeywords.Do).Token()
            from loopBody in Statement
            from whileKeyword in Parse.IgnoreCase(ApexKeywords.While).Token()
            from expression in GenericExpressionInBraces
            from semicolon in Parse.Char(';').Token()
            select new DoStatementSyntax
            {
                Expression = expression,
                Statement = loopBody,
            };

        // simple while statement without the expression support
        protected internal virtual Parser<WhileStatementSyntax> WhileStatement =>
            from whileKeyword in Parse.IgnoreCase(ApexKeywords.While).Token()
            from expression in GenericExpressionInBraces
            from loopBody in Statement
            select new WhileStatementSyntax
            {
                Expression = expression,
                Statement = loopBody,
            };

        // examples: /* this is a member */ @isTest public
        protected internal virtual Parser<MemberDeclarationSyntax> MemberDeclarationHeading =>
            from comments in CommentParser.AnyComment.Token().Many()
            from annotations in Annotation.Many()
            from modifiers in Modifier.Many()
            select new MemberDeclarationSyntax
            {
                CodeComments = comments.ToList(),
                Annotations = annotations.ToList(),
                Modifiers = modifiers.ToList(),
            };

        // example: @TestFixture public static class Program { static void main() {} }
        public virtual Parser<ClassDeclarationSyntax> ClassDeclaration =>
            from heading in MemberDeclarationHeading
            from classBody in ClassDeclarationBody
            select new ClassDeclarationSyntax(heading)
            {
                Identifier = classBody.Identifier,
                BaseType = classBody.BaseType,
                Interfaces = classBody.Interfaces,
                Constructors = classBody.Constructors,
                Methods = classBody.Methods,
                Fields = classBody.Fields,
                Properties = classBody.Properties,
                InnerClasses = classBody.InnerClasses,
            };

        // example: class Program { void main() {} }
        protected internal virtual Parser<ClassDeclarationSyntax> ClassDeclarationBody =>
            from @class in Parse.IgnoreCase(ApexKeywords.Class).Token()
            from className in Identifier
            from baseType in Parse.IgnoreCase(ApexKeywords.Extends).Token().Then(t => TypeReference).Optional()
            from interfaces in Parse.IgnoreCase(ApexKeywords.Implements).Token().Then(t => TypeReference.DelimitedBy(Parse.Char(',').Token())).Optional()
            from openBrace in Parse.Char('{').Token()
            from members in ClassMemberDeclaration.Many()
            from closeBrace in Parse.Char('}').Token()
            select new ClassDeclarationSyntax()
            {
                Identifier = className,
                BaseType = baseType.GetOrDefault(),
                Interfaces = interfaces.GetOrElse(Enumerable.Empty<TypeSyntax>()).ToList(),
                Constructors = GetConstructors(members),
                Methods = GetMethods(members),
                Fields = GetFields(members),
                Properties = GetProperties(members),
                InnerClasses = GetClasses(members),
            };

        private List<ClassDeclarationSyntax> GetClasses(IEnumerable<MemberDeclarationSyntax> members) =>
            members.OfType<ClassDeclarationSyntax>().ToList();

        private List<FieldDeclarationSyntax> GetFields(IEnumerable<MemberDeclarationSyntax> members) =>
            members.OfType<FieldDeclarationSyntax>().ToList();

        private List<PropertyDeclarationSyntax> GetProperties(IEnumerable<MemberDeclarationSyntax> members) =>
            members.OfType<PropertyDeclarationSyntax>().ToList();

        private Func<MethodDeclarationSyntax, bool> IsConstructor { get; } =
            ConstructorDeclarationSyntax.IsConstructor;

        private List<ConstructorDeclarationSyntax> GetConstructors(IEnumerable<MemberDeclarationSyntax> members) =>
            members.OfType<MethodDeclarationSyntax>().Where(m => IsConstructor(m)).Select(m => new ConstructorDeclarationSyntax(m)).ToList();

        private List<MethodDeclarationSyntax> GetMethods(IEnumerable<MemberDeclarationSyntax> members) =>
            members.OfType<MethodDeclarationSyntax>().Where(m => !IsConstructor(m)).ToList();

        // method or property declaration starting with the type and name
        protected internal virtual Parser<MemberDeclarationSyntax> MethodPropertyOrFieldDeclaration =>
            from typeAndName in TypeAndName
            from member in MethodParametersAndBody.Select(c => c as MemberDeclarationSyntax)
                .XOr(PropertyAccessors)
                .XOr(FieldInitializer)
            select member.WithTypeAndName(typeAndName);

        // class members: methods, classes, properties
        protected internal virtual Parser<MemberDeclarationSyntax> ClassMemberDeclaration =>
            from heading in MemberDeclarationHeading
            from member in ClassDeclarationBody.Select(c => c as MemberDeclarationSyntax).Or(MethodPropertyOrFieldDeclaration)
            select member.WithProperties(heading);
    }
}