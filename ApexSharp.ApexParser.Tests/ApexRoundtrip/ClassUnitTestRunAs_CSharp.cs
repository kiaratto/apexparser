namespace ApexSharpDemo.ApexCode
{
    using Apex;
    using Apex.ApexSharp;
    using Apex.ApexSharp.ApexAttributes;
    using Apex.ApexSharp.Extensions;
    using Apex.System;
    using SObjects;

    public class ClassUnitTestRunAs
    {
        static void RunAsExample()
        {
            User newUser = Soql.query<User>(@"SELECT Id FROM User LIMIT 1");
            using (System.runAs(newUser))
            {
            }
        }
    }
}
