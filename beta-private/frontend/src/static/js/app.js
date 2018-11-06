/* global $, Modernizr, Document, Element */

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
// initial scroll position isn't set at that point.
// FIXME: does not work in IE or Edge - scrollingElement.scrollTop is always 0 here
window.onload = window.onhashchange = function () {
  const adjustmentElement = $('body > header')
  const adjustment = adjustmentElement.offsetHeight
  const scrollingElement = document.scrollingElement || document.documentElement
  scrollingElement.scrollTop = Math.max(0, scrollingElement.scrollTop - adjustment)
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
