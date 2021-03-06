﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexSharp.ApexParser.Parser;
using ApexSharp.ApexParser.Syntax;
using NUnit.Framework;
using Sprache;

namespace ApexSharp.ApexParser.Tests.Parser
{
    [TestFixture]
    public class ApexRegressionTests
    {
        private ApexGrammar Apex { get; } = new ApexGrammar();

        [Test]
        public void AccountDaoExtendsBaseDaoIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse("public with sharing class AccountDAO extends BaseDAO { }");
            Assert.AreEqual("AccountDAO", cd.Identifier);
            Assert.AreEqual("BaseDAO", cd.BaseType.Identifier);
        }

        [Test]
        public void AccountDaoTestIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse("@IsTest(seeAllData = false) private class AccountDAOTest { }");
            Assert.AreEqual("AccountDAOTest", cd.Identifier);
            Assert.AreEqual(1, cd.Annotations.Count);
            Assert.AreEqual("IsTest", cd.Annotations[0].Identifier);
            Assert.AreEqual("seeAllData = false", cd.Annotations[0].Parameters);
        }

        [Test]
        public void AccountTeamBatchClassIsParsed()
        {
            var nt = Apex.NonGenericType.Parse("Database");
            var pt = Apex.TypeReference.Parse("Database.batchable<sObject>");

            var cd = Apex.ClassDeclaration.Parse("global class AccountTeamBatchClass implements Database.batchable<sObject> { }");
            Assert.AreEqual("AccountTeamBatchClass", cd.Identifier);
            Assert.AreEqual(1, cd.Interfaces.Count);
            Assert.AreEqual(1, cd.Interfaces[0].Namespaces.Count);
            Assert.AreEqual("Database", cd.Interfaces[0].Namespaces[0]);
            Assert.AreEqual("batchable", cd.Interfaces[0].Identifier);
        }

        [Test]
        public void TestMethodMyUnitTestIsParsed()
        {
            var md = Apex.MethodDeclaration.Parse("static testMethod void myUnitTest() { }");
            Assert.AreEqual("void", md.ReturnType.Identifier);
            Assert.AreEqual("myUnitTest", md.Identifier);
        }

        [Test]
        public void AccountTeamScheduleBatchIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse("global class AccountTeamschduleBatch implements Schedulable { }");
            Assert.AreEqual("AccountTeamschduleBatch", cd.Identifier);
            Assert.AreEqual(1, cd.Interfaces.Count);
            Assert.False(cd.Interfaces[0].Namespaces.Any());
            Assert.AreEqual("Schedulable", cd.Interfaces[0].Identifier);
        }

        [Test]
        public void StaticVariableToCheckWhetherItsRecurringOrNotIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class RecurrencyHelper
            {
                //Static Variable to check whether its recurring or not.
                static bool recurring = false;
            }");

            Assert.AreEqual(1, cd.Fields.Count);
        }

        [Test]
        public void PublicStringStrVinIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class DummyController
            {
                Public String strVIN { get;set; }
            }");

            Assert.AreEqual(1, cd.Properties.Count);
            Assert.AreEqual("strVIN", cd.Properties[0].Identifier);
            Assert.AreEqual("String", cd.Properties[0].Type.Identifier);
        }

        [Test]
        public void PostPaymentResponseExtendsExampleCompanyApi2ResponseObjectsResponseIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse("global class PostPaymentResponse extends ExampleCompany_API2_ResponseObjects.Response {}");
            Assert.AreEqual("PostPaymentResponse", cd.Identifier);
            Assert.AreEqual(1, cd.BaseType.Namespaces.Count);
            Assert.AreEqual("ExampleCompany_API2_ResponseObjects", cd.BaseType.Namespaces[0]);
            Assert.AreEqual("Response", cd.BaseType.Identifier);
        }

        [Test]
        public void CreateAgreementAndAttachmentIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class DummyClass
            {
                void DummyMethod()
                {
                    s=true;
                    //Create Agreement & attachment
                }
            }");

            Assert.AreEqual(1, cd.Methods.Count);
            Assert.AreEqual(1, cd.Methods[0].Body.Statements.Count);
        }

        [Test]
        public void TestMethodForAgreementControllerXIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class AgreementControllerX_Test
            {
                static testMethod void testMethodForAgreementControllerX()
                {
                }
            }");

            Assert.AreEqual(1, cd.Methods.Count);
            Assert.AreEqual("testMethodForAgreementControllerX", cd.Methods[0].Identifier);
        }

        [Test]
        public void PrivateIntegerBatchSizeIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class Dummy
            {
                private bool flag = false;
                private integer batchSize = 0;
            }");

            Assert.AreEqual(2, cd.Fields.Count);
        }

        [Test]
        public void AppShopPublishingQueueDaoIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            public with sharing class AppShopPublishingQueueDAO
            {
                public List<AppShop_Publishing_Queue__c> getRecordsByVersionIdsStatusType(Set<Id> mmyvIds, String jobStatus, String jobType)
                {
                }
            }");

            Assert.AreEqual("AppShopPublishingQueueDAO", cd.Identifier);
            Assert.AreEqual(1, cd.Methods.Count);
            Assert.AreEqual("getRecordsByVersionIdsStatusType", cd.Methods[0].Identifier);
            Assert.AreEqual(3, cd.Methods[0].Parameters.Count);
            Assert.AreEqual("mmyvIds", cd.Methods[0].Parameters[0].Identifier);
            Assert.AreEqual("jobStatus", cd.Methods[0].Parameters[1].Identifier);
            Assert.AreEqual("jobType", cd.Methods[0].Parameters[2].Identifier);
        }

        [Test]
        public void BillingAdjustmentControllerIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            public without sharing class BillingAdjustmentController
            {
                public Zuora__CustomerAccount__c billingAccount { get; set; }
                private String billAcctId { get; set; }
                public Case c { get; set; }
                private String contactId { get; set; }
            }");
        }

        [Test]
        public void MainSetToAddressesNewStringIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class Test
            {
                void Test()
                {
                    mail.setToAddresses(new String[]
                    {
                        email
                    });
                }
            }");
        }

        [Test]
        public void UpdatePaymentMethodBillingAccountIdIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class Test
            {
                void Test()
                {
                    if (status == 'Processed') updatePaymentMethod(billingAccount.Id);
                    //need to write this method and use it after adjustment process
                    else
                    {
                        sendCaseToAdjustmentOperations(adjustmentCase,'Zuora error : Could not perform non reference refund. Status of refund :' + status,System.Label.inReview,true,purchaseResults.subNumber);
                        return;
                    }
                }
            }");
        }

        [Test]
        public void StaticTestMethodIsParsed()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class Test
            {
                static testMethod void StaticTestMethod()
                {
                    //System.AssertNotEquals(5, 0, 'Assert Not Equal');
                    System.AssertNotEquals(5, 0, 'Assert Not Equal');
                    //System.AssertNotEquals(5, 0, 'Assert Not Equal');
                }
            }");
        }

        [Test]
        public void ExceptionIsAValidNameForAMethod()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
            class Test
            {
                static void exception() { }
            }");
        }

        [Test]
        public void StringLiteralParsesJsonStringParts()
        {
            var str = @"'{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""25 - 34"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }'";
            var result = Apex.StringLiteral.Parse(str);
            Assert.AreEqual(result, str);
        }

        [Test]
        public void ExpressionCanContainComments()
        {
            var text = "  'ho /* not a comment */' // it's a quote, a comma, and a semicolon; all within an expression";
            var expr = Apex.GenericExpression.End().Parse(text);
            Assert.AreEqual("'ho /* not a comment */'", expr);
        }

        [Test]
        public void SingleQuotesWithinCommentsAreIgnored()
        {
            var text = @"
            @isTest
            private class SObjectDataLoaderTest {

                @IsTest(seeAllData=true) // http://stackoverflow.com/questions/9164986/how-do-i-avoid-standard-price-not-defined-when-unit-testing-an-opportunitylineit
                public static void testManuallyConfigured()
                {
                    // Save point to rollback test data
                    System.Savepoint savePoint = Database.setSavepoint();

                    // Serialise test data into JSON record set bundle via manual configuration
                    String serializedData = SObjectDataLoader.serialize(createOpportunities(),
                        new SObjectDataLoader.SerializeConfig().
                            followChild(OpportunityLineItem.OpportunityId).     // Serialize any related OpportunityLineItem's (children)
                                follow(OpportunityLineItem.PricebookEntryId).   // Serialize any related PricebookEntry's
                                    follow(PricebookEntry.Product2Id).          // Serialize any related Products's
                                    omit(OpportunityLineItem.UnitPrice));       // Do not serialize the UnitPrice, as TotalPrice is enough and both cannot exist together

                    // Rollback test data
                    Database.rollback(savePoint);

                    // Recreate test data via deserialize
                    Set<ID> resultIds = SObjectDataLoader.deserialize(serializedData, new ApplyStandardPricebook());
                    assertResults(resultIds);
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void FieldInitializedWithCommasInItIsParsed()
        {
            var text = @"
            public class Text
            {
                private Map<String, Object> availableFields = new Map<String, Object>();
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void MultipleFieldsCanBeDeclaredAtOnce()
        {
            var text = @"
            private class Test
            {
                // multiple variables
                private static Account DispAcc, PoiAcc,Poi2Acc;

                public string commas = 'a,b,c', semicolons = 'c;d;e;'; // comments
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void SemicolonsAndCurlyBracesInVariableInitializationExpressions()
        {
            var text = @"
            private class Test
            {
                void Test()
                {
                    string ch = 'France;Germany;Italy;';

                    // We can hold off, I will say lets fix

                    vt.Something = 'value"" : ""1234"" }, { ""key"" : ""tmp"", ""value"" : ""ignored"" } ] }';
                    System.debug('someData [ '+validateSomeCode+' ]');
                    System.debug('someData ['+someData.toString());
                    system.debug(SomeLevel.DEBUG,'12* (lstAcc[0] 1'+ (lstAcc[0].SomeIdentifier));
                    system.debug(SomeLevel.DEBUG,'12* (lstAcc[0]2 '+ (lstAcc[0].SomeIdentifier));
                    vt.SomeIdentifier = '{ ""SomeList"" : [ { ""key"" : ""Size"", ""value"" : ""1234"" }, { ""key"" : ""tmp"", ""value"" : ""ignored"" } ] }';
                    vt.SomeIdentifier = '{ ""SomeList"" : [ { ""key"" : ""Size"", ""value"" : ""1234"" }, { ""key"" : ""tmp"", ""value"" : ""ignored"" } ] }';
                    vt.SomeIdentifier = ' : ""Ignored"" } ] }';
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void SystemTodayExpressionIsAllowed()
        {
            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(@"class Text { DateTime t = System.today(); }"));
        }

        [Test]
        public void SetKeywordCanBeUsedAsName()
        {
            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(@"class Set { }"));
        }

        [Test]
        public void ExampleCompanyApiUtilsLoggerTestCommentedClassIsParsed()
        {
            var text = @"
            @isTest(SeeAllData=false)
            private class ExampleCompany_API_Utils_LoggerTest {
                /**
                 * ************************************************************************************************
                 * Method to test FilterSecureData
                 * ************************************************************************************************
                 */
                /*static testMethod void testConstructorWithOneParam() {
                    Test.startTest();
                    General_Settings__c enableLogging = new General_Settings__c();
                    enableLogging.Name = ExampleCompany_API_Utils_Logger.API_LOG_ENABLED_KEY;
                    enableLogging.Value__c = 'true';
                    insert enableLogging;
                    ExampleCompany_API_Utils_Logger tstObj = new ExampleCompany_API_Utils_Logger('A');
                    System.assertNotEquals(null, tstObj);
                    Test.stopTest();
                }  */
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void ExampleCompanyApiDomainObjectsTaxCommentedClassIsParsed()
        {
            var text = @"
            global with sharing class ExampleCompany_API_DomainObjects {

                global class Tax {
                    //global String name {get;set;} // Not in scope for 4/20/2015 Release
                    //global String description {get;set;} // Not in scope for 4/20/2015 Release
                    global Decimal value {get;set;}
                    global String currencyCode {get;set;}
                    //global List<JurisdictionCode> jurisdictionCodes {get;set;}  // Not in scope for Info3
                    //global String id {get;set;} // Not in scope for Info3
                    //global Boolean isAccessBased {get;set;} // // Not in scope for Info3
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void ExampleCompanyApiZuoraSyncTablesUtilCommentedClassIsParsed()
        {
            var text = @"
            global with sharing class ExampleCompany_API_ZuoraSyncTablesUtil {

            /*
                //BRM-02 Temp Wrapper
                public Static ExampleCompany_API_QuoteManagementFacade.QuoteLineItem getProductRatePlan(
                                        ExampleCompany_API_DomainObjects.LineItem lineItem,
                                        String profileCountry,
                                        Vehicle__c vehicle)
                {
                    return getProductRatePlan(lineItem, profileCountry, vehicle, null);
                }
                */
                /**
                 * ************************************************************************************************
                 * Get the Product Rate Plan for the given context
                 * Returns null if Rate Plan not found
                 * ************************************************************************************************
                 */
              }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void ExampleCompanyAuthenticationProviderTestIsParsed()
        {
            var text = @"
            public class ExampleCompany_AuthenticationProviderTest {
                public Case InsertTestRecord() {}

                public void RequestCertCaseString(Case demoCase) {
                    ExampleCompany_AuthenticationProvider provider = new ExampleCompany_pAPI_Appsigner();
                    Integer statusCode = provider.RequestCert(demoCase, 'Test').StatusCode;

                    System.debug(statusCode);
                }

                static testMethod void RequestCertUsingAppVersionIdIdString() {
                    ExampleCompany_AuthenticationProvider provider = new ExampleCompany_pAPI_Appsigner();
                    Integer statusCode = provider.RequestCertUsingAppVersionId('a1234567890','Test').StatusCode;
                    System.assertEquals(202, statusCode);
                    System.debug(statusCode);
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void ConvertCurrencyIsAValidMethodName()
        {
            var text = @"
            public class Test {
                public static decimal convertCurrency(String fromISO, String toISO, Decimal amt) {
                    return 0;
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void RestResponseInstrumenterIsParsed()
        {
            var text = @"
            public class RestResponseInstrumenter {
                private class Insturmentation
                {
                    String instrument(final String s)
                    {
                    }
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void NavPackageControllerIsParsed()
        {
            var text = @"
            public with sharing class NavPackageController1 {

                private List<PackageDetails>  sort(List<PackageDetails> pkgList)
                {
                }
             }
            ";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void OfferCategoryIsParsed()
        {
            var text = @"
            public  with sharing class OfferCategory
            {
                public boolean isChangeCommitted
                {
                    get // declaration of get
                    {
                        return true;
                    }
                    private set; //declaration of set
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void VehicleLifecycleHandlerIsParsed()
        {
            var text = @"
            global with sharing class VehicleLifecycleHandler
            {
                public static void eventSold(String vin )
                {
                        try        //ONS-21
                        {


                        }        //ONS-21
                        catch(Exception e)    //ONS-21
                        {            //ONS-21

                        }    //ONS-21
                    }
                }
            }";
            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void RunAsParsed()
        {
            var text = @"
            class Test
            {
                @isTest public static void LogErrorExceptionWithMessage()
                {
                    System.runAs(Info3TestFactory.getGatewayAdminUser())
                    {

                    }
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void JsonStringGeneratorIsParsed()
        {
            var text = @"
            class Test
            {
                public void Test()
                {
                    string reqBody = '';
                    reqBody = '{""size"":1,""application"":[{""id"":""com.gm.testxy111.pkg"",""version"":""1"",""action"":""Update"",""status"":{""code"":""success""}}]}';
                    reqBody = '{""size"":0,""application"":[]}';
                    reqBody = '{""size"":1,""application"":[{""id"":""com.gm.testxy111.pkg"",""version"":""1"",""action"":""Update"",""status"":{""code"":""success""}}]}';
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void JsonStringGeneratorIsParsed2()
        {
            var text = @"
            class Test
            {
                public void Test()
                {
                    tmp.Some_Identifier__tmp = '{""category"":[{""name"":""L10N.Translations"",""details"":{""data"":[{""val"":""50"",""name"":""RecentDestinationsMax""}]}},{""name"":""L10N.Translations"",""metadata"":{""property"":[{""val"":""6"",""name"":""HomeScreenPages""},{""val"":""2"",""name"":""HomeScreenRows""},{""val"":""4"",""name"":""HomeScreenCols""}]}},{""name"":""L10N.Translations"",""metadata"":{""property"":[{""val"":""40"",""name"":""FavoritesAudioMax""},{""val"":""10"",""name"":""Unidentified""},';
                    tmp.Some_Identifier__tmp += '{""tmp"":""20"",""name"":""Something""},{""val"":""2"",""name"":""FavoriteBundleVersion""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},';
                    tmp.Some_Identifier__tmp += '{""tmp"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},';
                    tmp.Some_Identifier__tmp += '{""tmp"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""true"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""true"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},{""val"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""},';
                    tmp.Some_Identifier__tmp += '{""tmp"":""false"",""name"":""IRQ_NOT_LESS_OR_EQUAL""}]}},{""name"":""L10N.Translations"",""metadata"":{""property"":[{""val"":""1920x1080"",""name"":""HMIResolution""},{""val"":""5.1.1"",""name"":""AndroidOS""},{""val"":""tmp"",""name"":""BuildVersion""},{""val"":""1"",""name"":""GMFrameworkVersion""},{""val"":""From CPUInfo"",""name"":""DeviceProcessor""}]}}]}';
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void JsonStringGeneratorIsParsed3()
        {
            var text = @"
            class Test
            {
                public void Test()
                {
                    tmp.requestBody = Blob.valueof('{test}');
                    tmp.requestBody = Blob.valueof('{test}');
                    tmp.setBody('{""name"":""name""}');
                    tmp.tmpName = new List<String> { 'test1', 'test2' };
                    tmp.tmpDesc = new List<String> { 'test1', 'test2' };
                    tmp.SomeIdentifier__tmp = '{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = '{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = ' {}8236kjah{}{';
                    tmp.SomeIdentifier__tmp = '{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = 'value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = '{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = '{ ""SegmentationFault"" : [ { ""key"" : ""Age"", ""value"" : ""1234"" }, { ""key"" : ""Species"", ""value"" : ""Beaver"" } ] }';
                    tmp.SomeIdentifier__tmp = ' : ""Temp"" } ] }';
                    tmp.requestBody = Blob.valueOf('{ ""checklist"": [ { ""checklistItem"": [ { ""id"": ""' + existingChecklist0.id + '"", ""isChecked"": false }, { ""id"": ""' + newChecklist0.id + '"", ""isChecked"": true }, { ""id"": ""' + newChecklist1.id + '"", ""isChecked"": false } ] } ] }');
                    result = Something.replace(new list<tmp.SomeRequest>{req}, tmp);
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void WeirdVariableDeclarationIsParsed()
        {
            var text = @"
            class Test
            {
                public void Test()
                {
                    int a = new Map<string, string>().count() + new Map<string, string>().count();
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void CommentsInMethodParametersAreParsed()
        {
            var text = @"
            class Test
            {
                public void Test(int a, // first parameter
                    int b // second parameter
                )
                {
                    int a = new Map<string, string>().count() + new Map<string, string>().count();
                }
            }";

            Assert.DoesNotThrow(() => Apex.ClassDeclaration.Parse(text));
        }

        [Test]
        public void InnerInterfaceIsParserAsInterface()
        {
            var text = @"
            class Test
            {
                interface InnerTest
                {
                }
            }";

            var cd = Apex.ClassDeclaration.Parse(text);
            Assert.AreEqual(1, cd.Members.Count);
            Assert.AreEqual(1, cd.InnerClasses.Count);

            var inner = cd.Members.OfType<InterfaceDeclarationSyntax>().FirstOrDefault();
            Assert.NotNull(inner);
        }

        [Test]
        public void MethodLeadingCommentsAfterInnerClassArePreserved()
        {
            var text = @"
            class Test
            {
                class InnerTest
                {
                }
                // leading comments
                void Hello()
                {
                }
            }";

            var cd = Apex.ClassDeclaration.Parse(text);
            Assert.AreEqual(1, cd.Methods.Count);
            var md = cd.Methods[0];
            Assert.AreEqual(1, md.LeadingComments.Count);
        }

        [Test]
        public void ClassInnerCommentsAfterInnerClassArePreserved()
        {
            var text = @"
            class Test
            {
                // leading
                class InnerTest
                {
                    // inner comment
                } // trailing comment

                // inner comments
            }";

            var cd = Apex.ClassDeclaration.Parse(text);
            Assert.AreEqual(1, cd.InnerClasses.Count);
            Assert.AreEqual(1, cd.InnerComments.Count);

            cd = cd.InnerClasses[0];
            Assert.AreEqual(1, cd.LeadingComments.Count);
            Assert.AreEqual(1, cd.InnerComments.Count);
            Assert.AreEqual(1, cd.TrailingComments.Count);
        }

        [Test]
        public void BatchCleanLandingZoneChartsIsCompiled()
        {
            var text = @"
            global class BatchCleanLandingZoneCharts implements Database.Batchable<sObject> {
                global String query;
                global List<sObject> gllstScopeRecords;
                global Datetime yesterday;

                global BatchCleanLandingZoneCharts() {
                    yesterday = System.now()-1;
                    query = 'SELECT Id, CreatedDate FROM Landing_Zone_Chart__c WHERE CreatedDate < :yesterday';
                }

                global Database.QueryLocator start(Database.BatchableContext BC) {
                    return Database.getQueryLocator(query);
                }

                global void execute(Database.BatchableContext BC, List<sObject> scope) {
                    gllstScopeRecords = scope;
                }

                global void finish(Database.BatchableContext BC) {
                }
            }";

            var cd = Apex.ClassDeclaration.Parse(text);
            Assert.AreEqual("BatchCleanLandingZoneCharts", cd.Identifier);
        }

        [Test]
        public void MultipleFieldDeclaratorsWithCommentsInBetween()
        {
            var cd = Apex.ClassDeclaration.Parse(@"
                class Sample {
                    final string a = '123',
                        // a comment in between
                        b = '321';
                }");

            Assert.AreEqual("Sample", cd.Identifier);
        }
    }
}
