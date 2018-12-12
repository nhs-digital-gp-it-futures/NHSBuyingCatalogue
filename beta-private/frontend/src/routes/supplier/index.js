const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const { dataProvider } = require('catalogue-data')

// work-around for Express bug 2281, open since 2014
// https://github.com/expressjs/express/issues/2281
function strictRouting (req, res, next) {
  if (req.originalUrl.slice(-1) === '/') return next()
  next('route')
}

router.get('/', strictRouting, async (req, res) => {
  const context = {
    errors: [],
    solutions: {
      onboarding: [],
      live: []
    },
    addUrl: 'solutions/new/#content'
  }

  try {
    context.solutions = await dataProvider.solutionsForSupplierDashboard(req.user.org.id, soln => ({
      ...soln,
      url: `solutions/${soln.id}/#content`
    }))

    context.solutions.onboarding = _.orderBy(context.solutions.onboarding, 'displayName')
    context.solutions.live = _.orderBy(context.solutions.live, 'displayName')
  } catch (err) {
    context.errors.push(err)
  }

  res.render('supplier/index', context)
})

router.use('/solutions/', require('./registration'))
router.use('/compliance/', require('./compliance'))
router.use('/capabilities/', require('./capabilities'))

module.exports = router
