/* global $, $$, Modernizr, Document, Element */

// jQuery-esque shorthand for common element selection operations
window.$ = function $ (selector, el) {
  return (el || document).$(selector)
}

window.$$ = function $$ (selector, el) {
  return (el || document).$$(selector)
}

Document.prototype.$ = Element.prototype.$ = function (selector) {
  return this.querySelector(selector)
}

Document.prototype.$$ = Element.prototype.$$ = function (selector) {
  return Array.from(this.querySelectorAll(selector))
}

// Compensate for fixed page header height causing anchored links to appear
// off-screen on page load. Note that this can't be DOMContentLoaded as the
// initial scroll position isn't set at that point. Nor is it set during the
// load event on IE and Edge. So, the actual compensatory scroll is deferred
// for a brief period to allow all browsers to settle on the required state.
window.onload = window.onhashchange = function () {
  const adjustmentElement = $('body > header')
  const adjustment = adjustmentElement.offsetHeight
  const scrollingElement = document.scrollingElement || document.documentElement

  setTimeout(function () {
    scrollingElement.scrollTop = Math.max(0, scrollingElement.scrollTop - adjustment)
  }, 25)
}

// Simulate support for the form attribute on inputs for IE11
if (!Modernizr.formattribute) {
  document.addEventListener('DOMContentLoaded', function () {
    $('body > header').addEventListener('click', function (ev) {
      if (ev.target.tagName === 'INPUT' && ev.target.hasAttribute('form')) {
        const form = document.getElementById(ev.target.getAttribute('form'))
        if (form) {
          ev.preventDefault()

          // append a hidden input with the name and value of the clicked button to
          // the form, then ask it to submit
          const input = document.createElement('input')
          input.type = 'hidden'
          input.name = ev.target.name
          input.value = ev.target.value

          form.appendChild(input)
          form.submit()
        }
      }
    })
  })
}

document.addEventListener('DOMContentLoaded', function () {
  // collapse all but the first collapsible fieldsets by default, except ones that contain
  // invalid fields
  function collapseAllFieldsetsExcept (elCurrent) {
    $$('fieldset.collapsible').forEach(function (el) {
      if (el !== elCurrent && !el.classList.contains('invalid')) {
        el.classList.add('collapsed')
      }
    })
  }

  // HACK: improperly moved code broke the script at the original position, expose the
  //       missing function
  window.collapseAllFieldsetsExcept = collapseAllFieldsetsExcept

  collapseAllFieldsetsExcept($('fieldset.collapsible:first-of-type'))

  function handleCollapsibleFieldset (ev) {
    if (ev.target.tagName === 'LEGEND') {
      const elFieldset = ev.target.parentNode
      if (elFieldset.classList.contains('collapsible')) {
        ev.preventDefault()

        elFieldset.classList.toggle('collapsed')
        if (!elFieldset.classList.contains('collapsed')) {
          collapseAllFieldsetsExcept(elFieldset)
        }

        return true
      }
    }
  }

  // support both mouse and keyboard access
  $('#content').addEventListener('click', handleCollapsibleFieldset)
  $('#content').addEventListener('keypress', function (ev) {
    if (ev.key === ' ' || ev.key === 'Enter') {
      handleCollapsibleFieldset(ev)
    }
  })

  // character counts on inputs with a maxlength and associated count display element
  $$('.control input[maxlength], .control textarea[maxlength]').forEach(function (elInput) {
    const elCount = elInput.parentElement.$('.character-count')
    if (!elCount) return

    const maxLength = +elInput.maxLength
    if (!maxLength || elInput.readOnly) {
      elCount.parentNode.removeChild(elCount)
      return
    }

    function refresh () {
      const remaining = maxLength - elInput.value.length
      const isInvalid = remaining < 0

      elCount.classList.toggle('invalid', isInvalid)

      if (isInvalid) {
        elCount.textContent = 'You have ' + -remaining + ' characters too many (out of ' + maxLength + ').'
      } else {
        elCount.textContent = 'You have ' + remaining + ' (out of ' + maxLength + ') characters remaining.'
      }
    }

    // remove the physical maxLength restriction
    elInput.removeAttribute('maxlength')
    refresh()
    elInput.addEventListener('input', refresh)
  })
})
