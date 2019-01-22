/* global fixture, test */

import { asSupplier } from './roles'
import { supplierDashboardPage, onboardingDashboardPage, registrationPage, capabilityEvidencePage } from './pages'

fixture('Capability Assessment - First Access')
  .page(capabilityEvidencePage.baseUrl)
  .afterEach(supplierDashboardPage.checkAccessibility)

test('Unregistered solution does not allow access to capability assessment', async t => {
  await asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .expect(supplierDashboardPage.secondOnboardingSolutionStatus.textContent).eql('Draft')
    .click(supplierDashboardPage.secondOnboardingSolutionName)
    .expect(onboardingDashboardPage.capabilityAssessmentButton.exists).notOk()
})

test('Register Really Kool Kore System with 3 core capabilities', async t => {
  await asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.secondOnboardingSolutionName)
    .click(onboardingDashboardPage.continueRegistrationButton)
    .click(registrationPage.continueButton)

    .click('#capability-selector .capability[data-cap-id="CAP-C-001"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-C-001"]')
    .click('#capability-selector .capability[data-cap-id="CAP-C-002"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-C-002"]')
    .click('#capability-selector .capability[data-cap-id="CAP-C-003"]:not(.selected)')
    .click('[type=checkbox][data-id="CAP-C-003"]')

    .click(registrationPage.continueButton)
    .click(onboardingDashboardPage.homeLink)

    .expect(supplierDashboardPage.secondOnboardingSolutionStatus.textContent).eql('Registered')
})

test('Access button has correct text when no evidence submitted', async t => {
  await asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.secondOnboardingSolutionName)

    .expect(onboardingDashboardPage.capabilityAssessmentButton.textContent).eql('Start')
})
