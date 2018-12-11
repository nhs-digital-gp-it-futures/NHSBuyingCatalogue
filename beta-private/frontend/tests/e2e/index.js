/* global fixture, test */

const { Selector, Role } = require('testcafe')
const axeCheck = require('axe-testcafe')

const supplierRole = Role('http://localhost:3000/oidc/authenticate', async t => {
  await t
    .typeText('[name=email]', 'drkool@kool.com')
    .typeText('[name=password]', 'kool')
    .click('[name=submit]')
})

const homeLink = Selector('body > header a[aria-label=Home]')
const addNewSolutionButton = Selector('#add-new-solution')
const firstOnboardingSolutionName = Selector(
  '#solutions-onboarding table > tbody > tr:first-child > td:first-child'
)
const firstOnboardingSolutionStatus = Selector(
  '#solutions-onboarding table > tbody > tr:first-child > td:nth-child(4)'
)
const lastOnboardingSolutionName = Selector(
  '#solutions-onboarding table > tbody > tr:last-child > td:first-child'
)
const continueRegistrationButton = Selector('#content a[href^=register]')

const solutionNameInput = Selector('#content [name="solution\\[name\\]"]')
const solutionDescriptionInput = Selector('#content [name="solution\\[description\\]"]')
const solutionVersionInput = Selector('#content [name="solution\\[version\\]"]')

const globalSolutionName = Selector('body > header .active-form .title')
const globalSaveButton = Selector('body > header .active-form [name="action\\[save\\]"]')
const globalSaveAndExitButton = Selector('body > header .active-form [name="action\\[exit\\]"]')

const continueButton = Selector('[name="action\\[continue\\]"]')

const leadContactFieldset = Selector('.contact[data-contact-index="0"] > fieldset > legend')
const leadContactFirstNameInput = Selector('input#solution\\.contacts\\[0\\]\\.firstName')
const leadContactLastNameInput = Selector('input#solution\\.contacts\\[0\\]\\.lastName')
const leadContactEmailInput = Selector('input#solution\\.contacts\\[0\\]\\.emailAddress')
const leadContactPhoneInput = Selector('input#solution\\.contacts\\[0\\]\\.phoneNumber')

const addNewContactButton = Selector('#add-contact-button')
const newContactFieldset = Selector('.contact[data-contact-index="1"] > fieldset > legend')
const newContactContactTypeInput = Selector('input#solution\\.contacts\\[1\\]\\.contactType')
const newContactFirstNameInput = Selector('input#solution\\.contacts\\[1\\]\\.firstName')
const newContactLastNameInput = Selector('input#solution\\.contacts\\[1\\]\\.lastName')
const newContactEmailInput = Selector('input#solution\\.contacts\\[1\\]\\.emailAddress')
const newContactPhoneInput = Selector('input#solution\\.contacts\\[1\\]\\.phoneNumber')

fixture('Getting started')
  .page('http://localhost:3000')

test('a11y: logged out homepage', async t => {
  await axeCheck(t)
})

test('Login as supplier', async t => {
  await t
    .useRole(supplierRole)
    .expect(Selector('#account .user').innerText).contains('Hi, Dr')
})

test('a11y: supplier homepage', async t => {
  await t
    .useRole(supplierRole)
  await axeCheck(t)
})

test('Clicking logo returns to supplier homepage', async t => {
  await t
    .useRole(supplierRole)
    .click('body > header a[href^="/about"]')
    .click(homeLink)

    .expect(Selector('#content > h1').innerText).eql('My Solutions')
})

test('Solutions that are currently onboarding are listed', async t => {
  await t
    .useRole(supplierRole)
    .expect(firstOnboardingSolutionName.textContent).eql('Really Kool Document Manager | 1')
    .expect(firstOnboardingSolutionStatus.textContent).eql('Draft')
})

function navigateToSupplierOnboardingSolution (t) {
  return t
    .useRole(supplierRole)
    .click(homeLink)
    .click(firstOnboardingSolutionName)
    .click(continueRegistrationButton)
}

test('Registration page shows correct information accessibly', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(solutionNameInput.value).eql('Really Kool Document Manager')
    .expect(solutionDescriptionInput.value).eql('Does Really Kool document management')
    .expect(solutionVersionInput.value).eql('1')

  await axeCheck(t)
})

test('Registration page validation is correct and accessible', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .selectText(solutionNameInput).pressKey('backspace')
    .selectText(solutionDescriptionInput).pressKey('backspace')
    .click(continueButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Summary description is missing')
    .expect(solutionNameInput.parent('.control.invalid').child('.action').textContent).contains('Please enter a Solution name')
    .expect(solutionDescriptionInput.parent('.control.invalid').child('.action').textContent).contains('Please enter a Summary description')

  await axeCheck(t)
})

