<template>
    <div v-cloak>
        <div class="d-flex flex-column flex-sm-row justify-content-sm-between">
            <h4 class="mb-sm-0">
                <i class="fas fa-fw fa-user-plus mr-1"></i>Add Team
            </h4>
            <div class="text-right">
                <button @click="save" class="btn btn-primary">
                    <span class="fas fa-fw fa-save"></span>
                </button>
                <button @click="close" class="btn btn-secondary">
                    <span class="fas fa-fw fa-times-circle"></span>
                </button>
            </div>
        </div>

        <div class="form-group">
            <label for="name">Name</label>
            <div>
                <input v-model="item.name" type="text" id="name" class="form-control" />
            </div>
        </div>
        <div class="form-group">
            <label for="description">Description</label>
            <div>
                <input v-model="item.description" type="text" id="description" class="form-control" />
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

        },

        methods: {
            async save() {
                const vm = this;

                try {
                    const item = vm.item;

                    const payload = vm.$util.clone(item);

                    await vm.$util.axios.post(`/api/teams/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('New team created.', { title: 'Add Team', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            vm.close();
                        })
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>