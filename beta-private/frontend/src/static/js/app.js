/* global Document, Element */

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
