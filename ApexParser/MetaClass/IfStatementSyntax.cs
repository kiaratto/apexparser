﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexParser.Visitors;

namespace ApexParser.MetaClass
{
    public class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax()
        {
            Kind = SyntaxType.IfStatement;
        }

        public override void Accept(ApexSyntaxVisitor visitor) => visitor.VisitIfStatement(this);

        public string Expression { get; set; }

        public StatementSyntax ThenStatement { get; set; }

        public StatementSyntax ElseStatement { get; set; }
    }
}