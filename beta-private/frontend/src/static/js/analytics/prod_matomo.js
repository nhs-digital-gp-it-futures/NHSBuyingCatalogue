// require user consent before processing data
var _paq = window._paq || []

/* tracker methods like "setCustomDimension" should be called before "trackPageView" */
_paq.push(['requireConsent'])
_paq.push(['trackPageView'])
_paq.push(['enableLinkTracking'])
_paq.push(['trackVisibleContentImpressions']);

(function () {
  var u = 'https://analytics.buyingcatalogue.digital.nhs.uk/'
  _paq.push(['setTrackerUrl', u + 'matomo.php'])
  _paq.push(['setSiteId', '5'])

  var d = document
  var g = d.createElement('script')
  var s = d.getElementsByTagName('script')[0]

  g.type = 'text/javascript'
  g.async = true
  g.defer = true
  g.src = u + 'matomo.js'

  s.parentNode.insertBefore(g, s)
})()

window.addEventListener('load', function () {
  window.cookieconsent.initialise({
    'palette': {
      'popup': {
        'background': '#005EB8'
      },
      'button': {
        'background': '#fff',
        'text': '#005EB8'
      }
    },
    'content': {
      'header': 'Cookies used on the website!',
      'message': 'This site puts small files called cookies on your device. Some cookies are essential to the functioning of this site. Other cookies are optional but help us make this site better.',
      'dismiss': 'Got it!',
      'allow': 'Accept',
      'deny': 'Decline',
      'link': 'Privacy and cookie policy',
      'href': '/privacy',
      'close': '&#x274c;',
      'policy': 'Privacy and cookie Policy',
      'target': '_blank',
    },
    'type': 'opt-in',
    onInitialise: function (status) {
      var type = this.options.type
      var didConsent = this.hasConsented()
      if (type === 'opt-in' && didConsent) {
        // enable cookies
        _paq.push(['rememberConsentGiven', 24])
      }
    },

    onStatusChange: function (status, chosenBefore) {
      var type = this.options.type
      var didConsent = this.hasConsented()
      if (type === 'opt-in' && didConsent) {
        // enable cookies
        _paq.push(['rememberConsentGiven', 24])
      }
    },

    onRevokeChoice: function () {
      var type = this.options.type
      if (type === 'opt-in') {
        // disable cookies
        _paq.push(['forgetConsentGiven'])
      }
    }
  })
})
