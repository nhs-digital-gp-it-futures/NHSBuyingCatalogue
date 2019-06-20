Feature: StandardsEvidence
	In order to prove my solution meets the standards
	As a supplier
	I want to be able to upload evidence for standards compliance

Background: 
	Given A solution has been created
	And the capability evidence has been submitted

Scenario: The Standards Evidence screen is displayed correctly
	Given I click the Start button below the Standards Evidence section
	Then the page title should be 'Standards Dashboard'
	And two tables should be displayed for the standards

Scenario: The Standards Evidence screen should show the correct standards
	Given I click the Start button below the Standards Evidence section
	Then all standards from the selected capabilities are displayed

Scenario: The help reveal contains the correct details
	Given I click the Start button below the Standards Evidence section
	When I click the Help - What are the associated Standards link
	Then the help text should be correct

Scenario: Clicking a standard opens the correct page
	Given I click the Start button below the Standards Evidence section
	When I click on a standard
	Then the correct standard page should be displayed