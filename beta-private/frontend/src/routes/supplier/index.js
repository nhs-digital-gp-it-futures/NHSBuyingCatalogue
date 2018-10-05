const router = require('express').Router({ mergeParams: true })
const { SolutionsApi, Solutions } = require('catalogue-api')

const isSolutionLive = (soln) => soln.status === Solutions.StatusEnum.Approved
const isSolutionOnboarding = (soln) => soln.status !== Solutions.StatusEnum.Approved &&
  soln.status !== Solutions.StatusEnum.Failed

router.get('/', async (req, res) => {
  const context = {
    errors: [],
    solutions: {
      onboarding: [],
      live: []
    }
  }

  try {
    const api = new SolutionsApi()
    const paginatedSolutions = await api.apiSolutionsByOrganisationByOrganisationIdGet(
      req.user.org.id,
      { pageSize: 9999 }
    )
    context.solutions.onboarding = paginatedSolutions.items.filter(isSolutionOnboarding)
    context.solutions.live = paginatedSolutions.items.filter(isSolutionLive)
  } catch (err) {
    context.errors.push(err)
  }

  res.render('supplier/index', context)
})

module.exports = router
