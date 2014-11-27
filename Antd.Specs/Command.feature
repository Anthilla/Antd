Feature: Command
	Test command page


Scenario: execut ls and check if there is "Antd.exe"
	Given I go to page command
	When I check "ls" command result
	Then The result contains "Antd.exe" string
	And The result contains "Nancy.dll" string

Scenario: execut ls and check if there is "Nancy.dll"
	Given I go to page command
	When I check "ls" command result
	Then The result contains "Nancy.dll" string

Scenario: execut ls and check if there is "Owin.dll"
	Given I go to page command
	When I check "ls" command result
	Then The result contains "Owin.dll" string
