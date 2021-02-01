'use strict';

import VueRouter from 'vue-router';

import index from './Pages/index.vue';

import accountsIndex from './Pages/Accounts/index.vue';

//import contactsIndex from './Pages/Contacts/index.vue';

import teamsIndex from './Pages/Teams/index.vue';
import teamsAdd from './Pages/Teams/add.vue';
import teamsView from './Pages/Teams/index.vue';

import usersIndex from './Pages/Users/index.vue';
import usersAdd from './Pages/Users/add.vue';
import usersView from './Pages/Users/view.vue';


//import tasksIndex from './Pages/Tasks/index.vue';
//import tasksAdd from './Pages/Tasks/add.vue';
//import tasksView from './Pages/Tasks/View/index.vue';

const NotFound = {
    template: '<div>Not found</div>'
};

const routes = [
    { path: '/', name: "index", component: index },

    { path: '/accounts', name: "accounts", component: accountsIndex },

    //{ path: '/contacts', name: "contacts", component: contactsIndex },

    { path: '/teams', name: "teams", component: teamsIndex },
    { path: '/teams/add', name: "teamsAdd", component: teamsAdd },
    { path: '/teams/view/:id', name: "teamsView", component: teamsView, props: true },

    { path: '/users', name: "users", component: usersIndex },
    { path: '/users/add', name: "usersAdd", component: usersAdd },
    { path: '/users/view/:id', name: "usersView", component: usersView, props: true },

    //{ path: '/tasks', name: "tasks", component: tasksIndex },

    { path: '*', component: NotFound },
];

const router = new VueRouter({
    base:'/administrator',
    mode: "history",
    routes: routes,
});

export default router;
