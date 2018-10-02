const withSass = require('@zeit/next-sass')
module.exports = withSass({
  webpack: function (config, { isServer }) {
    if (!isServer) {
      config.plugins.push(
        new (require('webpack').IgnorePlugin)(/catalogue-api/)
      )
    }

    return config
  }
})
