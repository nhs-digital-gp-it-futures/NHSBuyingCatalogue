/* global fixture, test */

import { Selector } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { page, supplierDashboardPage, onboardingDashboardPage, registrationPage } from './pages'

fixture('Getting started')
  .page(page.baseUrl)
  .afterEach(axeCheck)

test('a11y: logged out homepage', async t => {
})

test('Login as supplier', async t => {
  await asSupplier(t)
    .expect(Selector('#account .user').innerText).contains('Hi, Dr')
})

test('Clicking logo returns to supplier homepage', async t => {
  await asSupplier(t)
    .click('body > header a[href^="/about"]')
    .click(page.homeLink)

    .expect(Selector('#content > h1').innerText).eql('My Solutions')
})

test('Solutions that are currently onboarding are listed', async t => {
  await asSupplier(t)
    .expect(supplierDashboardPage.firstOnboardingSolutionName.textContent).eql('Really Kool Document Manager | 1')
    .expect(supplierDashboardPage.firstOnboardingSolutionStatus.textContent).eql('Draft')
})

function navigateToSupplierOnboardingSolution (t) {
  return asSupplier(t)
    .click(page.homeLink)
    .click(supplierDashboardPage.firstOnboardingSolutionName)
    .click(onboardingDashboardPage.continueRegistrationButton)
}

test('Registration page shows correct information accessibly', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(registrationPage.solutionNameInput.value).eql('Really Kool Document Manager')
    .expect(registrationPage.solutionDescriptionInput.value).eql('Does Really Kool document management')
    .expect(registrationPage.solutionVersionInput.value).eql('1')
})

test('Registration page validation is correct and accessible', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .selectText(registrationPage.solutionNameInput).pressKey('backspace')
    .selectText(registrationPage.solutionDescriptionInput).pressKey('backspace')
    .click(page.continueButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Summary description is missing')
    .expect(registrationPage.solutionNameInput.parent('.control.invalid').child('.action').textContent).contains('Please enter a Solution name')
    .expect(registrationPage.solutionDescriptionInput.parent('.control.invalid').child('.action').textContent).contains('Please enter a Summary description')
})

test('Solution name shows in top bar and updates correctly when saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(page.globalSolutionName.textContent).eql('Really Kool Document Manager, 1')

  await t
    .selectText(registrationPage.solutionNameInput)
    .typeText(registrationPage.solutionNameInput, 'Really Really Kool Document Manager')
    .selectText(registrationPage.solutionVersionInput).pressKey('backspace')
    .click(page.globalSaveButton)
    .expect(registrationPage.solutionNameInput.value).eql('Really Really Kool Document Manager')
    .expect(registrationPage.solutionVersionInput.value).eql('')
    .expect(page.globalSolutionName.textContent).eql('Really Really Kool Document Manager')

  await t
    .selectText(registrationPage.solutionNameInput)
    .typeText(registrationPage.solutionNameInput, 'Really Kool Document Manager')
    .selectText(registrationPage.solutionVersionInput)
    .typeText(registrationPage.solutionVersionInput, '1')
    .click(page.globalSaveAndExitButton)
    .click(onboardingDashboardPage.continueRegistrationButton)
    .expect(page.globalSolutionName.textContent).eql('Really Kool Document Manager, 1')
})

test('Global save buttons trigger validation and does not update top bar', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .selectText(registrationPage.solutionNameInput).pressKey('backspace')
    .selectText(registrationPage.solutionDescriptionInput).pressKey('backspace')
    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(page.globalSolutionName.textContent).eql('Really Kool Document Manager, 1')

  await t
    .click(page.globalSaveAndExitButton)
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Summary description is missing')
    .expect(page.globalSolutionName.textContent).eql('Really Kool Document Manager, 1')
})

