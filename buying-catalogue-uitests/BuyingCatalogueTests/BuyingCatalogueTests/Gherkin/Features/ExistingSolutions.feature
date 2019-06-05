Feature: ExistingSolutions
	As a supplier with existing solutions
	I want to be able to view the solutions in my portfolio

Scenario: Open Existing Solution
	Given I have opened a solution	
	Then the solution title should be displayed
	
Scenario: View Solution Details
	Given I have opened a solution
	When I click on the solution details
	Then the solution title should be displayed in the details section