/* global fixture, test */

import { Selector } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { page, supplierDashboardPage, onboardingDashboardPage, registrationPage } from './pages'

fixture('Solution Registration - Add New Solution')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(async t => {
    return asSupplier(t)
      .click(supplierDashboardPage.homeLink)
  })
  .afterEach(axeCheck)

test('Creating a new solution leads to an empty form via a customised status page', async t => {
  await t
    .click(supplierDashboardPage.addNewSolutionButton)

    .expect(onboardingDashboardPage.continueRegistrationButton.textContent).eql('Start')

    .click(onboardingDashboardPage.continueRegistrationButton)
    .click(registrationPage.leadContactFieldset)

    .expect(page.globalSolutionName.exists).notOk()
    .expect(registrationPage.solutionNameInput.value).eql('')
    .expect(registrationPage.solutionDescriptionInput.value).eql('')
    .expect(registrationPage.solutionVersionInput.value).eql('')
    .expect(registrationPage.leadContactFirstNameInput.value).eql('')
    .expect(registrationPage.leadContactLastNameInput.value).eql('')
    .expect(registrationPage.leadContactEmailInput.value).eql('')
    .expect(registrationPage.leadContactPhoneInput.value).eql('')
})

test('Solution cannot have the same name and version as another existing solution', async t => {
  await t
    .click(supplierDashboardPage.addNewSolutionButton)
    .click(onboardingDashboardPage.continueRegistrationButton)

    .typeText(registrationPage.solutionNameInput, 'Really Kool Document Manager')
    .typeText(registrationPage.solutionDescriptionInput, 'This is the koolest kore system')
    .typeText(registrationPage.solutionVersionInput, '1')

    .click(registrationPage.leadContactFieldset)
    .typeText(registrationPage.leadContactFirstNameInput, 'Automated')
    .typeText(registrationPage.leadContactLastNameInput, 'Testing')
    .typeText(registrationPage.leadContactEmailInput, 'autotest@example.com')
    .typeText(registrationPage.leadContactPhoneInput, '123 456 78910')

    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.name').textContent).contains('Solution name and version already exists')

    .selectText(registrationPage.solutionNameInput)
    .typeText(registrationPage.solutionNameInput, 'Really Kool Kore System')

    .click(page.globalSaveButton)

    .expect(Selector('#errors').exists).notOk()
    .expect(page.globalSolutionName.textContent).eql('Really Kool Kore System, 1')
})

test('Newly created solution appears in the correct place on the supplier dashboard', async t => {
  await t
    .expect(supplierDashboardPage.secondSolutionName.textContent).eql('Really Kool Kore System | 1')
})
