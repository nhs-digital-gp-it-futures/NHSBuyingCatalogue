import { Selector } from 'testcafe'

class Page {
  constructor () {
    this.baseUrl = 'http://localhost:3000'

    this.content = Selector('#content')
    this.homeLink = Selector('body > header a[aria-label=Home]')

    this.globalSolutionName = Selector('body > header .active-form .title')
    this.globalSaveButton = Selector('body > header .active-form [name="action\\[save\\]"]')
    this.globalSaveAndExitButton = Selector('body > header .active-form [name="action\\[exit\\]"]')

    this.continueButton = Selector('[name="action\\[continue\\]"]')
  }
}

class HomePage extends Page {
  constructor () {
    super()

    this.loginLink = Selector('#account a[href="/oidc/authenticate"]')

    this.welcomeBanner = Selector('#home-page .welcome-banner')

    this.cardContainer = Selector('#home-page .cards.container')

    this.stepsContainer = Selector('#home-page .steps.container')
  }
}

class SupplierDashboardPage extends Page {
  constructor () {
    super()

    this.addNewSolutionButton = Selector('#add-new-solution')

    // first solution
    this.firstOnboardingSolutionName = Selector(
      '#solutions-onboarding table > tbody > tr:first-child > td:first-child'
    )
    this.firstOnboardingSolutionStatus = Selector(
      '#solutions-onboarding table > tbody > tr:first-child > td:nth-child(4)'
    )

    // Second Solution
    this.secondSolutionName = Selector(
      '#solutions-onboarding table > tbody > tr:nth-child(2) > td:first-child'
    )
    this.secondSolutionStatus = Selector(
      '#solutions-onboarding table > tbody > tr:nth-child(2) > td:nth-child(4)'
    )

    // Last Solution
    this.lastOnboardingSolutionName = Selector(
      '#solutions-onboarding table > tbody > tr:last-child > td:first-child'
    )
    this.lastOnboardingSolutionStatus = Selector(
      '#solutions-onboarding table > tbody > tr:last-child > td:nth-child(4)'
    )
  }
}

class OnboardingDashboardPage extends Page {
  constructor () {
    super()

    this.continueRegistrationButton = Selector('#content a[href^=register]')
<<<<<<< HEAD
    this.capabilityAssessmentButton = Selector('#onboarding.dashboard.page a[href^="../../capabilities"]')
    this.capabilityAssessmentStatus = Selector('#onboarding.dashboard.page .onboarding-stages > :nth-child(2) .action')
=======
    this.capabilityAssessmentButton = Selector('#content a[href^="../../capabilities"]')
    this.capabilityAssessmentStatus = Selector('.onboarding-stages > :nth-child(2) .action')
    this.standardsComplianceButton = Selector('#content a[href^="../../compliance"]')
>>>>>>> starting some regression testing for standards compliance, on the dashboard and the evidence uploading
  }
}

class RegistrationPage extends Page {
  constructor () {
    super()

    this.solutionNameInput = Selector('#content [name="solution\\[name\\]"]')
    this.solutionDescriptionInput = Selector('#content [name="solution\\[description\\]"]')
    this.solutionVersionInput = Selector('#content [name="solution\\[version\\]"]')

    this.leadContactFieldset = Selector('.contact[data-contact-index="0"] > fieldset > legend')
    this.leadContactFirstNameInput = Selector('input#solution\\.contacts\\[0\\]\\.firstName')
    this.leadContactLastNameInput = Selector('input#solution\\.contacts\\[0\\]\\.lastName')
    this.leadContactEmailInput = Selector('input#solution\\.contacts\\[0\\]\\.emailAddress')
    this.leadContactPhoneInput = Selector('input#solution\\.contacts\\[0\\]\\.phoneNumber')

    this.addNewContactButton = Selector('#add-contact-button')
    this.newContactFieldset = Selector('.contact[data-contact-index="1"] > fieldset > legend')
    this.newContactContactTypeInput = Selector('input#solution\\.contacts\\[1\\]\\.contactType')
    this.newContactFirstNameInput = Selector('input#solution\\.contacts\\[1\\]\\.firstName')
    this.newContactLastNameInput = Selector('input#solution\\.contacts\\[1\\]\\.lastName')
    this.newContactEmailInput = Selector('input#solution\\.contacts\\[1\\]\\.emailAddress')
    this.newContactPhoneInput = Selector('input#solution\\.contacts\\[1\\]\\.phoneNumber')
  }
}

class StandardsComplianceDashboardPage extends Page {
  constructor () {
    super()
    this.breadcrumb = Selector('#content > .breadcrumb')
    this.associatedStandardsTable = Selector('#compliance > table:nth-child(4)')
    this.overarchingStandards = Selector('#compliance > table:nth-child(7)')
  }
}

class CapabilityEvidencePage extends Page {
  constructor () {
    super()
  }
}

export const page = new Page()
export const homePage = new HomePage()
export const supplierDashboardPage = new SupplierDashboardPage()
export const onboardingDashboardPage = new OnboardingDashboardPage()
export const registrationPage = new RegistrationPage()
export const capabilityEvidencePage = new CapabilityEvidencePage()
export const standardsComplianceDashboardPage = new StandardsComplianceDashboardPage()
