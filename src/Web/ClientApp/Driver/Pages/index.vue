<template>
    <div v-cloak>
        <div class="text-right pb-1">
            <button @click="get" class="btn btn-sm btn-outline-primary">
                <i class="fas fa-fw fa-sync"></i>
            </button>
        </div>
        <div class="row">
            <div class="col">
                <div class="card shadow-sm">
                    <div @click="toggle('tasks')" class="card-header bg-primary text-white ">
                        <div class="d-flex flex-row justify-content-between align-items-center">
                            <div>Task Summary</div>
                            <div>
                                <span>
                                    <span v-if="toggles.tasks" class="fas fa-fw fa-angle-up"></span>
                                    <span v-else class="fas fa-fw fa-angle-down"></span>
                                </span>
                            </div>
                        </div>
                    </div>

                    <b-collapse v-model="toggles.tasks">
                        <b-overlay :show="busy">
                            <div v-if="emptyTasks" class="text-center">
                                No Data
                            </div>
                            <div v-else class="table-responsive mb-0">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th></th>
                                            <th>Title</th>
                                            <th>Contact</th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <tr v-for="(t,index) in item.newTasks">
                                            <th v-bind:rowspan="item.newTasks.length" v-if="index==0" class="text-center">New</th>
                                            <td>
                                                <a @click.prevent="$refs.modalViewTask.open(t.taskId)" href="#">
                                                    {{t.title}}
                                                </a>
                                                <div class="badge badge-success">{{t.taskTypeText}}</div>
                                            </td>
                                            <td>
                                                <router-link :to="{name:'contactsView', params:{id:t.contactId}}">
                                                    {{t.firstName}} {{t.middleName}} {{t.lastName}}
                                                </router-link>
                                            </td>
                                        </tr>

                                        <tr v-for="(t,index) in item.upcomingTasks">
                                            <th v-bind:rowspan="item.upcomingTasks.length" v-if="index==0" class="text-center">Upcoming</th>
                                            <td>
                                                <a @click.prevent="$refs.modalViewTask.open(t.taskId)" href="#">
                                                    {{t.title}}
                                                </a>
                                                <div class="badge badge-info">{{t.taskTypeText}}</div>
                                            </td>
                                            <td>
                                                <router-link :to="{name:'contactsView', params:{id:t.contactId}}">
                                                    {{t.firstName}} {{t.middleName}} {{t.lastName}}
                                                </router-link>
                                            </td>
                                        </tr>

                                        <tr v-for="(t,index) in item.overdueTasks">
                                            <th v-bind:rowspan="item.overdueTasks.length" v-if="index==0" class="text-center">Overdue</th>
                                            <td>

                                                <!--<span>{{t.taskStatusText}}: </span>-->
                                                <a @click.prevent="$refs.modalViewTask.open(t.taskId)" href="#">
                                                    {{t.title}}
                                                </a>
                                                <div class="badge badge-danger">{{t.taskTypeText}}</div>
                                            </td>
                                            <td>
                                                <router-link :to="{name:'contactsView', params:{id:t.contactId}}">
                                                    {{t.firstName}} {{t.middleName}} {{t.lastName}}
                                                </router-link>
                                            </td>
                                        </tr>
                                    </tbody>

                                </table>
                            </div>
                        </b-overlay>
                    </b-collapse>

                </div>

                <div class="mt-2 card shadow-sm">
                    <div @click="toggle('contacts')" class="card-header bg-success text-white">
                        <div class="d-flex flex-row justify-content-between align-items-center">
                            <div>New Contacts</div>
                            <div>
                                <span>
                                    <span v-if="toggles.contacts" class="fas fa-fw fa-angle-up"></span>
                                    <span v-else class="fas fa-fw fa-angle-down"></span>
                                </span>
                            </div>
                        </div>
                    </div>
                    <b-collapse v-model="toggles.contacts">
                        <b-overlay :show="busy">
                            <div v-if="emptyContacts" class="text-center">
                                No Data
                            </div>
                            <div v-else class="table-responsive mb-0">
                                <table class="table">
                                    <tbody>
                                        <tr v-for="c in item.newContacts">
                                            <td>
                                                <router-link :to="{name:'contactsView', params:{id:c.contactId}}">
                                                    {{c.firstName}} {{c.middleName}} {{c.lastName}}
                                                </router-link>
                                                <div class="small">
                                                    {{c.statusText}}
                                                </div>
                                            </td>
                                            <td>
                                                <b-form-rating inline no-border size="sm" v-model="c.rating" readonly variant="success"></b-form-rating>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </b-overlay>
                    </b-collapse>
                </div>

                <div class="mt-2 card shadow-sm">
                    <div @click="toggle('attachments')" class="card-header bg-info text-white">
                        <div class="d-flex flex-row justify-content-between align-items-center">
                            <div>Recent Attachments</div>
                            <div>
                                <span>
                                    <span v-if="toggles.attachments" class="fas fa-fw fa-angle-up"></span>
                                    <span v-else class="fas fa-fw fa-angle-down"></span>
                                </span>
                            </div>
                        </div>
                    </div>
                    <b-collapse v-model="toggles.attachments">
                        <b-overlay :show="busy">
                            <div v-if="emptyAttachments" class="text-center">
                                No Data
                            </div>

                            <div class="table-responsive mb-0">
                                <table class="table">
                                    <tbody>
                                        <template v-for="att in item.recentAttachments">
                                            <tr>
                                                <td>
                                                    <i v-if="att.attachmentType===1" class="fas fa-fw fa-sticky-note"></i>
                                                    <i v-else-if="att.attachmentType===2">
                                                        <i v-if="att.contentType && att.contentType.startsWith('image/')" class="fas fa-fw fa-image"></i>
                                                        <i v-else class="fas fa-fw fa-paperclip"></i>
                                                    </i>

                                                    <span v-if="att.attachmentType===1" class="text-break">
                                                        {{att.title}}
                                                    </span>
                                                    <span v-if="att.attachmentType===2" class="text-break">
                                                        {{att.fileName}}
                                                    </span>
                                                </td>
                                                <td>
                                                    <div class="text-right">
                                                        <button v-if="att.attachmentType===1 || (att.contentType && att.contentType.startsWith('image/'))" v-b-toggle="`view-${att.contactAttachmentId}`" class="btn btn-sm btn-outline-info mt-1 mt-sm-0">
                                                            <i class="fas fa-fw fa-eye"></i>
                                                        </button>

                                                        <a v-if="att.attachmentType===2" :href="att.url" class="btn btn-sm btn-outline-info mt-1 mt-sm-0">
                                                            <i class="fas fa-fw fa-file-download"></i>
                                                        </a>

                                                        <router-link :to="{name:'contactsView', params:{id:att.contactId}}" class="btn btn-sm btn-outline-info mt-1 mt-sm-0">
                                                            <i class="fas fa-fw fa-id-card"></i>
                                                        </router-link>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="p-0 m-1" colspan="3">


                                                    <b-collapse :id="`view-${att.contactAttachmentId}`" class="p-1">

                                                        <div v-if="att.attachmentType===1">
                                                            {{att.content}}
                                                        </div>
                                                        <div v-if="att.attachmentType===2">
                                                            <div v-if="att.contentType && att.contentType.startsWith('image/')">
                                                                <b-img-lazy :src="att.url" fluid center></b-img-lazy>
                                                            </div>
                                                            <div v-else>
                                                                <object :data="`${att.url}`"
                                                                        :type="att.contentType"
                                                                        height="100%"
                                                                        width="100%">
                                                                    <p>
                                                                        Your browser does not support PDFs.
                                                                        <a href="https://example.com/test.pdf">Download the PDF</a>.
                                                                    </p>
                                                                </object>
                                                            </div>
                                                        </div>
                                                    </b-collapse>
                                                </td>
                                            </tr>
                                        </template>
                                    </tbody>
                                </table>
                            </div>
                        </b-overlay>
                    </b-collapse>
                </div>
            </div>

        </div>

        <modal-view-task ref="modalViewTask" @updated="get" @removed="get"></modal-view-task>
    </div>
