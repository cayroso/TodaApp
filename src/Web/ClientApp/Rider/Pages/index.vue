<template>
    <div v-cloak>
        <div>
            under construction
        </div>
        <div class="d-none row row-cols-2 row-cols-sm-3">
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Teams</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.teams}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-users"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Users</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.users}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-user"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Tasks</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.tasks}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-tasks"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Contacts</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.contacts}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-id-card"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Attachments</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.attachments}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-paperclip"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Documents</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.documents}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-archive"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
        },

        data() {
            return {
                item: {}
            };
        },

        computed: {

        },

        async created() {
            const vm = this;

        },

        async mounted() {
            const vm = this;

            //await vm.get();
        },

        methods: {
            async get() {
                const vm = this;

                try {
                    await vm.$util.axios.get(`/api/managers/default/dashboard`)
                        .then(resp => vm.item = resp.data);
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>