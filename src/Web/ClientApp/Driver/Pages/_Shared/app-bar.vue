<template>
    <b-navbar toggleable="sm" fixed="top" :sticky="true" type="dark" variant="dark">
        <div class="container-lg">
            <a href="/" class="navbar-brand">{{appName}}</a>
            <div class="collapse navbar-collapse" id="navbarColor01">
                <ul class="navbar-nav">
                    <li v-for="menu in menus" class="nav-item">
                        <router-link :to="menu.to" class="nav-link">
                            <span v-bind:class="menu.icon" class="mr-1"></span>
                            <span v-text="menu.label" class="d-none d-md-inline"></span>
                        </router-link>
                    </li>
                </ul>
            </div>

            <ul class="navbar-nav ml-auto flex-row">
                <!-- Nav Item - Messages -->
                <li class="nav-item px-2 px-sm-0">
                    <a v-b-toggle.messagesDrawer class="nav-link" @click.prevent href="#">
                        <i class="fas fa-envelope fa-fw"></i>
                        <!-- Counter - Messages -->
                        <span class="badge badge-danger badge-counter invisible initialHidden">
                            <span v-if="messages.length>0">{{messages.length}}</span>
                        </span>
                    </a>
                </li>

                <!--<li class="nav-item px-2 px-sm-0">
                    <a v-b-toggle.teamsDrawer class="nav-link" @click.prevent href="#">
                        <i class="fas fa-users fa-fw"></i>
                    </a>
                </li>-->

                <!-- Nav Item - User Information -->
                <li class="nav-item px-2 px-sm-0 dropdown">
                    <a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <b-avatar variant="info" size="sm" :src="urlProfilePicture"></b-avatar>
                    </a>
                    <!-- Dropdown - User Information -->
                    <div class="dropdown-menu dropdown-menu-right" aria-labelledby="userDropdown" style="position:absolute">
                        <router-link to="/accounts" class="dropdown-item">
                            <i class="fas fa-user fa-sm fa-fw mr-2"></i>
                            Account
                        </router-link>

                        <div class="dropdown-divider"></div>

                        <a href="/" class="dropdown-item">
                            <i class="fas fa-home fa-sm fa-fw mr-2"></i>
                            Home Page
                        </a>

                        <div class="dropdown-divider"></div>

                        <a class="dropdown-item" href="#" data-toggle="modal" data-target="#logoutModal">
                            <i class="fas fa-sign-out-alt fa-sm fa-fw mr-2 "></i>
                            Logout
                        </a>
                    </div>
                </li>
            </ul>

        </div>

        <!--<nav-drawer :appName="appName"></nav-drawer>-->
        <notifications-drawer :notifications="notifications"></notifications-drawer>
        <messages-drawer :messages="messages"></messages-drawer>
        <!--<teams-drawer :uid="uid"></teams-drawer>-->
    </b-navbar>
</template>
<script>
    import navbarMixin from '../../../_Core/Mixins/navbarMixin';

    //import navDrawer from './nav-drawer.vue';
    import NotificationsDrawer from '../../../_Common/Drawers/notifications-drawer.vue';
    import MessagesDrawer from '../../../_Common/Drawers/messages-drawer.vue';
    import TeamsDrawer from '../../../_Common/Drawers/teams-drawer.vue';

    export default {
        mixins: [navbarMixin],
        props: {
            uid: String,
            appName: {
                type: String, required: true,
                default: 'LMS'
            },
            urlProfilePicture: String,
            menus: Array
        },
        components: {
            //navDrawer,
            NotificationsDrawer,
            MessagesDrawer,
            TeamsDrawer,
        }
    };
</script>