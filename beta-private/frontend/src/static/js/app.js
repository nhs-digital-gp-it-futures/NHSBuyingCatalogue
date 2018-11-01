/* global $, Document, Element */

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
window.onload = window.onhashchange = () => {
  const adjustmentElement = $('body > header')
  const adjustment = adjustmentElement.offsetHeight
  const scrollingElement = document.scrollingElement || document.documentElement
  scrollingElement.scrollTop = Math.max(0, scrollingElement.scrollTop - adjustment)
}