test('Lead contact details can be changed and saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.leadContactFieldset)

    .expect(registrationPage.leadContactFirstNameInput.value).eql('Helpma')
    .expect(registrationPage.leadContactLastNameInput.value).eql('Boab')
    .expect(registrationPage.leadContactEmailInput.value).eql('helpma.boab@example.com')
    .expect(registrationPage.leadContactPhoneInput.value).eql('N/A')

  await t
    .selectText(registrationPage.leadContactFirstNameInput)
    .typeText(registrationPage.leadContactFirstNameInput, 'Automated')
    .selectText(registrationPage.leadContactLastNameInput)
    .typeText(registrationPage.leadContactLastNameInput, 'Testing')
    .selectText(registrationPage.leadContactEmailInput)
    .typeText(registrationPage.leadContactEmailInput, 'autotest@example.com')
    .selectText(registrationPage.leadContactPhoneInput)
    .typeText(registrationPage.leadContactPhoneInput, '123 456 78910')
    .click(page.globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.leadContactFieldset)

    .expect(registrationPage.leadContactFirstNameInput.value).eql('Automated')
    .expect(registrationPage.leadContactLastNameInput.value).eql('Testing')
    .expect(registrationPage.leadContactEmailInput.value).eql('autotest@example.com')
    .expect(registrationPage.leadContactPhoneInput.value).eql('123 456 78910')

  await t
    .selectText(registrationPage.leadContactFirstNameInput)
    .typeText(registrationPage.leadContactFirstNameInput, 'Helpma')
    .selectText(registrationPage.leadContactLastNameInput)
    .typeText(registrationPage.leadContactLastNameInput, 'Boab')
    .selectText(registrationPage.leadContactEmailInput)
    .typeText(registrationPage.leadContactEmailInput, 'helpma.boab@example.com')
    .selectText(registrationPage.leadContactPhoneInput)
    .typeText(registrationPage.leadContactPhoneInput, 'N/A')
    .click(page.globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.leadContactFieldset)

    .expect(registrationPage.leadContactFirstNameInput.value).eql('Helpma')
    .expect(registrationPage.leadContactLastNameInput.value).eql('Boab')
    .expect(registrationPage.leadContactEmailInput.value).eql('helpma.boab@example.com')
    .expect(registrationPage.leadContactPhoneInput.value).eql('N/A')
})

test('Blanking any and all Lead Contact fields triggers validation', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.leadContactFieldset)
    .selectText(registrationPage.leadContactFirstNameInput).pressKey('backspace')
    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.firstName').textContent).contains('Contact first name is missing')

  await t
    .selectText(registrationPage.leadContactLastNameInput).pressKey('backspace')
    .selectText(registrationPage.leadContactEmailInput).pressKey('backspace')
    .selectText(registrationPage.leadContactPhoneInput).pressKey('backspace')
    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.firstName').textContent).contains('Contact first name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.lastName').textContent).contains('Contact last name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.emailAddress').textContent).contains('Contact email is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.phoneNumber').textContent).contains('Contact phone number is missing')
})

test('Creating a new contact requires all fields to be filled', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(registrationPage.newContactFieldset.exists).notOk()

  await t
    .click(registrationPage.addNewContactButton)
    .typeText(registrationPage.newContactContactTypeInput, 'Clinical Safety Unicorn')
    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.firstName').textContent).contains('Contact first name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.lastName').textContent).contains('Contact last name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.emailAddress').textContent).contains('Contact email is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.phoneNumber').textContent).contains('Contact phone number is missing')

  await t
    .typeText(registrationPage.newContactFirstNameInput, 'Zyra')
    .typeText(registrationPage.newContactLastNameInput, 'Smith Fotheringham-Shaw')
    .typeText(registrationPage.newContactEmailInput, 'safety.unicorn@example.com')
    .typeText(registrationPage.newContactPhoneInput, '555 123 4567')
    .click(page.globalSaveButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.newContactFieldset)

    .expect(registrationPage.newContactContactTypeInput.value).eql('Clinical Safety Unicorn')
    .expect(registrationPage.newContactFirstNameInput.value).eql('Zyra')
    .expect(registrationPage.newContactLastNameInput.value).eql('Smith Fotheringham-Shaw')
    .expect(registrationPage.newContactEmailInput.value).eql('safety.unicorn@example.com')
    .expect(registrationPage.newContactPhoneInput.value).eql('555 123 4567')
})

