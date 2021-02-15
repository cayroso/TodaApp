<template>
    <div v-cloak>
        <b-modal ref="modal"
                 :no-close-on-esc="true"
                 :no-close-on-backdrop="true">
            <template v-slot:modal-header>
                <h5 class="font-weight-bold mb-0">
                    <span class="fa fas fa-star mr-1"></span>{{title}}
                </h5>
            </template>
            <template v-slot:modal-footer>
                <button @click="close" class="btn btn-outline-primary">Cancel</button>
                <button @click="save" class="btn btn-primary">Save</button>
            </template>
            <div>


                <div class="form-group">
                    <div class="">
                        <div class="btn-group-vertical btn-block">
                            <button class="btn px-4 py-2"
                                    v-for="(opt,index) in options"
                                    v-bind:class="{'btn-outline-primary': !opt.check, 'btn-primary': opt.check}"
                                    @click="check(opt)">
                                <span class="fas fa-fw fa-star" v-for="i in opt.id"></span>
                                <p>
                                    {{opt.title}}
                                </p>
                            </button>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div>
                        <textarea v-model="comment" rows="3" class="form-control" placeholder="Enter your comment here"></textarea>
                    </div>
                </div>
            </div>
        </b-modal>
    </div>
</template>
<script>
    export default {
        data() {
            return {
                title: 'Add Feedback',
                comment: null,
                selectedOption: null,
                options: [
                    { id: 1, title: 'Extremely Dissatisfied', check: false },
                    { id: 2, title: 'Somewhat Dissatisfied', check: false },
                    { id: 3, title: 'Neither Satisfied or Dissatisfied', check: false },
                    { id: 4, title: 'Somewhat Satisfied', check: false },
                    { id: 5, title: 'Extremely Satisfied', check: false },
                ]
            };
        },
        methods: {
            reset() {
                const vm = this;
                vm.title = 'Add Feedback';
                vm.comment = null;
                vm.selectedOption = null;
                vm.options.map(item => {
                    item.check = false;
                });
            },
            check(obj) {
                const vm = this;
                if (obj.check) {
                    obj.check = false;
                    vm.selectedOption = null;
                }
                else {
                    vm.options.map((item, i) => {
                        item.check = obj.id === item.id;
                        if (item.check)
                            vm.selectedOption = item;
                    });
                }
            },
            open(tripId, driverId, riderId, feedback) {
                const vm = this;

                vm.reset();
              
                if (feedback != null) {
                    vm.title = 'Edit Feedback';
                    vm.comment = feedback.comment;
                    vm.options.map(item => {
                        if (item.id == feedback.rate) {
                            item.check = true;
                            vm.selectedOption = item;
                        }
                    });
                }
                vm.$refs.modal.show();
            },
            close() {
                let vm = this;

                vm.$refs.modal.hide();
            },
            async save() {
                const vm = this;
                //if (!vm.comment) {
                //    alert('Comment is required.')
                //    return;
                //}
                try {
                    const payload = {
                        comment: vm.comment,
                        rate: vm.selectedOption.id
                    };

                    vm.$emit('onSave', payload);
                    //await vm.$util.axios.post(`api/feedbacks/addOrUpdate`, payload)
                    //    .then(resp => {
                    //        vm.$bvToast.toast(`Your feedback was successfully saved.`, { title: vm.title, variant: 'success' });
                    //        vm.$bus.closeFeedback(vm.jobId);
                    //        vm.close();
                    //    });
                } catch (e) {
                    vm.$util.handleError(e);
                }
            },
        }
    }
</script>