Feature: AddCapabilites
	As a Supplier
	I want to be able to add Capabilities to my solution
	So that the buyers know what the solution can do
	
Background: 
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I complete the Basic Details Section
	And I complete the Lead Contact Details section
	And I click Save & Continue the Which Capabilities does your Solution provide?* page is displayed

Scenario: Adding a capability from each section increments the counter
	Given The list of capabilities is displayed	
	When I select a foundational capability
	And  I select an other capability
	Then the capabilities selected counter is '2'
	And the capabilities are listed beneath the counter

Scenario: Adding a foundation capability adds the relevant standards
	Given The list of capabilities is displayed
	When I select a foundational capability and save its standards
	Then The list of standards contains the standards from the capability

Scenario: Adding an Other capability adds the relevant standards
	Given The list of capabilities is displayed
	When I select an Other capability and save its standards
	Then The list of standards contains the standards from the capability

Scenario: Clicking save before adding any capabilities should throw an error
	Given The list of capabilities is displayed
	When I click the Save & Complete button
	Then an error should be thrown 'Select at least one capability to continue'