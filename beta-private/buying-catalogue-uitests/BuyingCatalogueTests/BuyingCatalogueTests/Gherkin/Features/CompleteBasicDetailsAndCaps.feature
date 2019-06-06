Feature: Complete basic details and Caps

Scenario: Complete basic details and capabilities
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I complete all basic details
	And The list of capabilities is displayed	
	When I select a foundational capability
	And  I select an other capability
	And I click the Save & Complete button
	Then I should see the onboarding page with the name of the solution