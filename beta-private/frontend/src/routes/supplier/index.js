const router = require('express').Router({ mergeParams: true })

router.get('/', (req, res) => {
  res.render('supplier/index')
})

module.exports = router
