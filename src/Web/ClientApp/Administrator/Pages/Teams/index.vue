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
                        <th>Members</th>
                        <th>Action(s)</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <div>
                                {{row.item.name}}
                            </div>
                            <small>{{row.item.description}}</small>
                        </td>
                        <td>
                            <ol class="list-unstyled">
                                <li v-for="item in row.item.members">
                                    <div class="d-flex flex-row justify-content-between">
                                        <div>
                                            <b-avatar :src="item.urlProfilePicture" size="sm"></b-avatar>
                                            <span v-if="item.userId === uid">
                                                {{item.name}}
                                            </span>
                                            <a v-else @click.prevent="$bus.$emit('event:send-message', item.userId)" href="#">
                                                {{item.name}}
                                            </a>
                                        </div>
                                        <button @click="removeTeamMember(row.item, item)" class="btn btn-sm btn-outline-danger">
                                            <i class="fas fa-fw fa-trash"></i>
                                        </button>
                                    </div>
                                </li>
                            </ol>
                        </td>                        
                        <td>
                            <button @click="addTeamMember(row.item)" class="btn btn-sm btn-primary">
                                <i class="fas fa-fw fa-user-plus mr-1"></i>Add Member
                            </button>

                            <button @click="removeTeam(row.item)" class="btn btn-sm btn-danger">
                                <i class="fas fa-fw fa-trash mr-1"></i>Remove Team
                            </button>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    <div>
                                        {{row.item.name}}
                                    </div>
                                    <small>{{row.item.description}}</small>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Members</label>
                                <div class="col align-self-center">
                                    <ul class="list-unstyled">
                                        <li v-for="item in row.item.members">
                                            <div class="d-flex flex-row justify-content-between">
                                                <div>
                                                    <b-avatar :src="item.urlProfilePicture" size="sm"></b-avatar>
                                                    <span v-if="item.userId === uid">
                                                        {{item.name}}
                                                    </span>
                                                    <a v-else @click.prevent="$bus.$emit('event:send-message', item.userId)" href="#">
                                                        {{item.name}}
                                                    </a>
                                                </div>
                                                <button @click="removeTeamMember(row.item, item)" class="btn btn-sm btn-outline-danger">
                                                    <i class="fas fa-fw fa-trash"></i>
                                                </button>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <div >
                                        <button @click="addTeamMember(row.item)" class="btn btn-sm btn-outline-primary">
                                            <i class="fas fa-fw fa-user-plus"></i>
                                            <span class="ml-1 d-none d-sm-inline-flex">
                                                Add Member
                                            </span>
                                        </button>
                                        <button @click="removeTeam(row.item)" class="btn btn-sm btn-outline-danger">
                                            <i class="fas fa-fw fa-trash"></i>
                                            <span class="ml-1 d-none d-sm-inline-flex">
                                                Remove Team
                                            </span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </template>
                </table-list>





            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>

        <modal-add-member ref="modalAddMember" @saved="search"></modal-add-member>
    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';
    import modalAddMember from '../../Modals/Teams/add-member.vue';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            //modalAddTask
            modalAddMember
        },
        data() {
            return {
                baseUrl: `/api/teams`,
                filter: {
                    cacheKey: `filter-${this.uid}/teams`,
                    //query: {
                    //    orderStatus: 0,
                    //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                    //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    //}
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
            },

            async removeTeamMember(team, user) {
                const vm = this;

                try {
                    this.$bvModal.msgBoxConfirm(`Are you sure you want to remove "${user.name}" from "${team.name}"?`)
                        .then(async value => {
                            if (value) {

                                await vm.$util.axios.delete(`/api/teams/${team.teamId}/remove-member/${user.userId}/`)
                                    .then(resp => {
                                        vm.$bvToast.toast('User removed from group.', { title: 'Remove User from Group', variant: 'success', toaster: 'b-toaster-bottom-right' });
                                    })

                                vm.search();
                            }
                        })
                        .catch(err => {
                            vm.$util.handleError(err);
                        });
                } catch (e) {
                    vm.$util.handleError(e)
                }
            },

            async removeTeam(team) {
                const vm = this;

                try {
                    this.$bvModal.msgBoxConfirm(`Are you sure you want to delete "${team.name}"?`)
                        .then(async value => {
                            if (value) {

                                await vm.$util.axios.delete(`/api/teams/${team.teamId}`)
                                    .then(resp => {
                                        vm.$bvToast.toast('Team deleted.', { title: 'Delete Team', variant: 'warning', toaster: 'b-toaster-bottom-right' });
                                    })

                                vm.search();
                            }
                        })
                        .catch(err => {
                            vm.$util.handleError(err);
                        });                   
                } catch (e) {
                    vm.$util.handleError(e);
                }
            },
        }
    }
</script>