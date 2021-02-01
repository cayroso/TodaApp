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
                        Manage User Roles
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
            <div class="form-group">
                <label>User</label>
                <div>
                    {{item.firstName}} {{item.middleName}} {{item.lastName}}
                </div>
            </div>
            <div class="form-group">
                <label for="memberId">Roles</label>
                <div>
                    <b-form-group v-slot="{ ariaDescribedby }">
                        <b-form-checkbox-group id="checkbox-group-1"
                                               v-model="selectedRoles"
                                               :options="roles"
                                               :aria-describedby="ariaDescribedby"
                                               name="flavour-1"></b-form-checkbox-group>
                    </b-form-group>
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

                userId: null,
                item: {},
                selectedRoles: [],
                roles: [
                    //{ value: 'system', text: 'System' },
                    { value: 'administrator', text: 'Administrator' },
                    { value: 'manager', text: 'Manager' },
                    { value: 'member', text: 'Member' },
                ],
            }
        },
        methods: {
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
                vm.userId = null;
                vm.item = {};
            },

            async open(id) {
                const vm = this;

                vm.userId = id;

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
                    await vm.$util.axios.get(`/api/administrators/users/${vm.userId}/`)
                        .then(resp => {
                            vm.item = resp.data;
                            vm.selectedRoles = vm.item.roles.map(e => e.roleId);
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
                        userId: vm.userId,
                        roleIds: vm.selectedRoles
                    };

                    await vm.$util.axios.post(`/api/administrators/users/manage-roles/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('User roles updated.', { title: 'Update User Roles', variant: 'success', toaster: 'b-toaster-bottom-right' });

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