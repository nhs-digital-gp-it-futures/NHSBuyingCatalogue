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
    this.something = 'foo'
  }
}

class SupplierDashboardPage extends Page {
  constructor () {
    super()

    this.addNewSolutionButton = Selector('#add-new-solution')
    this.firstOnboardingSolutionName = Selector(
      '#solutions-onboarding table > tbody > tr:first-child > td:first-child'
    )
    this.firstOnboardingSolutionStatus = Selector(
      '#solutions-onboarding table > tbody > tr:first-child > td:nth-child(4)'
    )
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
    this.capabilityAssessmentButton = Selector('#content a[href^="../../capabilities"]')
    this.capabilityAssessmentStatus = Selector('.onboarding-stages > :nth-child(2) .action')
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
