(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["vendor.portal-vue"],{

/***/ "K4j9":
/*!***********************************************************!*\
  !*** ./node_modules/portal-vue/dist/portal-vue.common.js ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
eval("/*! \n * portal-vue © Thorsten Lünborg, 2019 \n * \n * Version: 2.1.7\n * \n * LICENCE: MIT \n * \n * https://github.com/linusborg/portal-vue\n * \n*/\n\n\nObject.defineProperty(exports, '__esModule', {\n  value: true\n});\n\nfunction _interopDefault(ex) {\n  return ex && typeof ex === 'object' && 'default' in ex ? ex['default'] : ex;\n}\n\nvar Vue = _interopDefault(__webpack_require__(/*! vue */ \"oCYn\"));\n\nfunction _typeof(obj) {\n  if (typeof Symbol === \"function\" && typeof Symbol.iterator === \"symbol\") {\n    _typeof = function (obj) {\n      return typeof obj;\n    };\n  } else {\n    _typeof = function (obj) {\n      return obj && typeof Symbol === \"function\" && obj.constructor === Symbol && obj !== Symbol.prototype ? \"symbol\" : typeof obj;\n    };\n  }\n\n  return _typeof(obj);\n}\n\nfunction _toConsumableArray(arr) {\n  return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _nonIterableSpread();\n}\n\nfunction _arrayWithoutHoles(arr) {\n  if (Array.isArray(arr)) {\n    for (var i = 0, arr2 = new Array(arr.length); i < arr.length; i++) arr2[i] = arr[i];\n\n    return arr2;\n  }\n}\n\nfunction _iterableToArray(iter) {\n  if (Symbol.iterator in Object(iter) || Object.prototype.toString.call(iter) === \"[object Arguments]\") return Array.from(iter);\n}\n\nfunction _nonIterableSpread() {\n  throw new TypeError(\"Invalid attempt to spread non-iterable instance\");\n}\n\nvar inBrowser = typeof window !== 'undefined';\n\nfunction freeze(item) {\n  if (Array.isArray(item) || _typeof(item) === 'object') {\n    return Object.freeze(item);\n  }\n\n  return item;\n}\n\nfunction combinePassengers(transports) {\n  var slotProps = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};\n  return transports.reduce(function (passengers, transport) {\n    var temp = transport.passengers[0];\n    var newPassengers = typeof temp === 'function' ? temp(slotProps) : transport.passengers;\n    return passengers.concat(newPassengers);\n  }, []);\n}\n\nfunction stableSort(array, compareFn) {\n  return array.map(function (v, idx) {\n    return [idx, v];\n  }).sort(function (a, b) {\n    return compareFn(a[1], b[1]) || a[0] - b[0];\n  }).map(function (c) {\n    return c[1];\n  });\n}\n\nfunction pick(obj, keys) {\n  return keys.reduce(function (acc, key) {\n    if (obj.hasOwnProperty(key)) {\n      acc[key] = obj[key];\n    }\n\n    return acc;\n  }, {});\n}\n\nvar transports = {};\nvar targets = {};\nvar sources = {};\nvar Wormhole = Vue.extend({\n  data: function data() {\n    return {\n      transports: transports,\n      targets: targets,\n      sources: sources,\n      trackInstances: inBrowser\n    };\n  },\n  methods: {\n    open: function open(transport) {\n      if (!inBrowser) return;\n      var to = transport.to,\n          from = transport.from,\n          passengers = transport.passengers,\n          _transport$order = transport.order,\n          order = _transport$order === void 0 ? Infinity : _transport$order;\n      if (!to || !from || !passengers) return;\n      var newTransport = {\n        to: to,\n        from: from,\n        passengers: freeze(passengers),\n        order: order\n      };\n      var keys = Object.keys(this.transports);\n\n      if (keys.indexOf(to) === -1) {\n        Vue.set(this.transports, to, []);\n      }\n\n      var currentIndex = this.$_getTransportIndex(newTransport); // Copying the array here so that the PortalTarget change event will actually contain two distinct arrays\n\n      var newTransports = this.transports[to].slice(0);\n\n      if (currentIndex === -1) {\n        newTransports.push(newTransport);\n      } else {\n        newTransports[currentIndex] = newTransport;\n      }\n\n      this.transports[to] = stableSort(newTransports, function (a, b) {\n        return a.order - b.order;\n      });\n    },\n    close: function close(transport) {\n      var force = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : false;\n      var to = transport.to,\n          from = transport.from;\n      if (!to || !from && force === false) return;\n\n      if (!this.transports[to]) {\n        return;\n      }\n\n      if (force) {\n        this.transports[to] = [];\n      } else {\n        var index = this.$_getTransportIndex(transport);\n\n        if (index >= 0) {\n          // Copying the array here so that the PortalTarget change event will actually contain two distinct arrays\n          var newTransports = this.transports[to].slice(0);\n          newTransports.splice(index, 1);\n          this.transports[to] = newTransports;\n        }\n      }\n    },\n    registerTarget: function registerTarget(target, vm, force) {\n      if (!inBrowser) return;\n\n      if (this.trackInstances && !force && this.targets[target]) {\n        console.warn(\"[portal-vue]: Target \".concat(target, \" already exists\"));\n      }\n\n      this.$set(this.targets, target, Object.freeze([vm]));\n    },\n    unregisterTarget: function unregisterTarget(target) {\n      this.$delete(this.targets, target);\n    },\n    registerSource: function registerSource(source, vm, force) {\n      if (!inBrowser) return;\n\n      if (this.trackInstances && !force && this.sources[source]) {\n        console.warn(\"[portal-vue]: source \".concat(source, \" already exists\"));\n      }\n\n      this.$set(this.sources, source, Object.freeze([vm]));\n    },\n    unregisterSource: function unregisterSource(source) {\n      this.$delete(this.sources, source);\n    },\n    hasTarget: function hasTarget(to) {\n      return !!(this.targets[to] && this.targets[to][0]);\n    },\n    hasSource: function hasSource(to) {\n      return !!(this.sources[to] && this.sources[to][0]);\n    },\n    hasContentFor: function hasContentFor(to) {\n      return !!this.transports[to] && !!this.transports[to].length;\n    },\n    // Internal\n    $_getTransportIndex: function $_getTransportIndex(_ref) {\n      var to = _ref.to,\n          from = _ref.from;\n\n      for (var i in this.transports[to]) {\n        if (this.transports[to][i].from === from) {\n          return +i;\n        }\n      }\n\n      return -1;\n    }\n  }\n});\nvar wormhole = new Wormhole(transports);\nvar _id = 1;\nvar Portal = Vue.extend({\n  name: 'portal',\n  props: {\n    disabled: {\n      type: Boolean\n    },\n    name: {\n      type: String,\n      default: function _default() {\n        return String(_id++);\n      }\n    },\n    order: {\n      type: Number,\n      default: 0\n    },\n    slim: {\n      type: Boolean\n    },\n    slotProps: {\n      type: Object,\n      default: function _default() {\n        return {};\n      }\n    },\n    tag: {\n      type: String,\n      default: 'DIV'\n    },\n    to: {\n      type: String,\n      default: function _default() {\n        return String(Math.round(Math.random() * 10000000));\n      }\n    }\n  },\n  created: function created() {\n    var _this = this;\n\n    this.$nextTick(function () {\n      wormhole.registerSource(_this.name, _this);\n    });\n  },\n  mounted: function mounted() {\n    if (!this.disabled) {\n      this.sendUpdate();\n    }\n  },\n  updated: function updated() {\n    if (this.disabled) {\n      this.clear();\n    } else {\n      this.sendUpdate();\n    }\n  },\n  beforeDestroy: function beforeDestroy() {\n    wormhole.unregisterSource(this.name);\n    this.clear();\n  },\n  watch: {\n    to: function to(newValue, oldValue) {\n      oldValue && oldValue !== newValue && this.clear(oldValue);\n      this.sendUpdate();\n    }\n  },\n  methods: {\n    clear: function clear(target) {\n      var closer = {\n        from: this.name,\n        to: target || this.to\n      };\n      wormhole.close(closer);\n    },\n    normalizeSlots: function normalizeSlots() {\n      return this.$scopedSlots.default ? [this.$scopedSlots.default] : this.$slots.default;\n    },\n    normalizeOwnChildren: function normalizeOwnChildren(children) {\n      return typeof children === 'function' ? children(this.slotProps) : children;\n    },\n    sendUpdate: function sendUpdate() {\n      var slotContent = this.normalizeSlots();\n\n      if (slotContent) {\n        var transport = {\n          from: this.name,\n          to: this.to,\n          passengers: _toConsumableArray(slotContent),\n          order: this.order\n        };\n        wormhole.open(transport);\n      } else {\n        this.clear();\n      }\n    }\n  },\n  render: function render(h) {\n    var children = this.$slots.default || this.$scopedSlots.default || [];\n    var Tag = this.tag;\n\n    if (children && this.disabled) {\n      return children.length <= 1 && this.slim ? this.normalizeOwnChildren(children)[0] : h(Tag, [this.normalizeOwnChildren(children)]);\n    } else {\n      return this.slim ? h() : h(Tag, {\n        class: {\n          'v-portal': true\n        },\n        style: {\n          display: 'none'\n        },\n        key: 'v-portal-placeholder'\n      });\n    }\n  }\n});\nvar PortalTarget = Vue.extend({\n  name: 'portalTarget',\n  props: {\n    multiple: {\n      type: Boolean,\n      default: false\n    },\n    name: {\n      type: String,\n      required: true\n    },\n    slim: {\n      type: Boolean,\n      default: false\n    },\n    slotProps: {\n      type: Object,\n      default: function _default() {\n        return {};\n      }\n    },\n    tag: {\n      type: String,\n      default: 'div'\n    },\n    transition: {\n      type: [String, Object, Function]\n    }\n  },\n  data: function data() {\n    return {\n      transports: wormhole.transports,\n      firstRender: true\n    };\n  },\n  created: function created() {\n    var _this = this;\n\n    this.$nextTick(function () {\n      wormhole.registerTarget(_this.name, _this);\n    });\n  },\n  watch: {\n    ownTransports: function ownTransports() {\n      this.$emit('change', this.children().length > 0);\n    },\n    name: function name(newVal, oldVal) {\n      /**\r\n       * TODO\r\n       * This should warn as well ...\r\n       */\n      wormhole.unregisterTarget(oldVal);\n      wormhole.registerTarget(newVal, this);\n    }\n  },\n  mounted: function mounted() {\n    var _this2 = this;\n\n    if (this.transition) {\n      this.$nextTick(function () {\n        // only when we have a transition, because it causes a re-render\n        _this2.firstRender = false;\n      });\n    }\n  },\n  beforeDestroy: function beforeDestroy() {\n    wormhole.unregisterTarget(this.name);\n  },\n  computed: {\n    ownTransports: function ownTransports() {\n      var transports = this.transports[this.name] || [];\n\n      if (this.multiple) {\n        return transports;\n      }\n\n      return transports.length === 0 ? [] : [transports[transports.length - 1]];\n    },\n    passengers: function passengers() {\n      return combinePassengers(this.ownTransports, this.slotProps);\n    }\n  },\n  methods: {\n    // can't be a computed prop because it has to \"react\" to $slot changes.\n    children: function children() {\n      return this.passengers.length !== 0 ? this.passengers : this.$scopedSlots.default ? this.$scopedSlots.default(this.slotProps) : this.$slots.default || [];\n    },\n    // can't be a computed prop because it has to \"react\" to this.children().\n    noWrapper: function noWrapper() {\n      var noWrapper = this.slim && !this.transition;\n\n      if (noWrapper && this.children().length > 1) {\n        console.warn('[portal-vue]: PortalTarget with `slim` option received more than one child element.');\n      }\n\n      return noWrapper;\n    }\n  },\n  render: function render(h) {\n    var noWrapper = this.noWrapper();\n    var children = this.children();\n    var Tag = this.transition || this.tag;\n    return noWrapper ? children[0] : this.slim && !Tag ? h() : h(Tag, {\n      props: {\n        // if we have a transition component, pass the tag if it exists\n        tag: this.transition && this.tag ? this.tag : undefined\n      },\n      class: {\n        'vue-portal-target': true\n      }\n    }, children);\n  }\n});\nvar _id$1 = 0;\nvar portalProps = ['disabled', 'name', 'order', 'slim', 'slotProps', 'tag', 'to'];\nvar targetProps = ['multiple', 'transition'];\nvar MountingPortal = Vue.extend({\n  name: 'MountingPortal',\n  inheritAttrs: false,\n  props: {\n    append: {\n      type: [Boolean, String]\n    },\n    bail: {\n      type: Boolean\n    },\n    mountTo: {\n      type: String,\n      required: true\n    },\n    // Portal\n    disabled: {\n      type: Boolean\n    },\n    // name for the portal\n    name: {\n      type: String,\n      default: function _default() {\n        return 'mounted_' + String(_id$1++);\n      }\n    },\n    order: {\n      type: Number,\n      default: 0\n    },\n    slim: {\n      type: Boolean\n    },\n    slotProps: {\n      type: Object,\n      default: function _default() {\n        return {};\n      }\n    },\n    tag: {\n      type: String,\n      default: 'DIV'\n    },\n    // name for the target\n    to: {\n      type: String,\n      default: function _default() {\n        return String(Math.round(Math.random() * 10000000));\n      }\n    },\n    // Target\n    multiple: {\n      type: Boolean,\n      default: false\n    },\n    targetSlim: {\n      type: Boolean\n    },\n    targetSlotProps: {\n      type: Object,\n      default: function _default() {\n        return {};\n      }\n    },\n    targetTag: {\n      type: String,\n      default: 'div'\n    },\n    transition: {\n      type: [String, Object, Function]\n    }\n  },\n  created: function created() {\n    if (typeof document === 'undefined') return;\n    var el = document.querySelector(this.mountTo);\n\n    if (!el) {\n      console.error(\"[portal-vue]: Mount Point '\".concat(this.mountTo, \"' not found in document\"));\n      return;\n    }\n\n    var props = this.$props; // Target already exists\n\n    if (wormhole.targets[props.name]) {\n      if (props.bail) {\n        console.warn(\"[portal-vue]: Target \".concat(props.name, \" is already mounted.\\n        Aborting because 'bail: true' is set\"));\n      } else {\n        this.portalTarget = wormhole.targets[props.name];\n      }\n\n      return;\n    }\n\n    var append = props.append;\n\n    if (append) {\n      var type = typeof append === 'string' ? append : 'DIV';\n      var mountEl = document.createElement(type);\n      el.appendChild(mountEl);\n      el = mountEl;\n    } // get props for target from $props\n    // we have to rename a few of them\n\n\n    var _props = pick(this.$props, targetProps);\n\n    _props.slim = this.targetSlim;\n    _props.tag = this.targetTag;\n    _props.slotProps = this.targetSlotProps;\n    _props.name = this.to;\n    this.portalTarget = new PortalTarget({\n      el: el,\n      parent: this.$parent || this,\n      propsData: _props\n    });\n  },\n  beforeDestroy: function beforeDestroy() {\n    var target = this.portalTarget;\n\n    if (this.append) {\n      var el = target.$el;\n      el.parentNode.removeChild(el);\n    }\n\n    target.$destroy();\n  },\n  render: function render(h) {\n    if (!this.portalTarget) {\n      console.warn(\"[portal-vue] Target wasn't mounted\");\n      return h();\n    } // if there's no \"manual\" scoped slot, so we create a <Portal> ourselves\n\n\n    if (!this.$scopedSlots.manual) {\n      var props = pick(this.$props, portalProps);\n      return h(Portal, {\n        props: props,\n        attrs: this.$attrs,\n        on: this.$listeners,\n        scopedSlots: this.$scopedSlots\n      }, this.$slots.default);\n    } // else, we render the scoped slot\n\n\n    var content = this.$scopedSlots.manual({\n      to: this.to\n    }); // if user used <template> for the scoped slot\n    // content will be an array\n\n    if (Array.isArray(content)) {\n      content = content[0];\n    }\n\n    if (!content) return h();\n    return content;\n  }\n});\n\nfunction install(Vue$$1) {\n  var options = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};\n  Vue$$1.component(options.portalName || 'Portal', Portal);\n  Vue$$1.component(options.portalTargetName || 'PortalTarget', PortalTarget);\n  Vue$$1.component(options.MountingPortalName || 'MountingPortal', MountingPortal);\n}\n\nvar index = {\n  install: install\n};\nexports.default = index;\nexports.Portal = Portal;\nexports.PortalTarget = PortalTarget;\nexports.MountingPortal = MountingPortal;\nexports.Wormhole = wormhole;\n\n//# sourceURL=webpack:///./node_modules/portal-vue/dist/portal-vue.common.js?");

/***/ })

}]);