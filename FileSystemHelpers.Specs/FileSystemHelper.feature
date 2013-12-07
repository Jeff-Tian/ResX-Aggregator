Feature: FileSystemHelper
	In order to easily search files in a folder
	I want to be able to get their file names through specifying a pattern

@file
Scenario: Search file through a string pattern
	Given I have created some files into the test folder
	| File name               |
	| index.cshtml.resx       |
	| index.cshtml.en-US.resx |
	| index.cshtml.zh.resx    |
	| index.cshtml            |
	| about.cshtml            |
	| about.cshtml.resx       |
	| about.cshtml.en-US.resx |
	| about.cshtml.zh.resx    |
	| site.css                |
	When I specify a pattern "^about\.cshtml.*\.resx$" to do the searching
	Then the result should be these list of file names:
	|File name|
	| about.cshtml.resx       |
	| about.cshtml.en-US.resx |
	| about.cshtml.zh.resx    |