test('New contact details can be changed and saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.newContactFieldset)
    .selectText(registrationPage.newContactContactTypeInput)
    .typeText(registrationPage.newContactContactTypeInput, 'Chief Safety Unicorn')
    .selectText(registrationPage.newContactEmailInput)
    .typeText(registrationPage.newContactEmailInput, 'chief.safety.unicorn@example.com')
    .click(page.globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.newContactFieldset)

    .expect(registrationPage.newContactContactTypeInput.value).eql('Chief Safety Unicorn')
    .expect(registrationPage.newContactFirstNameInput.value).eql('Zyra')
    .expect(registrationPage.newContactLastNameInput.value).eql('Smith Fotheringham-Shaw')
    .expect(registrationPage.newContactEmailInput.value).eql('chief.safety.unicorn@example.com')
    .expect(registrationPage.newContactPhoneInput.value).eql('555 123 4567')
})

test('Blanking some optional contact fields triggers validation', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.newContactFieldset)
    .selectText(registrationPage.newContactContactTypeInput).pressKey('backspace')
    .selectText(registrationPage.newContactEmailInput).pressKey('backspace')
    .click(page.globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.contactType').textContent).contains('Contact type is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.emailAddress').textContent).contains('Contact email is missing')
})

test('Blanking all optional contact fields removes the contact', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(registrationPage.newContactFieldset)
    .selectText(registrationPage.newContactContactTypeInput).pressKey('backspace')
    .selectText(registrationPage.newContactFirstNameInput).pressKey('backspace')
    .selectText(registrationPage.newContactLastNameInput).pressKey('backspace')
    .selectText(registrationPage.newContactEmailInput).pressKey('backspace')
    .selectText(registrationPage.newContactPhoneInput).pressKey('backspace')
    .click(page.globalSaveButton)

    .expect(Selector('#errors').exists).notOk()
    .expect(registrationPage.newContactFieldset.exists).notOk()
})

test('Creating a new solution leads to an empty form via a customised status page', async t => {
  await asSupplier(t)
    .click(page.homeLink)
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
  await asSupplier(t)
    .click(page.homeLink)
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
  await asSupplier(t)
    .click(page.homeLink)

    .expect(supplierDashboardPage.lastOnboardingSolutionName.textContent).eql('Really Kool Kore System | 1')
})

function navigateToSupplierOnboardingSolutionCapabilities (t) {
  return navigateToSupplierOnboardingSolution(t)
    .click(page.continueButton)
}

test('Capabilities page shows correct information accessibly', async t => {
  const allCoreCapNames = Selector('#capabilities-core .capability .name')
  const allNonCoreCapNames = Selector('#capabilities-non-core .capability .name')

  await navigateToSupplierOnboardingSolutionCapabilities(t)

    .expect(Selector('#capabilities-core .capability').count).eql(6)
    .expect(Selector('#capabilities-non-core .capability').count).eql(31)

    .expect(allCoreCapNames.nth(0).innerText).eql('Appointments Management - GP')
    .expect(allCoreCapNames.nth(3).innerText).eql('Patient Information Maintenance')
    .expect(allCoreCapNames.nth(5).innerText).eql('Recording Consultations')

    .expect(allNonCoreCapNames.nth(0).innerText).eql('Appointments Management - Citizen')
    .expect(allNonCoreCapNames.nth(13).innerText).eql('e-Consultations (Patient/Service User to Professional)')
    .expect(allNonCoreCapNames.nth(30).innerText).eql('Workflow')

    .expect(Selector('#capability-selector .capability[data-cap-id="CAP-C-004"].selected')).ok()
})

test('Capabilities page validation is correct and accessible', async t => {
  await navigateToSupplierOnboardingSolutionCapabilities(t)

  const selectedCapabilities = Selector('#capability-selector .capability.selected')
  const selectedCapabilitiesCount = await selectedCapabilities.count
  for (let i = 0; i < selectedCapabilitiesCount; i++) {
    await t
      .click(selectedCapabilities.nth(i))
      .click(selectedCapabilities.nth(i).find('input:checked'))
  }

  await t
    .expect(Selector('#capability-selector .capability.revealed').exists).notOk()

    .click(page.continueButton)
    .expect(Selector('#errors #error-capabilities').innerText).contains('Select at least one capability to continue')
    .expect(selectedCapabilities.exists).notOk('No capabilities should be selected after reload')
})

