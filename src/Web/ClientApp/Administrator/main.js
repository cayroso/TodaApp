'use strict';

import '../_Core/core';
import './main.css';

import Vue from 'vue';
import App from './Pages/_Shared/app.vue';

import VueObserveVisibility from 'vue-observe-visibility'
Vue.use(VueObserveVisibility);

import common from '../_Core/Plugins/common';
Vue.use(common);

import VueRouter from 'vue-router';
Vue.use(VueRouter);

import Router from './router';

new Vue({
    el: '#app',
    router: Router,
    components: {
        App
    },
    created() {
        $(document).ready(function () {
            $('.main').addClass('main-loaded');
        });

        //let theme = localStorage.getItem('theme') || '';

        //if (theme) {
        //    //debugger;
        //    let style = document.createElement('link');
        //    style.type = "text/css";
        //    style.rel = "stylesheet";
        //    style.href = theme;// 'https://bootswatch.com/4/yeti/bootstrap.min.css';
        //    document.head.appendChild(style);
        //}
    }
});