test('Solution name shows in top bar and updates correctly when saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(globalSolutionName.textContent).eql('Really Kool Document Manager, 1')

  await t
    .selectText(solutionNameInput).typeText(solutionNameInput, 'Really Really Kool Document Manager')
    .selectText(solutionVersionInput).pressKey('backspace')
    .click(globalSaveButton)
    .expect(solutionNameInput.value).eql('Really Really Kool Document Manager')
    .expect(solutionVersionInput.value).eql('')
    .expect(globalSolutionName.textContent).eql('Really Really Kool Document Manager')

  await t
    .selectText(solutionNameInput).typeText(solutionNameInput, 'Really Kool Document Manager')
    .selectText(solutionVersionInput).typeText(solutionVersionInput, '1')
    .click(globalSaveAndExitButton)
    .click(continueRegistrationButton)
    .expect(globalSolutionName.textContent).eql('Really Kool Document Manager, 1')
})

test('Global save buttons trigger validation and does not update top bar', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .selectText(solutionNameInput).pressKey('backspace')
    .selectText(solutionDescriptionInput).pressKey('backspace')
    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(globalSolutionName.textContent).eql('Really Kool Document Manager, 1')

  await t
    .click(globalSaveAndExitButton)
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Summary description is missing')
    .expect(globalSolutionName.textContent).eql('Really Kool Document Manager, 1')
})

test('Lead contact details can be changed and saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(leadContactFieldset)

    .expect(leadContactFirstNameInput.value).eql('Helpma')
    .expect(leadContactLastNameInput.value).eql('Boab')
    .expect(leadContactEmailInput.value).eql('helpma.boab@example.com')
    .expect(leadContactPhoneInput.value).eql('N/A')

  await t
    .selectText(leadContactFirstNameInput).typeText(leadContactFirstNameInput, 'Automated')
    .selectText(leadContactLastNameInput).typeText(leadContactLastNameInput, 'Testing')
    .selectText(leadContactEmailInput).typeText(leadContactEmailInput, 'autotest@example.com')
    .selectText(leadContactPhoneInput).typeText(leadContactPhoneInput, '123 456 78910')
    .click(globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(leadContactFieldset)

    .expect(leadContactFirstNameInput.value).eql('Automated')
    .expect(leadContactLastNameInput.value).eql('Testing')
    .expect(leadContactEmailInput.value).eql('autotest@example.com')
    .expect(leadContactPhoneInput.value).eql('123 456 78910')

  await t
    .selectText(leadContactFirstNameInput).typeText(leadContactFirstNameInput, 'Helpma')
    .selectText(leadContactLastNameInput).typeText(leadContactLastNameInput, 'Boab')
    .selectText(leadContactEmailInput).typeText(leadContactEmailInput, 'helpma.boab@example.com')
    .selectText(leadContactPhoneInput).typeText(leadContactPhoneInput, 'N/A')
    .click(globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(leadContactFieldset)

    .expect(leadContactFirstNameInput.value).eql('Helpma')
    .expect(leadContactLastNameInput.value).eql('Boab')
    .expect(leadContactEmailInput.value).eql('helpma.boab@example.com')
    .expect(leadContactPhoneInput.value).eql('N/A')
})

test('Blanking any and all Lead Contact fields triggers validation', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(leadContactFieldset)
    .selectText(leadContactFirstNameInput).pressKey('backspace')
    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.firstName').textContent).contains('Contact first name is missing')

  await t
    .selectText(leadContactLastNameInput).pressKey('backspace')
    .selectText(leadContactEmailInput).pressKey('backspace')
    .selectText(leadContactPhoneInput).pressKey('backspace')
    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.firstName').textContent).contains('Contact first name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.lastName').textContent).contains('Contact last name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.emailAddress').textContent).contains('Contact email is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[0\\]\\.phoneNumber').textContent).contains('Contact phone number is missing')
})

test('Creating a new contact requires all fields to be filled', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .expect(newContactFieldset.exists).notOk()

  await t
    .click(addNewContactButton)
    .typeText(newContactContactTypeInput, 'Clinical Safety Unicorn')
    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.firstName').textContent).contains('Contact first name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.lastName').textContent).contains('Contact last name is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.emailAddress').textContent).contains('Contact email is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.phoneNumber').textContent).contains('Contact phone number is missing')

  await t
    .typeText(newContactFirstNameInput, 'Zyra')
    .typeText(newContactLastNameInput, 'Smith Fotheringham-Shaw')
    .typeText(newContactEmailInput, 'safety.unicorn@example.com')
    .typeText(newContactPhoneInput, '555 123 4567')
    .click(globalSaveButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(newContactFieldset)

    .expect(newContactContactTypeInput.value).eql('Clinical Safety Unicorn')
    .expect(newContactFirstNameInput.value).eql('Zyra')
    .expect(newContactLastNameInput.value).eql('Smith Fotheringham-Shaw')
    .expect(newContactEmailInput.value).eql('safety.unicorn@example.com')
    .expect(newContactPhoneInput.value).eql('555 123 4567')
})

