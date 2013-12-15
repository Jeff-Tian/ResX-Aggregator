using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private ResXAggregator _resxAggregator;

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
            _resxAggregator = new ResXAggregator(_resxAggregatorFileFullPath);
        }
        
        [Then(@"the dictionary in the ResXLoader should be")]
        public void ThenTheDictionaryInTheResXLoaderShouldBe(Table table)
        {
            Assert.AreEqual(table.Rows.Count, _resxAggregator.Dictionary.Count);
            foreach (var row in table.Rows)
            {
                var key = row["Name"];
                Assert.AreEqual(true, _resxAggregator.Dictionary.ContainsKey(key));

                var dic = _resxAggregator.Dictionary[key];

                var i = 0;
                foreach (var column in row.Keys)
                {
                    i++;
                    if(i==0)continue;

                    Assert.AreEqual(true, dic.ContainsKey(column));
                    Assert.AreEqual(row[column], dic[column]);
                }
            }
        }

        [Given(@"I edited the resx-aggregator file and it looks like this now")]
        public void GivenIEditedTheResx_AggregatorFileAndItLooksLikeThisNow(Table table)
        {
            _resxAggregator = new ResXAggregator(_resxAggregatorFileFullPath);
            _resxAggregator.Dictionary.Clear();

            foreach (var row in table.Rows)
            {
                _resxAggregator.Dictionary.Add(row["Name"], new Dictionary<string, string>());

                var i = 0;
                foreach (var column in row)
                {
                    i++;
                    if (i == 1) continue;

                    _resxAggregator.Dictionary[row["Name"]].Add(column.Key, column.Value);
                }
            }
        }

        [When(@"I save the resx-aggregator file")]
        public void WhenISaveTheResx_AggregatorFile()
        {
            _resxAggregator.Save();
        }

        [Then(@"there would be a file whose full path is <(.*)> has these contents:")]
        public void ThenThereWouldBeAFileWhoseFullPathIsHasTheseContents(string filePath, Table table)
        {
            var fullPath = Path.Combine(_testFolder, filePath);

            Assert.AreEqual(true, File.Exists(fullPath));

            var actualDictionary = new Dictionary<string, string>();

            using (var fileStream = File.OpenRead(fullPath))
            {
                using (var resxReader = new ResXResourceReader(fileStream))
                {
                    foreach (DictionaryEntry item in resxReader)
                    {
                        actualDictionary.Add(item.Key.ToString(), item.Value.ToString());
                    }

                    resxReader.Close();
                }

                fileStream.Close();
            }

            Assert.AreEqual(table.Rows.Count, actualDictionary.Count);
            foreach (var row in table.Rows)
            {
                Assert.AreEqual(true, actualDictionary.ContainsKey(row["Name"]));
                Assert.AreEqual(true,
                    actualDictionary[row["Name"]].Equals(row["Value"], StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