</template>
<script>
    import pageMixin from '../../_Core/Mixins/pageMixin';
    import modalViewTask from '../Modals/Tasks/view-task.vue';
    import modalChangeTheme from '../../_Common/Modals/Accounts/change-theme.vue';
    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
        },
        components: {
            modalViewTask, modalChangeTheme
        },
        data() {
            return {
                moment: moment,

                togglesKey: `dashboard/${this.uid}/toggles`,
                toggles: {
                    tasks: true,
                    contacts: true,
                    attachments: true,
                },

                item: {
                    newTasks: [],
                    upcomingTasks: [],
                    overdueTasks: [],

                    newContacts: [],

                    recentAttachments: []
                }
            };
        },
        computed: {
            emptyTasks() {
                const vm = this;

                return vm.item.newTasks.length === 0
                    && vm.item.upcomingTasks.length === 0
                    && vm.item.overdueTasks.length === 0;
            },

            emptyContacts() {
                const vm = this;

                return vm.item.newContacts.length === 0;
            },

            emptyAttachments() {
                const vm = this;

                return vm.item.recentAttachments.length === 0;
            },
        },


        async created() {
            const vm = this;

            const toggles = JSON.parse(localStorage.getItem(vm.togglesKey)) || null;

            if (toggles)
                vm.toggles = toggles;

            //await vm.get();
        },

        async mounted() {
            //const vm = this;
            //await vm.get();
        },

        methods: {
            async get() {
                const vm = this;

                if (vm.busy)
                    return;

                vm.busy = true;

                try {
                    await vm.$util.axios.get(`/api/members/default/dashboard`)
                        .then(resp => {
                            vm.item = resp.data;
                        });
                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            }
        }
    }
</script>