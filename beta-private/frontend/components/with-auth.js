import React from 'react'

export default function withAuth (WrappedPage) {
  return class extends React.Component {
    static async getInitialProps ({req}) {
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
