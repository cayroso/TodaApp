<template>
    <div v-cloak>
        <div class="d-flex flex-column flex-sm-row justify-content-sm-between">
            <h4 class="mb-sm-0">
                <i class="fas fa-fw fa-user-plus mr-1"></i>Add User
            </h4>
            <div class="text-right">
                <button :disabled="isDirty && !formIsValid" @click="save" class="btn btn-primary">
                    <span class="fas fa-fw fa-save"></span>
                </button>
                <button @click="close" class="btn btn-secondary">
                    <span class="fas fa-fw fa-times-circle"></span>
                </button>
            </div>
        </div>

        <div class="form-row">
            <div class="form-group col-md">
                <label for="email">Email</label>
                <div>
                    <input v-model="item.email" type="text" id="email" class="form-control"  v-bind:class="getValidClass('email')"/>
                    <div v-if="validations.has('email')" class="invalid-feedback">
                        {{validations.get('email')}}
                    </div>
                </div>
            </div>
            <div class="form-group col-md">
                <label for="phoneNumber">Phone Number</label>
                <div>
                    <input v-model="item.phoneNumber" type="text" id="phoneNumber" class="form-control" v-bind:class="getValidClass('phoneNumber')" />
                    <div v-if="validations.has('phoneNumber')" class="invalid-feedback">
                        {{validations.get('phoneNumber')}}
                    </div>
                </div>
            </div>
        </div>
        <div class="form-row">
            <div class="form-group col-md">
                <label for="firstName">First Name</label>
                <div>
                    <input v-model="item.firstName" type="text" id="firstName" class="form-control"  v-bind:class="getValidClass('firstName')"/>
                    <div v-if="validations.has('firstName')" class="invalid-feedback">
                        {{validations.get('firstName')}}
                    </div>
                </div>
            </div>
            <div class="form-group col-md">
                <label for="middleName">Middle Name</label>
                <div>
                    <input v-model="item.middleName" type="text" id="middleName" class="form-control"  v-bind:class="getValidClass('middleName')"/>
                    <div v-if="validations.has('middleName')" class="invalid-feedback">
                        {{validations.get('middleName')}}
                    </div>
                </div>
            </div>
            <div class="form-group col-md">
                <label for="lastName">Last Name</label>
                <div>
                    <input v-model="item.lastName" type="text" id="lastName" class="form-control"  v-bind:class="getValidClass('lastName')"/>
                    <div v-if="validations.has('lastName')" class="invalid-feedback">
                        {{validations.get('lastName')}}
                    </div>
                </div>
            </div>
        </div>



        <div class="form-row">
            <div class="form-group col-md">
                <label for="password">Password</label>
                <div>
                    <input v-model="item.password" type="password" id="password" class="form-control"  v-bind:class="getValidClass('password')"/>
                    <div v-if="validations.has('password')" class="invalid-feedback">
                        {{validations.get('password')}}
                    </div>
                </div>
            </div>
            <div class="form-group col-md">
                <label for="confirmPassword">Confirm Password</label>
                <div>
                    <input v-model="item.confirmPassword" type="password" id="confirmPassword" class="form-control" v-bind:class="getValidClass('confirmPassword')" />
                    <div v-if="validations.has('confirmPassword')" class="invalid-feedback">
                        {{validations.get('confirmPassword')}}
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
        },

        data() {
            return {
                isDirty: false,
                validations: new Map(),

                item: {
                    email: null,
                    phoneNumber: null,
                    firstName: null,
                    middleName: null,
                    lastName: null,
                    password: null,
                    confirmPassword: null
                }
            };
        },

        computed: {
            formIsValid() {
                const vm = this;

                //if (!vm.isDirty)
                //    return true;

                const item = vm.item;

                const validations = new Map();

                if (!item.email) {
                    validations.set('email', 'Email is required.');
                }

                if (!item.phoneNumber) {
                    validations.set('phoneNumber', 'Phone Number is required.');
                }

                const missingName = !item.firstName && !item.middleName && !item.lastName;

                if (!item.firstName) {
                    validations.set('firstName', 'First name is required.');                    
                }

                if (!item.lastName) {                    
                    validations.set('lastName', 'Last name is required.');
                }

                if (!item.password) {
                    validations.set('password', 'Password is required.');
                }

                if (!item.confirmPassword) {
                    validations.set('confirmPassword', 'Confirm Password is required.');
                }

                if (item.password !== item.confirmPassword) {
                    validations.set('password', 'Password and Confirm Password must.');
                    validations.set('confirmPassword', 'Password and Confirm Password must.');
                }

                vm.isDirty = true;
                vm.validations = validations;

                return validations.size == 0;
            },
        },

        async created() {
            const vm = this;

        },

        async mounted() {
            const vm = this;

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

            async save() {
                const vm = this;

                if (vm.busy)
                    return;

                if (!vm.formIsValid)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = vm.$util.clone(item);

                    await vm.$util.axios.post(`/api/administrators/users/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('New user created.', { title: 'Add User', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            setTimeout(() => {
                                vm.close();
                            }, 500);
                            
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