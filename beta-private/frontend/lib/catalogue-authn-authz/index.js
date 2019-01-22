// set up OpenID Connect authentication
const passport = require('passport')
const { Issuer, Strategy } = require('openid-client')
const { dataProvider } = require('catalogue-data')

const OIDC_CALLBACK_PATH = '/oidc/callback'

function authentication (app) {
  app.use(passport.initialize())
  app.use(passport.session())

  app.get('/oidc/authenticate', passport.authenticate('oidc'))
  app.get(OIDC_CALLBACK_PATH, authenticationHandler)

  Issuer.defaultHttpOptions = { timeout: 10000, retries: 3 }

  const makeIssuer = process.env.OIDC_ISSUER_URL
    ? Issuer.discover(process.env.OIDC_ISSUER_URL)
    : Promise.resolve(
        new Issuer({
          issuer: 'http://oidc-provider:9000',
          authorization_endpoint: 'http://localhost:9000/auth',
          token_endpoint: 'http://oidc-provider:9000/token',
          userinfo_endpoint: 'http://oidc-provider:9000/me',
          jwks_uri: 'http://oidc-provider:9000/certs',
          check_session_iframe: 'http://localhost:9000/session/check',
          end_session_endpoint: 'http://localhost:9000/session/end'
        })
      )

  return makeIssuer
    .then(issuer => {
      const client = new issuer.Client({
        client_id: process.env.OIDC_CLIENT_ID,
        client_secret: process.env.OIDC_CLIENT_SECRET
      })

      client.CLOCK_TOLERANCE = 60

      passport.use('oidc', new Strategy({
        client,
        params: {
          scope: 'openid email',
          redirect_uri: `${process.env.BASE_URL}${OIDC_CALLBACK_PATH}/`
        }
      }, authCallback))

      passport.serializeUser((user, done) => {
        done(null, user)
      })

      passport.deserializeUser((user, done) => {
        if (user.auth_header) {
          dataProvider.setAuthenticationToken(user.auth_header)
        }
        done(null, user)
      })

      app.get('/logout', (req, res) => {
        req.logout()

        if (!process.env.OIDC_ISSUER_URL) {
          const endSessionUrl = client.endSessionUrl()
          res.redirect(endSessionUrl)
        } else {
          res.redirect('/')
        }
      })
    })
    .catch(err => {
      console.error('OIDC initialisation failed:', err)
      process.exit(1)
    })
}

async function authCallback (tokenset, userinfo, done) {
  const authHeader = tokenset.access_token
  dataProvider.setAuthenticationToken(authHeader)

  if (!userinfo) {
    done(null, false)
    return
  }

  // load the contact corresponding with the email address
  // if there is no such contact, the user is still authenticated but the
  // lack of contact or organisation will prevent authorisation for
  // supplier-specific routes
  try {
    const { contact, org } = await dataProvider.contactByEmail(userinfo.email)
    const user = {
      ...userinfo,
      org,
      contact,
      first_name: contact.firstName,
      is_authenticated: true,
      auth_header: authHeader
    }
    done(null, user)
  } catch (err) {
    console.log('API Error', err)
    return done(null, {
      ...userinfo,
      first_name: userinfo.email,
      is_authenticated: true,
      auth_header: authHeader
    })
  }
}

function authenticationHandler (req, res, next) {
  // Preserve the originally requested page so that it can be returned to
  // once authentication is successful. Blacklist the OIDC callback page
  // as returning to that would cause an endless loop.
  if (!req.session.redirectTo && !req.originalUrl.startsWith(OIDC_CALLBACK_PATH)) {
    req.session.redirectTo = req.originalUrl
  }

  passport.authenticate('oidc', function (err, user, info) {
    // This is where the "did not find expected authorization request details
    // in session, req.session["oidc:oidc-provider"] is undefined" happens.
    // Leaving it unhandled for now to assess the extent to which this new code
    // fixes the reported issues.
    if (err) { return next(err) }

    // If no user resulted from authentication, send the user back through the process.
    if (!user) { return res.redirect('/oidc/authenticate') }

    // Log the user in and send them to their originally requested page.
    req.logIn(user, function (err) {
      if (err) { return next(err) }

      const redirectTo = req.session.redirectTo || '/'
      delete req.session.redirectTo
      return res.redirect(redirectTo)
    })
  })(req, res, next)
}

function authenticatedOnly (req, res, next) {
  if (!req.user || !req.user.is_authenticated) {
    authenticationHandler(req, res, next)
  } else next()
}

module.exports = {
  authentication,
  authorisation: {
    authenticatedOnly,
    suppliersOnly: [
      authenticatedOnly,
      (req, res, next) => {
        if (req.user && req.user.org && req.user.org.isSupplier) next()
        else res.redirect('/')
      }
    ],
    assessmentTeamOnly: [
      authenticatedOnly,
      (req, res, next) => {
        if (req.user && req.user.org && req.user.org.isNHSDigital) next()
        else res.redirect('/')
      }
    ]
  }
}
