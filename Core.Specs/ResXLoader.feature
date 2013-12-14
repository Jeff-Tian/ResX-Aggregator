Feature: ResXLoader

@file
Scenario: Load resources from resx files
	Given a resx-aggregator file full path is <.\index.cshtml.resx-aggregator>
	And there is a resx file which full path is <.\index.cshtml.resx> has these contents:
	| Name            | Value                    |
	| CloseLinkDialog | Stäng                    |
	| CloseMsg        | Stäng meddelandefönstret |
	| CloseTips       | Stäng hjälp              |
	And there is a resx file which full path is <.\index.cshtml.en-US.resx> has these contents:
	| Name            | Value                |
	| CloseLinkDialog | Close                |
	| CloseMsg        | Close message window |
	| CloseTips       | Close help           |
	When I load the resx-aggregator file
	Then the dictionary in the ResXLoader should be
	| Name            | default                  | en-US                |
	| CloseLinkDialog | Stäng                    | Close                |
	| CloseMsg        | Stäng meddelandefönstret | Close message window |
	| CloseTips       | Stäng hjälp              | Close help           |
