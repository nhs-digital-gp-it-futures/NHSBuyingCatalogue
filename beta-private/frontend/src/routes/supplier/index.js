const router = require('express').Router({ mergeParams: true })
const { dataProvider } = require('catalogue-data')

router.get('/', async (req, res) => {
  const context = {
    errors: [],
    solutions: {
      onboarding: [],
      live: []
    },
    addUrl: `${req.baseUrl}/solutions/new`
  }

  try {
    context.solutions = await dataProvider.solutionsForSupplierDashboard(req.user.org.id, soln => ({
      ...soln,
      url: `${req.baseUrl}/solutions/${soln.id}`
    }))
  } catch (err) {
    context.errors.push(err)
  }

  res.render('supplier/index', context)
})

module.exports = router
