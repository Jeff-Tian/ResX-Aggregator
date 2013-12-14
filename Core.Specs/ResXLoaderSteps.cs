using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ZiZhuJY.ResX_Aggregator.Core.Specs
{
    [Binding]
    public class ResXLoaderSteps
    {
        private string _testFolder = string.Empty;
        private string _resxAggregatorFileFullPath;
        private ResXLoader _resxLoader;

        [Given(@"a resx-aggregator file full path is <(.*)>")]
        public void GivenAResx_AggregatorFileFullPathIs(string p0)
        {
            _testFolder = FeatureContext.Current.Get<string>("TestFolder");
            Debug.Assert(_testFolder != null, "_testFolder != null");

            _resxAggregatorFileFullPath = Path.Combine(_testFolder, p0);
        }
        
        [Given(@"there is a resx file which full path is <(.*)> has these contents:")]
        public void GivenThereIsAResxFileWhichFullPathIsHasTheseContents(string p0, Table table)
        {
            using (var fileStream = File.Create(Path.Combine(_testFolder, p0)))
            {
                using (var resxWriter = new ResXResourceWriter(fileStream))
                {
                    foreach (var row in table.Rows)
                    {
                        resxWriter.AddResource(row["Name"], row["Value"]);
                    }

                    resxWriter.Generate();
                    resxWriter.Close();
                }

                fileStream.Close();
            }
        }
        
        [When(@"I load the resx-aggregator file")]
        public void WhenILoadTheResx_AggregatorFile()
        {
            _resxLoader = new ResXLoader(_resxAggregatorFileFullPath);
        }
        
        [Then(@"the dictionary in the ResXLoader should be")]
        public void ThenTheDictionaryInTheResXLoaderShouldBe(Table table)
        {
            Assert.AreEqual(table.Rows.Count, _resxLoader.Dictionary.Count);
        }
    }
}
