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

@file
Scenario: Save resources back to resx files
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
	And I edited the resx-aggregator file and it looks like this now	
	| Name            | default                  | en-US                | fr                           |
	| CloseLinkDialog | Stäng                    | Close                | proche                       |
	| CloseMsg        | Stäng meddelandefönstret | Close message window | Fermer la fenêtre de message |
	| CloseTips       | Stäng hjälp              | Close help           | Fermer l'aide                |
	| Test            | test                     | test                 | test                         |
	When I save the resx-aggregator file
	Then there would be a file whose full path is <.\index.cshtml.resx> has these contents:	
	| Name            | Value                    |
	| CloseLinkDialog | Stäng                    |
	| CloseMsg        | Stäng meddelandefönstret |
	| CloseTips       | Stäng hjälp              |
	| Test            | test                     |
	And there would be a file whose full path is <.\index.cshtml.en-US.resx> has these contents:
	| Name            | Value                |
	| CloseLinkDialog | Close                |
	| CloseMsg        | Close message window |
	| CloseTips       | Close help           |
	| Test            | test                 |
	And there would be a file whose full path is <.\index.cshtml.fr.resx> has these contents:
	| Name            | Value                           |
	| CloseLinkDialog | proche                       |
	| CloseMsg        | Fermer la fenêtre de message |
	| CloseTips       | Fermer l'aide                |
	| Test            | test                         |