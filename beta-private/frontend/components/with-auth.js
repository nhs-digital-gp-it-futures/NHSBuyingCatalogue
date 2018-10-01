import React from 'react'

export default function withAuth (WrappedPage) {
  return class extends React.Component {
    static async getInitialProps ({ req }) {
      const defaults = WrappedPage.getInitialProps
        ? await WrappedPage.getInitialProps(...arguments)
        : {}

      if (req) {
        defaults.user = req.user
      } else {
        defaults.user = window.__NEXT_DATA__.props.pageProps.user
      }

      return defaults
    }

    render () {
      return <WrappedPage {...this.props} />
    }
  }
}

export function suppliersOnly (WrappedPage) {
  return withAuth(class extends React.Component {
    static async getInitialProps ({ req, res }) {
      const defaults = WrappedPage.getInitialProps
        ? await WrappedPage.getInitialProps(...arguments)
        : {}

      // only doing this on the server - not sure what'll happen on the client but I'd
      // assume that the initial authorised page load must succeed for this to be called on
      // the client, which is fine by me
      if (req) {
        if (!req.user || !req.user.is_supplier) {
          res.redirect('/')
          res.end()
        }
      }

      return defaults
    }

    render () {
      return <WrappedPage {...this.props} />
    }
  })
}
