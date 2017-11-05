﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexParser.Visitors;

namespace ApexParser.MetaClass
{
    public class ForStatementSyntax : StatementSyntax
    {
        public override SyntaxType Kind => SyntaxType.ForStatement;

        public override void Accept(ApexSyntaxVisitor visitor) => visitor.VisitForStatement(this);

        public override IEnumerable<BaseSyntax> ChildNodes => GetNodes(Declaration, Statement);

        public VariableDeclarationSyntax Declaration { get; set; }

        public string Condition { get; set; }

        public List<string> Incrementors { get; set; }

        public StatementSyntax Statement { get; set; }
    }
}
