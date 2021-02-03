<template>
    <div v-cloak>

        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-users mr-1"></i>Teams
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <div class="mr-1">
                        <router-link to="/teams/add" class="btn btn-primary">
                            <i class="fas fa-plus"></i>
                        </router-link>
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

        </b-collapse>

        <b-overlay :show="busy">
            <div class="mt-2 table-responsive shadow-sm">
                <table-list :header="{key: 'teamId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Members</th>
                        <th>Date</th>
                        <td></td>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <div>
                                {{row.item.name}}
                            </div>

                        </td>
                        <td>
                            {{row.item.description}}
                        </td>
                        <td>
                            <ul>
                                <li v-for="item in row.item.members">
                                    {{item.firstName}} {{item.lastName}}
                                    <button v-if="item.userId !== uid" @click="$bus.$emit('event:send-message', item.userId)" class="btn btn-sm btn-primary">
                                        <i class="fas fa-fw fa-comment"></i>
                                    </button>
                                </li>
                            </ul>
                        </td>
                        <td>
                            {{row.item.dateCreated|moment('calendar')}}
                        </td>
                        <td>
                            <button @click="addTeamMember(row.item)">Add Member</button>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    {{row.item.firstName}} {{row.item.middleName}} {{row.item.lastName}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Email</label>
                                <div class="col align-self-center">
                                    {{row.item.email}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Phone Number</label>
                                <div class="col align-self-center">
                                    {{row.item.phoneNumber}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Roles</label>
                                <div class="col align-self-center">
                                    <ul>
                                        <li v-for="r in row.item.roles">
                                            {{r}}
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Teams</label>
                                <div class="col align-self-center">
                                    tbh
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <button v-if="row.item.userId !== uid" @click="$bus.$emit('event:send-message',row.item.userId)" class="btn btn-sm btn-primary">
                                        <i class="fas fa-fw fa-comment"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </template>
                </table-list>





            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>

        <!--<modal-add-member ref="modalAddMember"></modal-add-member>-->
    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';
    //import modalAddMember from '../../Modals/Teams/add-member.vue';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            //modalAddTask
            //modalAddMember
        },
        data() {
            return {
                baseUrl: `/api/teams/my-teams`,
                filter: {
                    cacheKey: `filter-${this.uid}/teams`,
                },
            };
        },

        computed: {

        },

        async created() {
            const vm = this;
            const cache = JSON.parse(localStorage.getItem(vm.filter.cacheKey)) || {};

            vm.initializeFilter(cache);

            await vm.search();

        },

        async mounted() {
            const vm = this;

        },

        methods: {
            addTeamMember(item) {
                const vm = this;

                //const payload = {
                //    contactId: item.contactId,
                //    firstName: item.firstName,
                //    middleName: item.middleName,
                //    lastName: item.lastName,

                //    statusText: item.statusText,
                //    rating: item.rating,
                //};

                vm.$refs.modalAddMember.open(item.teamId, item.name);
            }
        }
    }
</script>