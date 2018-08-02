// jQuery-esque shorthand for common element selection operations
window.$ = function $ (selector, el) {
  return (el || document).querySelector(selector)
}

window.$$ = function $$ (selector, el) {
  return Array.from((el || document).querySelectorAll(selector))
}
