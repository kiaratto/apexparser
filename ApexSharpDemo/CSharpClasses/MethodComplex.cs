using Apex.ApexSharp.ApexAttributes;namespace ApexSharpDemo.CSharpClasses
{    using Apex.ApexSharp;
    using Apex.System;
    using SObjects;
    using SalesForceAPI.ApexApi;

    [WithSharing]
    public class MethodComplex
    {
        public static void MethodOne()
        {
            foreach (Account a in Soql.Query<Account>("SELECT Id FROM Account"))
            {
                System.Debug(a.Id);
            }

            for (int i = 0; i<10; i++)
            {
            }
        }

        public Database.QueryLocator QueryLocator(Database.BatchableContext bc)
        {
            return Database.GetQueryLocator(Soql.Query<Contact>("SELECT Id FROM Contact"));
        }
    }
}
