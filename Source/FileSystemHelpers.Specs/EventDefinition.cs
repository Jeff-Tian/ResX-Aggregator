using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TechTalk.SpecFlow;

namespace ZiZhuJY.FileSystemHelpers.Specs
{
    [Binding]
    public class EventDefinition
    {
        internal const string DoCommit = "commit";
        internal const string InteractiveWithFileSystem = "file";

        [BeforeScenario]
        public void BeforeScenario()
        {
            if (FeatureContext.Current.FeatureInfo.Tags.Contains(InteractiveWithFileSystem) ||
                ScenarioContext.Current.ScenarioInfo.Tags.Contains(InteractiveWithFileSystem))
            {
                DeleteTestFolderIfExists();
                SetupTestFolder();
            }

            if (FeatureContext.Current.FeatureInfo.Tags.Contains(DoCommit) ||
                ScenarioContext.Current.ScenarioInfo.Tags.Contains(DoCommit)) return;

            var transaction = new TransactionScope();
            ScenarioContext.Current.Set(transaction, "ScenarioTransaction");
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (!FeatureContext.Current.FeatureInfo.Tags.Contains(DoCommit) &&
                !ScenarioContext.Current.ScenarioInfo.Tags.Contains(DoCommit))
            {
                ScenarioContext.Current.Get<TransactionScope>("ScenarioTransaction").Dispose();
            }
            
            if (FeatureContext.Current.FeatureInfo.Tags.Contains(InteractiveWithFileSystem) ||
                ScenarioContext.Current.ScenarioInfo.Tags.Contains(InteractiveWithFileSystem))
            {
                DeleteTestFolderIfExists();
            }
        }

        private static void SetupTestFolder()
        {
            var testFolder = Path.Combine(Directory.GetCurrentDirectory(), "TestFolder");
            Directory.CreateDirectory(testFolder);
            FeatureContext.Current.Set(testFolder, "TestFolder");
        }

        private static void DeleteTestFolderIfExists()
        {
            var testFolder = FeatureContext.Current.ContainsKey("TestFolder")
                ? FeatureContext.Current.Get<string>("TestFolder")
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(testFolder) && Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
            }
        }
    }
}
