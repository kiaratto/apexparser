﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexParser;
using NUnit.Framework;
using static ApexParserTest.Properties.Resources;

namespace ApexParserTest.CodeGenerators
{
    [TestFixture]
    public class ApexResourceTests : TestFixtureBase
    {
        private void Check(string source, string expected) =>
            CompareLineByLine(ApexParser.ApexParser.IndentApex(source), expected);

        [Test]
        public void ClassOneIsFormattedUsingNewApexFormatter() =>
            Check(ClassOne, ClassOne_Formatted);

        [Test]
        public void ClassTwoIsFormattedUsingNewApexFormatter() =>
            Check(ClassTwo, ClassTwo_Formatted);

        [Test]
        public void ClassWithCommentsIsFormattedUsingNewApexFormatter() =>
            Check(ClassWithComments, ClassWithComments_Formatted);

        [Test]
        public void CustomerDtoIsFormattedUsingNewApexFormatter() =>
            Check(CustomerDto, CustomerDto_Formatted);

        [Test]
        public void FormatDemoIsFormattedUsingNewApexFormatter() =>
            Check(FormatDemo, FormatDemo_Formatted);
    }
}