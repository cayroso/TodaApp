'use strict';


import Vue from 'vue';
import MomentVue from 'vue-moment';
Vue.use(MomentVue);

window.moment = require('moment');

Vue.filter('formatDate', function (value) {
    if (value) {
        return moment(String(value)).format('YYYY-MM-DD hh:mm:ss');
    }
});

Vue.filter('toMoment', function (value) {
    if (value) {
        return moment(String(value)).calendar();
    }
});

Vue.filter('toCurrency', function (value) {
    if (typeof value !== "number") {
        return value;
    }
    //var formatter = new Intl.NumberFormat('en-US', {
    //    style: 'currency',
    //    currency: 'USD',
    //    minimumFractionDigits: 2
    //});
    var formatter = new Intl.NumberFormat('en-PH', {
        style: 'currency',
        currency: 'PHP',
        minimumFractionDigits: 2
    });
    return formatter.format(value);
});
Vue.filter('prettyJson', function (value) {
    return JSON.stringify(value, null,1);
});

import '../_Core/Plugins/bootstrap-vue';

//  global components
import Pagination from '../_Core/Components/pagination.vue';
import SortField from '../_Core/Components/sortfield.vue';
import TableList from '../_Core/Components/table-list.vue';
import GMapLocation from '../_Core/Components/gmap-location.vue';

Vue.component('m-pagination', Pagination);
Vue.component('sort-field', SortField);
Vue.component('table-list', TableList);
Vue.component('gmap-location', GMapLocation);