test('Capabilities can be changed, summary updates and data save correctly', async t => {
  const capabilityCount = Selector('#capability-summary .capability-count').innerText
  const standardCount = Selector('#capability-summary .standard-count').innerText
  const selSelectedCapabilities = Selector('#capability-summary .capabilities [data-id].selected')
  const selSelectedStandards = Selector('#capability-summary .standards [data-id].selected')

  await navigateToSupplierOnboardingSolutionCapabilities(t)

    .click('#capability-selector .capability[data-cap-id="CAP-C-004"].selected')
    .click('[type=checkbox][data-id="CAP-C-004"]')

    .expect(capabilityCount).eql('0 Capabilities selected')
    .expect(standardCount).eql('11 Standards will be required')
    .expect(Selector('#capability-summary .standards .associated').visible).notOk()

    .click('#capability-selector .capability[data-cap-id="CAP-C-001"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-C-001"]')

    .click('#capability-selector .capability[data-cap-id="CAP-N-037"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-N-037"]')

    .click('#capability-selector .capability[data-cap-id="CAP-N-020"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-N-020"]')

    .expect(capabilityCount).eql('3 Capabilities selected')
    .expect(standardCount).eql('14 Standards will be required')
    .expect(Selector('#capability-summary .standards .associated').visible).ok()

    .expect(selSelectedCapabilities.nth(0).textContent).eql('Appointments Management - GP')
    .expect(selSelectedCapabilities.nth(1).textContent).eql('Unified Care Record')
    .expect(selSelectedCapabilities.nth(2).textContent).eql('Workflow')

    .expect(selSelectedStandards.nth(0).textContent).eql('Appointments Management - GP - Standard')
    .expect(selSelectedStandards.nth(1).textContent).eql('General Practice Appointments Data Reporting')
    .expect(selSelectedStandards.nth(2).textContent).eql('Workflow - Standard')
    .expect(selSelectedStandards.nth(4).textContent).eql('Clinical Safety')
    .expect(selSelectedStandards.nth(13).textContent).eql('Service Management')

    .click(page.globalSaveButton)

    .expect(Selector('#errors').exists).notOk()
    .expect(capabilityCount).eql('3 Capabilities selected')
    .expect(standardCount).eql('14 Standards will be required')

    .click('#capability-selector .capability[data-cap-id="CAP-C-001"].selected')
    .click('[type=checkbox][data-id="CAP-C-001"]')

    .click('#capability-selector .capability[data-cap-id="CAP-N-037"].selected')
    .click('[type=checkbox][data-id="CAP-N-037"]')

    .click('#capability-selector .capability[data-cap-id="CAP-N-020"].selected')
    .click('[type=checkbox][data-id="CAP-N-020"]')

    .expect(capabilityCount).eql('0 Capabilities selected')
    .expect(standardCount).eql('11 Standards will be required')
    .expect(Selector('#capability-summary .standards .associated').visible).notOk()

    .click('#capability-selector .capability[data-cap-id="CAP-C-004"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-C-004"]')

    .expect(capabilityCount).eql('1 Capability selected')
    .expect(standardCount).eql('14 Standards will be required')
    .expect(Selector('#capability-summary .standards .associated').visible).ok()

    .click(page.globalSaveAndExitButton)

    .expect(Selector('#errors').exists).notOk()
})

test('Registering the solution shows a confirmation message', async t => {
  await navigateToSupplierOnboardingSolutionCapabilities(t)
    .click(page.continueButton)
    .expect(Selector('#onboarding.dashboard.page .callout .title').textContent).eql('Solution registration complete.')
    .expect(Selector('.onboarding-stages :first-child.complete').exists).ok()
})

test('Registered solution shows status change on dashboard', async t => {
  await asSupplier(t)
    .click(page.homeLink)
    .expect(supplierDashboardPage.firstOnboardingSolutionStatus.textContent).eql('Registered')
})
