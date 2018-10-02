const express = require('express')
const next = require('next')

const port = parseInt(process.env.PORT, 10) || 8000
const dev = process.env.NODE_ENV !== 'production'
const app = next({ dev })
const handle = app.getRequestHandler()

const server = express()

server.use(require('express-session')({
  secret: Math.floor(Math.random() * Number.MAX_SAFE_INTEGER).toString(36),
  resave: false,
  saveUninitialized: false
}))

const { authentication } = require('catalogue-authn-authz')

app.prepare()
  .then(() => authentication(server))
  .then(() => {
    server.get('/logout', (req, res) => {
      req.logout()
      res.redirect('/#account')
    })

    server.get('*', (req, res) => {
      return handle(req, res)
    })

    server.listen(port, (err) => {
      if (err) throw err
      console.log(`> Ready on http://localhost:${port}`)
    })
  })
