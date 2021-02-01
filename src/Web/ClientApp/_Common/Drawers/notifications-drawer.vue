<template>
    <b-sidebar id="notificationsDrawer" title="Notifications" backdrop right shadow>
        <div v-if="notifications" v-cloak>
            <a v-for="item in notifications" v-on:click="viewNotification(item.notificationId)" style="cursor:pointer" class="dropdown-item d-flex align-items-center">
                <div class="mr-3">

                    <i v-bind:class="item.iconClass"></i>
                </div>
                <div>
                    <div class="small text-gray-500">{{item.dateSent|moment('calendar')}}</div>
                    <span class="font-weight-bold">{{item.subject}}</span>
                </div>
            </a>
        </div>
        <a class="dropdown-item text-center small" href="/patient/notifications">Show All Motifications</a>
    </b-sidebar>
</template>
<script>
    export default {
        props: {
            notifications: Array
        },

        methods: {

            async getUnreadNotifications() {

                const vm = this;

                try {
                    await vm.$util.axios.get(`api/accounts/unread-notifications`)
                        .then(resp => {
                            vm.notifications = resp.data;
                        });
                } catch (e) {
                    vm.$util.handleError(e);
                }
            },

            viewNotification(id) {
                this.$bus.$emit('event:open-notification', id);
            },

            showHiddenElements() {
                let vm = this;

                let elems = vm.$el.getElementsByClassName('initialHidden');

                for (let i = 0; i < elems.length; i++) {
                    let el = elems[i];

                    el.classList.remove('invisible');
                }

            }
        }
    };
</script>