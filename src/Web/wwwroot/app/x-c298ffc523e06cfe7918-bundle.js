(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["vendor.timers-browserify"],{

/***/ "URgk":
/*!************************************************!*\
  !*** ./node_modules/timers-browserify/main.js ***!
  \************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("/* WEBPACK VAR INJECTION */(function(global) {var scope = typeof global !== \"undefined\" && global || typeof self !== \"undefined\" && self || window;\nvar apply = Function.prototype.apply; // DOM APIs, for completeness\n\nexports.setTimeout = function () {\n  return new Timeout(apply.call(setTimeout, scope, arguments), clearTimeout);\n};\n\nexports.setInterval = function () {\n  return new Timeout(apply.call(setInterval, scope, arguments), clearInterval);\n};\n\nexports.clearTimeout = exports.clearInterval = function (timeout) {\n  if (timeout) {\n    timeout.close();\n  }\n};\n\nfunction Timeout(id, clearFn) {\n  this._id = id;\n  this._clearFn = clearFn;\n}\n\nTimeout.prototype.unref = Timeout.prototype.ref = function () {};\n\nTimeout.prototype.close = function () {\n  this._clearFn.call(scope, this._id);\n}; // Does not start the time, just sets up the members needed.\n\n\nexports.enroll = function (item, msecs) {\n  clearTimeout(item._idleTimeoutId);\n  item._idleTimeout = msecs;\n};\n\nexports.unenroll = function (item) {\n  clearTimeout(item._idleTimeoutId);\n  item._idleTimeout = -1;\n};\n\nexports._unrefActive = exports.active = function (item) {\n  clearTimeout(item._idleTimeoutId);\n  var msecs = item._idleTimeout;\n\n  if (msecs >= 0) {\n    item._idleTimeoutId = setTimeout(function onTimeout() {\n      if (item._onTimeout) item._onTimeout();\n    }, msecs);\n  }\n}; // setimmediate attaches itself to the global object\n\n\n__webpack_require__(/*! setimmediate */ \"YBdB\"); // On some exotic environments, it's not clear which object `setimmediate` was\n// able to install onto.  Search each possibility in the same order as the\n// `setimmediate` library.\n\n\nexports.setImmediate = typeof self !== \"undefined\" && self.setImmediate || typeof global !== \"undefined\" && global.setImmediate || this && this.setImmediate;\nexports.clearImmediate = typeof self !== \"undefined\" && self.clearImmediate || typeof global !== \"undefined\" && global.clearImmediate || this && this.clearImmediate;\n/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(/*! ./../webpack/buildin/global.js */ \"yLpj\")))\n\n//# sourceURL=webpack:///./node_modules/timers-browserify/main.js?");

/***/ })

}]);