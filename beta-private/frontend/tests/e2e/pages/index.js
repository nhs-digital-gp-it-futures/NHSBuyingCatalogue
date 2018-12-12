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
  }
}

class OnboardingDashboardPage extends Page {
  constructor () {
    super()

    this.continueRegistrationButton = Selector('#content a[href^=register]')
  }
}

export const page = new Page()
export const supplierDashboardPage = new SupplierDashboardPage()
export const onboardingDashboardPage = new OnboardingDashboardPage()
