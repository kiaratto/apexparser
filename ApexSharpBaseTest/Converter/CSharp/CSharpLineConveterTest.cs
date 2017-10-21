﻿using ApexSharpBase.Converter.Apex;
using NUnit.Framework;

namespace ApexSharpBaseTest.Converter.CSharp
{
    [TestFixture]
    public class CSharpLineConveterTest
    {
        [Test]
        public void SoqlLineTest()
        {
            var reply = ExpressionConverter.GetApexLine(@"List<Contact> contacts = Soql.Query<Contact>(""SELECT Id, Email, Name FROM Contact WHERE Id = :contactNewId LIMIT 1"", new { contactNewId })");
            Assert.AreEqual("List<Contact> contacts = [SELECT Id, Email, Name FROM Contact WHERE Id = :contactNewId LIMIT 1]", reply);
        }

        [Test]
        public void SoqlUpdateTest()
        {
            var reply = ExpressionConverter.GetApexLine(@"Soql.Update(accountList)");
            Assert.AreEqual(@"update accountList", reply);
        }

        [Test]
        public void SoqlUpsert()
        {
            var reply = ExpressionConverter.GetApexLine("Soql.Upsert(accountList)");
            Assert.AreEqual(@"upsert accountList", reply);
        }

        [Test]
        public void SoqlInsertTest()
        {
            var reply = ExpressionConverter.GetApexLine("Soql.Insert(accountList)");
            Assert.AreEqual("insert accountList", reply);
        }

        [Test]
        public void SoqlDeleteTest()
        {
            var reply = ExpressionConverter.GetApexLine(@"Soql.Delete(accountList)");
            Assert.AreEqual("delete accountList", reply);
        }

        [Test]
        public void SoqlUnDeleteTest()
        {
            var reply = ExpressionConverter.GetApexLine(@"Soql.UnDelete(accountList);");
            Assert.AreEqual(@"undelete accountList", reply);
        }

        [Test]
        public void JsonDeSerializeTest()
        {
            var reply = ExpressionConverter.GetApexLine(@"List<Account> accounts = JSON.deserialize<List<Account>>(objectToDeserialize)");
            Assert.AreEqual(@"List<Account> accounts = (List<Account>)JSON.deserialize(objectToDeserialize,List<Account>.class)", reply);
        }

        // string json = JSON.serialize(objectToDeserialize)
    }
}
