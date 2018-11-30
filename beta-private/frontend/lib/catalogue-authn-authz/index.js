// set up OpenID Connect authentication
const passport = require('passport')
const { Issuer, Strategy } = require('openid-client')
const { dataProvider } = require('catalogue-data')

function authentication (app) {
  app.use(passport.initialize())
  app.use(passport.session())

  app.get('/oidc/authenticate', passport.authenticate('oidc'))
  app.get('/oidc/callback', passport.authenticate('oidc', { successRedirect: '/#account', failureRedirect: '/' }))

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
          redirect_uri: `${process.env.BASE_URL}/oidc/callback/`
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

function authenticatedOnly (req, res, next) {
  if (!req.user || !req.user.is_authenticated) {
    req.session.redirectTo = req.originalUrl
    passport.authenticate('oidc', { failureRedirect: '/' })(req, res, next)
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
