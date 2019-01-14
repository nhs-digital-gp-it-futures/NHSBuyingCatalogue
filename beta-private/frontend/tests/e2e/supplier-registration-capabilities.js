/* global fixture, test */

import { Selector } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { page, supplierDashboardPage, onboardingDashboardPage, registrationPage } from './pages'

fixture('Solution Registration - Capabilities')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(navigateToSupplierOnboardingSolutionCapabilities)
  .afterEach(axeCheck)

function navigateToSupplierOnboardingSolutionCapabilities (t) {
  return asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.firstOnboardingSolutionName)
    .click(onboardingDashboardPage.continueRegistrationButton)
    .click(registrationPage.continueButton)
    .expect(Selector('#capability-selector').exists).ok()
}

test('Capabilities page shows correct information accessibly', async t => {
  const allCoreCapNames = Selector('#capabilities-core .capability .name')
  const allNonCoreCapNames = Selector('#capabilities-non-core .capability .name')

  await t

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

  await t

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

test('Registering the solution shows a confirmation message and status change on dashboard', async t => {
  await t
    .click(page.continueButton)
    .expect(Selector('#onboarding.dashboard.page .callout .title').textContent).eql('Solution registration complete.')
    .expect(Selector('.onboarding-stages :first-child.complete').exists).ok()

    .click(page.homeLink)
    .expect(supplierDashboardPage.firstOnboardingSolutionStatus.textContent).eql('Registered')
})

test('A registered solution cannot have its name edited', async t => {
  await t
    .click('#content a.back-link')
    .expect(Selector('#content [readonly]').count).eql(1)
    .expect(registrationPage.solutionNameInput.hasAttribute('readonly')).ok()
    .expect(registrationPage.solutionNameCounter.exists).notOk()
})
