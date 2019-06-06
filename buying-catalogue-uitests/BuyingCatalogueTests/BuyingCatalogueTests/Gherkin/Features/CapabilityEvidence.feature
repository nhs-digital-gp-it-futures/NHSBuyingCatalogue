Feature: CapabilityEvidence
	
Background: 
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I complete all basic details
	And The list of capabilities is displayed	
	When I select a foundational capability
	And  I select an other capability
	And I click the Save & Complete button
	Then I should see the onboarding page with the name of the solution

Scenario: Contact Message displays
	Given I click the `Start` link below the Capabilities Evidence section
	Then I can click 'Schedule live witness demonstration'

Scenario: Error messages display for each Capability when nothing selected
	Given I click the `Start` link below the Capabilities Evidence section
	When I click the 'Save and review' button
	Then An error for each capability should be displayed