<template>
    <div v-cloak>

        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-tasks mr-1"></i>Tasks
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <!--<div class="mr-1">
                        <router-link to="/contacts/add" class="btn btn-primary">
                            <i class="fas fa-plus"></i>
                        </router-link>
                    </div>-->

                    <!--<div v-if="filter.visible" class="mr-1">
                        <button @click="resetDates" class="btn btn-primary">
                            <i class="fas fa-sync mr-1"></i>
                        </button>
                    </div>-->
                    <div class="mr-1">
                        <button @click="filter.visible = !filter.visible" class="btn btn-secondary">
                            <span class="fa fas fa-fw fa-filter"></span>
                        </button>
                    </div>
                    <div class="flex-grow-1">
                        <div class="input-group">
                            <input v-model="filter.query.criteria" @keyup.enter="search(1)" type="text" class="form-control" placeholder="Enter criteria..." aria-label="Enter criteria..." aria-describedby="button-addon2">
                            <div class="input-group-append">
                                <button @click="search(1)" class="btn btn-primary" type="button" id="button-addon2">
                                    <span class="fa fas fa-fw fa-search"></span>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <b-collapse v-model="filter.visible">
            <div class="card p-2 mt-2">
                <div class="row">
                    <div class="col-6 col-sm-4 col-md-3">
                        <div class="form-group">
                            <label class="col-form-label">Status</label>
                            <b-select v-model="filter.query.taskStatus" :options="lookups.taskStatuses" :value-field="`id`" :text-field="`name`">
                                <!--<template v-slot:first>
                                    <option :value="null">All</option>
                                </template>-->
                            </b-select>
                        </div>
                    </div>
                    <div class="col-6 col-sm-4 col-md-3">
                        <div class="form-group">
                            <label class="col-form-label">Type</label>
                            <b-form-select v-model="filter.query.taskType" :options="lookups.taskTypes" :value-field="`id`" :text-field="`name`">
                                <!--<template v-slot:first>
                                    <option :value="null">All</option>
                                </template>-->
                            </b-form-select>
                        </div>
                    </div>

                </div>
            </div>
        </b-collapse>

        <b-overlay :show="busy">
            <div class="mt-2 table-responsive shadow-sm">
                <table-list :header="{key: 'taskId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Type</th>
                        <th>Title</th>
                        <th>Contact</th>
                        <th>Date</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center" v-bind:class="getTaskTdCss(row.item)"></td>
                        <td v-text="row.item.taskTypeText"></td>

                        <td>
                            <a @click.prevent="$refs.modalViewTask.open(row.item.taskId)" href="#">
                                {{row.item.title}}
                            </a>
                            <div class="small">
                                {{row.item.taskStatusText}}
                            </div>
                        </td>
                        <td>
                            <router-link :to="{name:'contactsView', params:{id: row.item.contact.contactId}}">
                                {{row.item.contact.firstName}} {{row.item.contact.middleName}} {{row.item.contact.lastName}}
                            </router-link>
                        </td>
                        <td class="small">
                            <div>
                                Created: {{row.item.dateCreated|moment('calendar')}}
                            </div>
                            <div>
                                Assigned: {{row.item.dateAssigned|moment('calendar')}}
                            </div>
                            <div v-if="moment(row.item.dateCompleted).isBefore()">
                                To Complete: {{row.item.dateCompleted|moment('calendar')}}
                            </div>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Status</label>
                                <div class="col align-self-center">
                                    {{row.item.taskStatusText}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    <a @click.prevent="$refs.modalViewTask.open(row.item.taskId)" href="#">
                                        {{row.item.title}}
                                    </a>
                                    <div class="small">
                                        {{row.item.taskTypeText}}
                                    </div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Contact</label>
                                <div class="col align-self-center">
                                    <router-link :to="{name:'contactsView', params:{id: row.item.contact.contactId}}">
                                        {{row.item.contact.firstName}} {{row.item.contact.middleName}} {{row.item.contact.lastName}}
                                    </router-link>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Date</label>
                                <div class="col align-self-center small">
                                    <div>
                                        Created: {{row.item.dateCreated|moment('calendar')}}
                                    </div>
                                    <div>
                                        Assigned: {{row.item.dateAssigned|moment('calendar')}}
                                    </div>
                                    <div v-if="moment(row.item.dateCompleted).isBefore()">
                                        To Complete: {{row.item.dateCompleted|moment('calendar')}}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </template>
                </table-list>





            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>
        <modal-view-task ref="modalViewTask" @updated="search" @removed="search"></modal-view-task>
    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';
    import modalViewTask from '../../Modals/Tasks/view-task.vue';
    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            modalViewTask
        },
        data() {
            return {
                moment: moment,

                baseUrl: `/api/managers/tasks`,
                lookups: {
                    taskStatuses: [
                        { id: 0, name: 'All' },
                        { id: 1, name: 'Todo' },
                        { id: 2, name: 'In Progress' },
                        { id: 3, name: 'Done' },
                    ],
                    taskTypes: [
                        { id: 0, name: 'All' },
                        { id: 1, name: 'Follow-Up Email' },
                        { id: 2, name: 'Phone Call' },
                        { id: 3, name: 'Demo' },
                        { id: 4, name: 'Lunch Meeting' },
                        { id: 5, name: 'Meetup' },
                        { id: 6, name: 'Others' },
                    ]
                },
                filter: {
                    cacheKey: `filter-${this.uid}/contacts`,
                    query: {
                        taskStatus: 0,
                        taskType: 0,
                        //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                        //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    }
                },
            };
        },

        computed: {

        },

        async created() {
            const vm = this;
            const filter = vm.filter;

            const urlParams = new URLSearchParams(window.location.search);
            const cache = JSON.parse(localStorage.getItem(filter.cacheKey)) || {};

            filter.query.taskType = urlParams.get('tt') || cache.taskType || filter.query.taskType;
            filter.query.taskStatus = urlParams.get('ts') || cache.taskStatus || filter.query.taskStatus;

            vm.initializeFilter(cache);

            await vm.search();

        },

        async mounted() {
            const vm = this;

        },

        methods: {
            getQuery() {

                const vm = this;
                const filter = vm.filter;

                if (vm.busy)
                    return;

                const query = [
                    '?c=', encodeURIComponent(filter.query.criteria),
                    '&p=', filter.query.pageIndex,
                    '&s=', filter.query.pageSize,
                    '&sf=', filter.query.sortField,
                    '&so=', filter.query.sortOrder,
                    '&tt=', filter.query.taskType,
                    '&ts=', filter.query.taskStatus,
                ].join('');

                return query;
            },

            saveQuery() {
                const vm = this;
                const filter = vm.filter;

                localStorage.setItem(filter.cacheKey, JSON.stringify({
                    criteria: filter.query.criteria,
                    pageIndex: filter.query.pageIndex,
                    pageSize: filter.query.pageSize,
                    sortField: filter.query.sortField,
                    sortOrder: filter.query.sortOrder,
                    visible: filter.visible,
                    taskType: filter.query.taskType,
                    taskStatus: filter.query.taskStatus,
                }));
            },

            getTaskTdCss(task) {

                switch (task.taskStatus) {
                    case 1:
                        return 'table-warning';
                    case 2:
                        return 'table-info';
                    case 3:
                        return 'table-success';
                    default:
                        return '';
                }
            },
        }
    }
</script>