namespace Demo.CSharpClasses
{
    using Apex.ApexSharp;
    using Apex.ApexSharp.ApexAttributes;
    using Apex.System;
    using ApexSharpApi.ApexApi;
    using SObjects;

    public class ClassInitialization
    {
        public List<Contact> contactList;

        public ClassInitialization()
        {
            contactList = Soql.Query<Contact>("SELECT ID FROM Contact LIMIT 1");
        }

        public static Map<string, string> colorMap = new Map<string, string>();

        static ClassInitialization()
        {
            colorMap.Put("red", "255, 0, 0");
            colorMap.Put("cyan", "0, 255, 255");
            colorMap.Put("magenta", "255, 0, 255");
        }
    }
}