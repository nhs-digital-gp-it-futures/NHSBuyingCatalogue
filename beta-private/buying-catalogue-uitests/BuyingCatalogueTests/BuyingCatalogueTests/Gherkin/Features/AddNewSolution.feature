Feature: AddNewSolution
	As a supplier
	I want to add a new solution
	So that I can sell the solution to a buyer

Scenario: Add Solution Page is displayed correctly
	Given I click the `Add a solution` button
	Then I should see the onboarding page with a title of 'Adding a Solution'

Scenario: Solution cannot be saved with an empty name
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	When I click Save without filling in the Solution Name Field
	Then The Solution Name Error should be 'Solution name is missing'

Scenario: Solution cannot be saved with an empty description
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	When I click Save without filling in the Solution Description Field
	Then The Solution Description Error should be 'Summary description is missing'

Scenario:  Solution cannot be saved with an empty Lead Contact
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I complete the Basic Details Section
	When I click save without completing the Lead Contact details sections
	Then The correct errors for the Lead Contact are displayed

Scenario: Solution can have lots of additional contacts added
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I complete the Basic Details Section
	And I complete the Lead Contact Details section
	Then I can add a lot of additional contacts

Scenario: Solution cannot be saved with a name exceeding 60 Characters
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	When I enter a name with more than '60' characters
	And I click Save expecting a Solution Name error
	Then The Solution Name Error should be 'Solution name exceeds maximum length of 60 characters'

Scenario: Solution cannot be saved with a Description exceeding 300 Characters
	Given I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	When I enter a description with more than '300' characters
	And I click Save expecting a Solution Description error
	Then The Solution Name Error should be 'Summary description exceeds maximum length of 300 characters'

Scenario: Solution cannot be added if it matches the name and version of an existing active solution
	Given I save the name and version of an existing solution
	And I click the `Add a solution` button
	And I click the `Start` link below the Basic Details section
	And I enter the existing solution name and version
	And I complete the Lead Contact Details section
	When I click Save expecting a Solution Name error
	Then The Solution Name Error should be 'Solution name and version already exists'