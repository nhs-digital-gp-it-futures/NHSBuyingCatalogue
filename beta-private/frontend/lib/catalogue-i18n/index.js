const i18next = require('i18next')
const registerI18nHelper = require('handlebars-i18next').default
const path = require('path')

module.exports = function (Handlebars, localesPath) {
  i18next
    .use(require('i18next-node-fs-backend'))
    .init({
      backend: {
        loadPath: path.join(localesPath, '{{lng}}/{{ns}}.json')
      },
      lng: 'en',
      fallbackLng: false,
      debug: 'DEBUG_I18N' in process.env,
      ns: ['common', 'pages', 'validation'],
      defaultNS: 'pages',
      fallbackNS: ['common', 'validation']
    }, function (err, t) {
      if (err) throw new Error(err)
      module.exports.t = t
      registerI18nHelper(Handlebars, i18next, 't')
    })
}