test('New contact details can be changed and saved', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(newContactFieldset)
    .selectText(newContactContactTypeInput).typeText(newContactContactTypeInput, 'Chief Safety Unicorn')
    .selectText(newContactEmailInput).typeText(newContactEmailInput, 'chief.safety.unicorn@example.com')
    .click(globalSaveAndExitButton)

  await navigateToSupplierOnboardingSolution(t)
    .click(newContactFieldset)

    .expect(newContactContactTypeInput.value).eql('Chief Safety Unicorn')
    .expect(newContactFirstNameInput.value).eql('Zyra')
    .expect(newContactLastNameInput.value).eql('Smith Fotheringham-Shaw')
    .expect(newContactEmailInput.value).eql('chief.safety.unicorn@example.com')
    .expect(newContactPhoneInput.value).eql('555 123 4567')
})

test('Blanking some optional contact fields triggers validation', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(newContactFieldset)
    .selectText(newContactContactTypeInput).pressKey('backspace')
    .selectText(newContactEmailInput).pressKey('backspace')
    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.contactType').textContent).contains('Contact type is missing')
    .expect(Selector('#errors #error-solution\\.contacts\\[1\\]\\.emailAddress').textContent).contains('Contact email is missing')
})

test('Blanking all optional contact fields removes the contact', async t => {
  await navigateToSupplierOnboardingSolution(t)
    .click(newContactFieldset)
    .selectText(newContactContactTypeInput).pressKey('backspace')
    .selectText(newContactFirstNameInput).pressKey('backspace')
    .selectText(newContactLastNameInput).pressKey('backspace')
    .selectText(newContactEmailInput).pressKey('backspace')
    .selectText(newContactPhoneInput).pressKey('backspace')
    .click(globalSaveButton)

    .expect(Selector('#errors').exists).notOk()
    .expect(newContactFieldset.exists).notOk()
})

test('Creating a new solution leads to an empty form via a customised status page', async t => {
  await t
    .useRole(supplierRole)
    .click(homeLink)
    .click(addNewSolutionButton)

    .expect(continueRegistrationButton.textContent).eql('Start')

    .click(continueRegistrationButton)
    .click(leadContactFieldset)

    .expect(globalSolutionName.exists).notOk()
    .expect(solutionNameInput.value).eql('')
    .expect(solutionDescriptionInput.value).eql('')
    .expect(solutionVersionInput.value).eql('')
    .expect(leadContactFirstNameInput.value).eql('')
    .expect(leadContactLastNameInput.value).eql('')
    .expect(leadContactEmailInput.value).eql('')
    .expect(leadContactPhoneInput.value).eql('')
})

test('Solution cannot have the same name and version as another existing solution', async t => {
  await t
    .useRole(supplierRole)
    .click(homeLink)
    .click(addNewSolutionButton)
    .click(continueRegistrationButton)

    .typeText(solutionNameInput, 'Really Kool Document Manager')
    .typeText(solutionDescriptionInput, 'This is the koolest kore system')
    .typeText(solutionVersionInput, '1')

    .click(leadContactFieldset)
    .typeText(leadContactFirstNameInput, 'Automated')
    .typeText(leadContactLastNameInput, 'Testing')
    .typeText(leadContactEmailInput, 'autotest@example.com')
    .typeText(leadContactPhoneInput, '123 456 78910')

    .click(globalSaveButton)

    .expect(Selector('#errors #error-solution\\.name').textContent).contains('Solution name and version already exists')

    .selectText(solutionNameInput)
    .typeText(solutionNameInput, 'Really Kool Kore System')

    .click(globalSaveButton)

    .expect(Selector('#errors').exists).notOk()
    .expect(globalSolutionName.textContent).eql('Really Kool Kore System, 1')
})

test('Newly created solution appears in the correct place on the supplier dashboard', async t => {
  await t
    .useRole(supplierRole)
    .click(homeLink)

    .expect(lastOnboardingSolutionName.textContent).eql('Really Kool Kore System | 1')
})

function navigateToSupplierOnboardingSolutionCapabilities (t) {
  return navigateToSupplierOnboardingSolution(t)
    .click(continueButton)
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

  await axeCheck(t)
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

    .click(continueButton)
    .expect(Selector('#errors #error-capabilities').innerText).contains('Select at least one capability to continue')
    .expect(selectedCapabilities.exists).notOk('No capabilities should be selected after reload')

  await axeCheck(t)
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

    .click(globalSaveButton)

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

    .click(globalSaveAndExitButton)

    .expect(Selector('#errors').exists).notOk()
})

test('Registering the solution changes the status and shows a confirmation message', async t => {
  await navigateToSupplierOnboardingSolutionCapabilities(t)
    .click(continueButton)
    .expect(Selector('#onboarding.dashboard.page .callout .title').textContent).eql('Solution registration complete.')
    .expect(Selector('.onboarding-stages :first-child.complete').exists).ok()

  await axeCheck(t)

  await t
    .click(homeLink)
    .expect(firstOnboardingSolutionStatus.textContent).eql('Registered')
})
