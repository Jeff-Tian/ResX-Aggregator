Feature: Regex
	In order to understand C# Regex

@mytag
Scenario: IsMatch() Function
	Given pattern is "^about\.cshtml.*\.resx$"
	When test target is "about.cshtml.resx"
	Then the /pattern/.IsMatch(target) result should be true
