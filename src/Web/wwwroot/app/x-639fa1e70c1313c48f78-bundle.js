(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["vendor.setimmediate"],{

/***/ "YBdB":
/*!***************************************************!*\
  !*** ./node_modules/setimmediate/setImmediate.js ***!
  \***************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

eval("/* WEBPACK VAR INJECTION */(function(global, process) {(function (global, undefined) {\n  \"use strict\";\n\n  if (global.setImmediate) {\n    return;\n  }\n\n  var nextHandle = 1; // Spec says greater than zero\n\n  var tasksByHandle = {};\n  var currentlyRunningATask = false;\n  var doc = global.document;\n  var registerImmediate;\n\n  function setImmediate(callback) {\n    // Callback can either be a function or a string\n    if (typeof callback !== \"function\") {\n      callback = new Function(\"\" + callback);\n    } // Copy function arguments\n\n\n    var args = new Array(arguments.length - 1);\n\n    for (var i = 0; i < args.length; i++) {\n      args[i] = arguments[i + 1];\n    } // Store and register the task\n\n\n    var task = {\n      callback: callback,\n      args: args\n    };\n    tasksByHandle[nextHandle] = task;\n    registerImmediate(nextHandle);\n    return nextHandle++;\n  }\n\n  function clearImmediate(handle) {\n    delete tasksByHandle[handle];\n  }\n\n  function run(task) {\n    var callback = task.callback;\n    var args = task.args;\n\n    switch (args.length) {\n      case 0:\n        callback();\n        break;\n\n      case 1:\n        callback(args[0]);\n        break;\n\n      case 2:\n        callback(args[0], args[1]);\n        break;\n\n      case 3:\n        callback(args[0], args[1], args[2]);\n        break;\n\n      default:\n        callback.apply(undefined, args);\n        break;\n    }\n  }\n\n  function runIfPresent(handle) {\n    // From the spec: \"Wait until any invocations of this algorithm started before this one have completed.\"\n    // So if we're currently running a task, we'll need to delay this invocation.\n    if (currentlyRunningATask) {\n      // Delay by doing a setTimeout. setImmediate was tried instead, but in Firefox 7 it generated a\n      // \"too much recursion\" error.\n      setTimeout(runIfPresent, 0, handle);\n    } else {\n      var task = tasksByHandle[handle];\n\n      if (task) {\n        currentlyRunningATask = true;\n\n        try {\n          run(task);\n        } finally {\n          clearImmediate(handle);\n          currentlyRunningATask = false;\n        }\n      }\n    }\n  }\n\n  function installNextTickImplementation() {\n    registerImmediate = function (handle) {\n      process.nextTick(function () {\n        runIfPresent(handle);\n      });\n    };\n  }\n\n  function canUsePostMessage() {\n    // The test against `importScripts` prevents this implementation from being installed inside a web worker,\n    // where `global.postMessage` means something completely different and can't be used for this purpose.\n    if (global.postMessage && !global.importScripts) {\n      var postMessageIsAsynchronous = true;\n      var oldOnMessage = global.onmessage;\n\n      global.onmessage = function () {\n        postMessageIsAsynchronous = false;\n      };\n\n      global.postMessage(\"\", \"*\");\n      global.onmessage = oldOnMessage;\n      return postMessageIsAsynchronous;\n    }\n  }\n\n  function installPostMessageImplementation() {\n    // Installs an event handler on `global` for the `message` event: see\n    // * https://developer.mozilla.org/en/DOM/window.postMessage\n    // * http://www.whatwg.org/specs/web-apps/current-work/multipage/comms.html#crossDocumentMessages\n    var messagePrefix = \"setImmediate$\" + Math.random() + \"$\";\n\n    var onGlobalMessage = function (event) {\n      if (event.source === global && typeof event.data === \"string\" && event.data.indexOf(messagePrefix) === 0) {\n        runIfPresent(+event.data.slice(messagePrefix.length));\n      }\n    };\n\n    if (global.addEventListener) {\n      global.addEventListener(\"message\", onGlobalMessage, false);\n    } else {\n      global.attachEvent(\"onmessage\", onGlobalMessage);\n    }\n\n    registerImmediate = function (handle) {\n      global.postMessage(messagePrefix + handle, \"*\");\n    };\n  }\n\n  function installMessageChannelImplementation() {\n    var channel = new MessageChannel();\n\n    channel.port1.onmessage = function (event) {\n      var handle = event.data;\n      runIfPresent(handle);\n    };\n\n    registerImmediate = function (handle) {\n      channel.port2.postMessage(handle);\n    };\n  }\n\n  function installReadyStateChangeImplementation() {\n    var html = doc.documentElement;\n\n    registerImmediate = function (handle) {\n      // Create a <script> element; its readystatechange event will be fired asynchronously once it is inserted\n      // into the document. Do so, thus queuing up the task. Remember to clean up once it's been called.\n      var script = doc.createElement(\"script\");\n\n      script.onreadystatechange = function () {\n        runIfPresent(handle);\n        script.onreadystatechange = null;\n        html.removeChild(script);\n        script = null;\n      };\n\n      html.appendChild(script);\n    };\n  }\n\n  function installSetTimeoutImplementation() {\n    registerImmediate = function (handle) {\n      setTimeout(runIfPresent, 0, handle);\n    };\n  } // If supported, we should attach to the prototype of global, since that is where setTimeout et al. live.\n\n\n  var attachTo = Object.getPrototypeOf && Object.getPrototypeOf(global);\n  attachTo = attachTo && attachTo.setTimeout ? attachTo : global; // Don't get fooled by e.g. browserify environments.\n\n  if ({}.toString.call(global.process) === \"[object process]\") {\n    // For Node.js before 0.9\n    installNextTickImplementation();\n  } else if (canUsePostMessage()) {\n    // For non-IE10 modern browsers\n    installPostMessageImplementation();\n  } else if (global.MessageChannel) {\n    // For web workers, where supported\n    installMessageChannelImplementation();\n  } else if (doc && \"onreadystatechange\" in doc.createElement(\"script\")) {\n    // For IE 6–8\n    installReadyStateChangeImplementation();\n  } else {\n    // For older browsers\n    installSetTimeoutImplementation();\n  }\n\n  attachTo.setImmediate = setImmediate;\n  attachTo.clearImmediate = clearImmediate;\n})(typeof self === \"undefined\" ? typeof global === \"undefined\" ? this : global : self);\n/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(/*! ./../webpack/buildin/global.js */ \"yLpj\"), __webpack_require__(/*! ./../process/browser.js */ \"8oxB\")))\n\n//# sourceURL=webpack:///./node_modules/setimmediate/setImmediate.js?");

/***/ })

}]);