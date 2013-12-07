using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ZiZhuJY.FileSystemHelpers.Specs
{
    [Binding]
    public class RegexSteps
    {
        private Regex _regex;
        private bool _result;

        [Given(@"pattern is ""(.*)""")]
        public void GivenPatternIs(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }
        
        [When(@"test target is ""(.*)""")]
        public void WhenTestTargetIs(string testTarget)
        {
            _result = _regex.IsMatch(testTarget);
        }
        
        [Then(@"the /pattern/\.IsMatch\(target\) result should be true")]
        public void ThenThePattern_IsMatchTargetResultShouldBeTrue()
        {
            Assert.AreEqual(true, _result);
        }
    }
}
