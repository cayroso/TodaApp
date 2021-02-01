<style scoped>
    label {
        font-size: small;
        font-weight: bold;
    }
</style>
<template>
    <b-modal ref="modal"
             :no-close-on-esc="false"
             :no-close-on-backdrop="true"
             scrollable>
        <template v-slot:modal-header>
            <div class="w-100">
                <div class="d-flex flex-row  align-items-center justify-content-between">
                    <h5 class="m-0">
                        Add Team Member
                    </h5>
                </div>
            </div>
        </template>
        <template v-slot:modal-footer>
            <button @click="save" class="btn btn-primary">
                Save
            </button>

            <button @click="close" class="btn btn-secondary">
                Close
            </button>
        </template>
        <div>
            <div class="form-group col-md-3">
                <label>Status</label>
                <div>{{team.name}}</div>
            </div>
            <div class="form-group col-md">
                <label for="memberId">Member</label>
                <div>
                    <!--<b-form-checkbox-group v-model="selected"
                                           :options="members"
                                           value-field="id"
                                           text-field="name"
                                           stacked>
                    </b-form-checkbox-group>-->

                    <b-form-select v-model="memberId" :options="members" text-field="name" value-field="id" v-bind:class="getValidClass('memberId')"></b-form-select>
                    <span v-if="validations.get('memberId')" class="text-danger">
                        {{validations.get('memberId')}}
                    </span>
                </div>
            </div>
        </div>
    </b-modal>
</template>
<script>
    export default {
        data() {
            return {
                moment: moment,
                isDirty: false,
                validations: new Map(),
                busy: false,

                teamId: null,
                memberId: null,
                members: [],

                team: {},
            }
        },
        methods: {
            async getTeam(teamId) {
                const vm = this;

                try {
                    await vm.$util.axios.get(`/api/teams/${teamId}`)
                        .then(resp => {
                            vm.team = resp.data;
                        });

                } catch (e) {

                }
            },
            getValidClass(field) {
                const vm = this;

                if (!vm.isDirty)
                    return '';

                if (vm.validations.has(field))
                    return 'is-invalid';
                return 'is-valid';
            },

            reset() {
                const vm = this;

                vm.busy = false;
                vm.teamId = null;
                vm.memberId = null;
            },

            async open(id) {
                const vm = this;

                vm.teamId = id;
                vm.teamName = name;

                await vm.getTeam(id);
                await vm.get();
                

                vm.$refs.modal.show();
            },

            close() {
                const vm = this;

                vm.$refs.modal.hide();
            },

            async get() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    await vm.$util.axios.get(`/api/users/lookup/`)
                        .then(resp => {
                            const users = resp.data;
                            const members = [];

                            users.forEach(u => {                                
                                const fnd = vm.team.members.find(tm => tm.userId == u.id);

                                if (!fnd) {
                                    members.push(u);
                                }

                            });

                            vm.members = members;
                            
                        })
                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            },

            async save() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    const payload = {
                        teamId: vm.teamId,
                        memberId: vm.memberId
                    };

                    await vm.$util.axios.post(`/api/teams/add-member/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('Member added.', { title: 'Add Member', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            vm.$emit('saved');
                            vm.close();
                        })
                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            }
        }
    }
</script>