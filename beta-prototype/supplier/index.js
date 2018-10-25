require('dotenv').config()

const { PORT = 8000 } = process.env

const express = require('express')
const expresshbs = require('express-handlebars')

const app = express()

const session = require('express-session')
const os = require('os')
const path = require('path')

app.use(session({
  store: process.env.NODE_ENV === 'development'
         ? new (require('session-file-store')(session))({
           path: path.join(os.tmpdir(), 'buywolf', '.sessions')
         })
         : undefined,
  secret: process.env.SESSION_SECRET ||
            Math.floor(Math.random() * Number.MAX_SAFE_INTEGER).toString(36),
  resave: false,
  saveUninitialized: false
}))

app.use(require('body-parser').urlencoded({ extended: true }))

app.use(require('serve-static')('static', {
  index: false
}))

app.engine('html', expresshbs({
  defaultLayout: 'base.html',
  extname: '.html',
  partialsDir: 'views/partials/'
}))
app.set('view engine', 'html')

const { authentication, authorisation } = require('catalogue-authn-authz')

authentication(app).then(() => {
  app.get('/logout', (req, res) => {
    req.logout()
    res.redirect('/')
  })

  // middleware to set global template context variables
  app.use('*', (req, res, next) => {
    res.locals.user = req.user
    res.locals.title = 'Buying Catalogue Beta Prototype'
    res.locals.feedbackLink = `mailto:gpitfutures@nhs.net?${require('querystring').stringify({subject: `Feedback on Buying Catalogue Beta (${req.originalUrl})`})}`
    next()
  })

  app.get('/', (req, res) => {
    const redirectTo = req.session.redirectTo
    req.session.redirectTo = null

    if (redirectTo) {
      res.redirect(redirectTo)
    } else if (req.user && req.user.org && req.user.org.isSupplier) {
      res.redirect('/suppliers')
    } else if (req.user && req.user.org && req.user.org.isNHSDigital) {
      res.redirect('/assessment')
    } else {
      res.render('landing')
    }
  })

  app.use('/suppliers', authorisation.suppliersOnly, require('./routes/supplier'))
  app.use('/assessment', authorisation.assessmentTeamOnly, require('./routes/assessment'))

  app.listen(PORT)
})
