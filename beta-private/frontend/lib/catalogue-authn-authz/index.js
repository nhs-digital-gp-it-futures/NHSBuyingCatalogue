// set up OpenID Connect authentication
const passport = require('passport')
const { Issuer, Strategy } = require('openid-client')
//const api = require('catalogue-api')
const api = {
  set_authorisation(header) {
  },

  async get_org_for_user(user) {
    return {
      org: {},
      contact: {
        firstName: 'Yay!'
      }
    }
  }
}

function authentication (app) {
  app.use(passport.initialize())
  app.use(passport.session())

  app.get('/oidc/authenticate', passport.authenticate('oidc'))
  app.get('/oidc/callback', passport.authenticate('oidc', { successRedirect: '/#account', failureRedirect: '/' }))

  const makeIssuer = process.env.OIDC_ISSUER_URL
    ? Issuer.discover(process.env.OIDC_ISSUER_URL)
    : Promise.resolve(
        new Issuer({
          issuer: 'http://oidc-provider:9000',
          authorization_endpoint: 'http://localhost:9000/auth',
          token_endpoint: 'http://oidc-provider:9000/token',
          userinfo_endpoint: 'http://oidc-provider:9000/me',
          jwks_uri: 'http://oidc-provider:9000/certs'
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
          api.set_authorisation(user.auth_header)
        }
        done(null, user)
      })
    })
    .catch(err => {
      console.error('OIDC initialisation failed:', err)
      process.exit(1)
    })
}

function authCallback (tokenset, userinfo, done) {
  const authHeader = `Bearer ${tokenset.access_token}`
  api.set_authorisation(authHeader)

  if (!userinfo) {
    done(null, false)
    return
  }

  // load the contact corresponding with the email address
  // if there is no such contact, the user is still authenticated but the
  // lack of contact or organisation will prevent authorisation for
  // supplier-specific routes
  api.get_org_for_user(userinfo)
    .then(({ org, contact }) => {
      const user = {
        ...userinfo,
        org,
        contact,
        first_name: contact.firstName,
        is_authenticated: true,
        auth_header: authHeader
      }
      done(null, user)
    })
    .catch(() => done(null, {
      ...userinfo,
      is_authenticated: true,
      auth_header: authHeader
    }))
}

function authenticatedOnly (req, res, next) {
  if (!req.user || !req.user.is_authenticated) {
    req.session.redirectTo = req.originalUrl
    passport.authenticate('oidc', {failureRedirect: '/'})(req, res, next)
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
