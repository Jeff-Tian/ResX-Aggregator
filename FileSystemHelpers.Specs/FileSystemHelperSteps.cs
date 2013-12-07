using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TechTalk.SpecFlow;

namespace ZiZhuJY.FileSystemHelpers.Specs
{
    [Binding]
    public class FileSystemHelperSteps
    {
        private string[] _resultFileNames = null;
        private string _testFolder = string.Empty;

        [Given(@"I have created some files into the test folder")]
        public void GivenIHaveCreatedSomeFilesIntoTestFolder(Table table)
        {
            _testFolder = FeatureContext.Current.Get<string>("TestFolder");
            Debug.Assert(_testFolder != null, "_testFolder != null");

            foreach (var row in table.Rows)
            {
                File.Create(Path.Combine(_testFolder, row[0])).Close();
            }
        }
        
        [When(@"I specify a pattern ""(.*)"" to do the searching")]
        public void WhenISpecifyAPatternToDoTheSearching(string pattern)
        {
            _resultFileNames = FileSystemHelper.SearchFileNames(_testFolder, pattern);
        }
        
        [Then(@"the result should be these list of file names:")]
        public void ThenTheResultShouldBeTheseListOfFileNames(Table table)
        {
            var q = from a in _resultFileNames
                join b in table.Rows.Select(row=>row[0]) on a equals b
                select a;

            var equals = _resultFileNames.Length == table.Rows.Count && q.Count() == _resultFileNames.Length;

            Assert.AreEqual(true, equals);
        }
    }
}
