﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.18051
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ZiZhuJY.FileSystemHelpers.Specs
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("FileSystemHelper")]
    public partial class FileSystemHelperFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "FileSystemHelper.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "FileSystemHelper", "In order to easily search files in a folder\r\nI want to be able to get their file " +
                    "names through specifying a pattern", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Search file through a string pattern")]
        [NUnit.Framework.CategoryAttribute("file")]
        public virtual void SearchFileThroughAStringPattern()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Search file through a string pattern", new string[] {
                        "file"});
#line 6
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "File name"});
            table1.AddRow(new string[] {
                        "index.cshtml.resx"});
            table1.AddRow(new string[] {
                        "index.cshtml.en-US.resx"});
            table1.AddRow(new string[] {
                        "index.cshtml.zh.resx"});
            table1.AddRow(new string[] {
                        "index.cshtml"});
            table1.AddRow(new string[] {
                        "about.cshtml"});
            table1.AddRow(new string[] {
                        "about.cshtml.resx"});
            table1.AddRow(new string[] {
                        "about.cshtml.en-US.resx"});
            table1.AddRow(new string[] {
                        "about.cshtml.zh.resx"});
            table1.AddRow(new string[] {
                        "site.css"});
#line 7
 testRunner.Given("I have created some files into the test folder", ((string)(null)), table1, "Given ");
#line 18
 testRunner.When("I specify a pattern \"^about\\.cshtml.*\\.resx$\" to do the searching", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "File name"});
            table2.AddRow(new string[] {
                        "about.cshtml.resx"});
            table2.AddRow(new string[] {
                        "about.cshtml.en-US.resx"});
            table2.AddRow(new string[] {
                        "about.cshtml.zh.resx"});
#line 19
 testRunner.Then("the result should be these list of file names:", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
