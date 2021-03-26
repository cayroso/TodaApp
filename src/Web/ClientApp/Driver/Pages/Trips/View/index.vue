<style>
    label {
        font-size: small;
        font-weight: bold;
    }
</style>
<template>
    <div v-cloak>
        <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center">
            <h1 class="h3 mb-sm-0">
                <span class="fas fa-fw fa-map-marked mr-1"></span>View
                <b-form-rating v-model="item.riderRating" id="rating-inline" inline no-border size="sm" readonly></b-form-rating>
            </h1>

            <div class="text-right">
                <button v-if="item.status===10" @click="addFeedback()" class="btn btn-primary">
                    Rate Rider
                </button>

                <button v-if="item.status===7" @click="startTrip()" class="btn btn-primary">Start</button>
                <button v-if="item.status===9" @click="completeTrip()" class="btn btn-primary">Complete</button>

                <button v-if="item.status==4 || item.status==6 || item.status === 8" @click="offerFare()" class="btn btn-primary">Offer Fare</button>

                <button v-if="item.status==3" @click="acceptRequest()" class="btn btn-primary">Accept</button>
                <button v-if="item.status==3 || item.status==4" @click="rejectRequest()" class="btn btn-warning">Reject</button>

                <button @click="get" class="btn btn-primary">
                    <i class="fas fa-fw fa-sync"></i>
                </button>
                <button @click="close" class="btn btn-secondary">
                    <i class="fas fa-fw fa-times"></i>
                </button>
                <!--<div class="col-auto mr-1">
                    <a :href="urlAdd" class="btn btn-primary">
                        <span class="fas fa-fw fa-plus"></span>
                    </a>
                </div>
                <div class="col-auto mr-1">
                    <button @click="filter.visible = !filter.visible" class="btn btn-primary">
                        <span class="fas fa-fw fa-filter"></span>
                    </button>
                </div>
                <div class="col">
                    <div class="input-group">
                        <input v-model="filter.query.criteria" @keyup.enter="search(1)" type="text" class="form-control">
                        <div class="input-group-append">
                            <button @click="search(1)" class="btn btn-secondary" type="button" id="button-addon2">
                                <span class="fa fas fa-fw fa-search"></span>
                            </button>
                        </div>
                    </div>
                </div>-->
            </div>
        </div>

        <div class="card mt-2">
            <div @click="toggle('information')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-info-circle mr-1"></span>Information
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.information" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.information">
                <div class="p-2">
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label>Status</label>
                            <div class="form-control-plaintext">
                                {{item.statusText}}
                            </div>
                            <div class="small">
                                {{item.cancelReason}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Pickup</label>
                            <div class="form-control-plaintext">
                                {{item.startAddress}}
                                <div class="small">
                                    {{item.startAddressDescription}}
                                </div>
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Destination</label>
                            <div class="form-control-plaintext">
                                {{item.endAddress}}
                                <div class="small">
                                    {{item.endAddressDescription}}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </b-collapse>
        </div>

        <div class="card mt-2">
            <div @click="toggle('rider')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-street-view mr-1"></span>Rider
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.rider" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.rider">
                <div class="border-bottom">
                    <div class="text-right p-1">
                        <button @click.prevent="$bus.$emit('event:send-message', item.rider.riderId)" class="btn btn-primary">
                            <i class="fas fa-fw fa-envelope"></i>
                        </button>
                        <a :href="`tel:${item.rider.phoneNumber}`" class="btn btn-primary">
                            <i class="fas fa-fw fa-phone"></i>
                        </a>
                        <a :href="`sms:${item.rider.phoneNumber}`" class="btn btn-primary">
                            <i class="fas fa-fw fa-sms"></i>
                        </a>
                    </div>
                </div>
                <div class="p-2">

                    <div class="mt-2 form-row">
                        <div class="form-group col-md">
                            <label>Name</label>
                            <div class="align-self-center">
                                <b-avatar :src="item.rider.urlProfilePicture" :inline="true"></b-avatar>
                                <span>
                                    {{item.rider.name}}
                                </span>
                                <div class="mt-2">
                                    <b-form-rating v-model="item.rider.overallRating" inline no-border readonly size="sm"></b-form-rating>
                                </div>
                            </div>

                        </div>
                        <div class="form-group col-sm">
                            <label>Phone Number</label>
                            <div class="form-control-plaintext">
                                {{item.rider.phoneNumber}}
                            </div>
                        </div>
                        <div class="form-group col-sm">
                            <label>Offer</label>
                            <div class="form-control-plaintext">
                                {{item.fare|toCurrency}}
                            </div>
                        </div>
                    </div>

                </div>
            </b-collapse>
        </div>
        <div class="card mt-2">
            <div @click="toggle('map')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-map mr-1"></span>Map
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.map" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>

            </div>
            <b-collapse v-model="toggles.map">
                <div class="form-row">
                    <div class="form-group col">
                        <label>
                            <i class="fas fa-fw fa-map-signs"></i>Distance
                        </label>
                        <div readonly>
                            &nbsp;{{calculatedTrip.distance.text}}
                        </div>
                    </div>
                    <div class="form-group col">
                        <label>
                            <i class="fas fa-fw fa-clock"></i>Duration
                        </label>
                        <div readonly>
                            &nbsp;{{calculatedTrip.duration.text}}
                        </div>
                    </div>
                </div>
                <div v-if="item.startAddress && toggles.map" style="height:500px;">
                    <rider-map map-name="add-trip"
                               :fixed="false"
                               :show-location="true"
                               :cx="item.startX"
                               :cy="item.startY"
                               :startX="item.startX"
                               :startY="item.startY"
                               :endX="item.endX"
                               :endY="item.endY"
                               :draggable="false"
                               @onCalculatedTrip="onCalculatedTrip">
                    </rider-map>
                </div>
            </b-collapse>
        </div>
        <div class="card mt-2">
            <div @click="toggle('timeline')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-history mr-1"></span>Timeline
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.timeline" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.timeline">
                <div class="table-responsive mb-0">
                    <table class="table table-sm">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Status</th>
                                <th>Notes</th>

                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(tl,index) in item.timelines">
                                <td>{{index+1}}</td>
                                <td>
                                    <div>
                                        {{tl.statusText}}
                                    </div>
                                    <div class="small ml-3">
                                        {{tl.dateTimeline|moment('calendar')}}
                                    </div>
                                </td>
                                <td>{{tl.notes}}</td>

                            </tr>
                        </tbody>
                    </table>
                </div>
            </b-collapse>
        </div>

        <add-feedback ref="addFeedback" @onSave="onSaveFeedback"></add-feedback>
    </div>
</template>
<script>
    import pageMixin from '../../../../_Core/Mixins/pageMixin';
    import RiderMap from './_rider-map.vue';

    import addFeedback from '../../../../_Common/Modals/Trips/add-feedback.vue';

    export default {
        mixins: [pageMixin],
        components: {
            RiderMap,
            addFeedback
        },
        props: {
            uid: String,
            id: String,
        },

        data() {
            return {
                togglesKey: `view-trip/driver/${this.uid}/toggles`,
                toggles: {
                    information: false,
                    rider: false,
                    map: false,
                    timeline: false,
                    locations: false,
                },
                calculatedTrip: {
                    distance: {},
                    duration: {},
                },
                item: {
                    driver: {},
                    rider: {}
                }
            };
        },

        computed: {

        },

        async created() {
            const vm = this;

            const toggles = JSON.parse(localStorage.getItem(vm.togglesKey)) || null;

            if (toggles)
                vm.toggles = toggles;
        },

        async mounted() {
            const vm = this;

            await vm.get();

            vm.$bus.$on('event:driver-assigned', vm.refresh);
            vm.$bus.$on('event:rider-offered-fare-accepted', vm.refresh);
            vm.$bus.$on('event:rider-offered-fare-rejected', vm.refresh);
            vm.$bus.$on('event:rider-trip-cancelled', vm.refresh);
            vm.$bus.$on('event:rider-trip-inprogress', vm.refresh);
            vm.$bus.$on('event:rider-trip-completed', vm.refresh);
        },

        methods: {
            async addFeedback() {
                const vm = this;
                const feedback = {
                    rate: vm.item.driverRating,
                    comment: vm.item.driverComment,
                };
                vm.$refs.addFeedback.open(vm.id, vm.item.driver.driverId, null, feedback);
            },
            async onSaveFeedback(info) {
                ;
                const vm = this;

                try {
                    const payload = {
                        tripId: vm.id,
                        rating: info.rate,
                        comment: info.comment
                    };

                    await vm.$util.axios.put(`/api/trips/driver-feedback`, payload)
                        .then(_ => {
                            vm.$refs.addFeedback.close();
                        });

                    await vm.get();

                } catch (e) {
                    vm.$util.handleError(e);
                }


            },

            onCalculatedTrip(info) {
                this.calculatedTrip = info;
            },
            async refresh(resp) {
                const vm = this;

                if (resp.tripId === vm.id) {
                    await vm.get();
                }
            },
            async get() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    await vm.$util.axios.get(`/api/trips/${vm.id}`)
                        .then(resp => {
                            vm.item = resp.data;
                        })

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            },

            async acceptRequest() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = {
                        tripId: item.tripId,
                        token: item.token,
                    };
                    await vm.$util.axios.put(`/api/trips/driver/accept-rider-request`, payload)
                        .then(async resp => {
                            vm.$bvToast.toast('Trip request Accepted.', { title: 'Accept Trip Request', variant: 'success', toaster: 'b-toaster-bottom-right' });
                        })

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;

                    await vm.get();
                }
            },

            async rejectRequest() {
                const vm = this;

                if (vm.busy)
                    return;

                var notes = prompt("Enter reason here...", "Reject Reason");

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = {
                        tripId: item.tripId,
                        token: item.token,
                        notes: notes
                    };
                    await vm.$util.axios.put(`/api/trips/driver/reject-rider-request`, payload)
                        .then(async resp => {
                            vm.$bvToast.toast('Trip request rejected.', { title: 'Reject Trip Request', variant: 'success', toaster: 'b-toaster-bottom-right' });
                        })

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;

                    await vm.get();
                }
            },

            async offerFare() {
                var fare = prompt("Fare Offered", 0);

                if (isNaN(fare) || fare <= 0) {
                    alert('Invalid value for fare.');
                    return;
                }

                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = {
                        tripId: item.tripId,
                        token: item.token,
                        fare: fare,
                        notes: 'notes here'
                    };

                    await vm.$util.axios.put(`/api/trips/driver/offer-fare-to-rider-request`, payload)
                        .then(async resp => {
                            vm.$bvToast.toast('Fare offer sent to rider.', { title: 'Offer Fare', variant: 'success', toaster: 'b-toaster-bottom-right' });
                        })

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;

                    await vm.get();
                }
            },

            async startTrip() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    await vm.$util.axios.put(`/api/trips/${item.tripId}/driver-inprogress/${item.token}`)
                        .then(async resp => {
                            vm.$bvToast.toast('Trip started.', { title: 'Start Trip', variant: 'success', toaster: 'b-toaster-bottom-right' });
                        })

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;

                    await vm.get();
                }
            },

            async completeTrip() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    await vm.$util.axios.put(`/api/trips/${item.tripId}/driver-complete/${item.token}`)
                        .then(async resp => {
                            vm.$bvToast.toast('Trip completed.', { title: 'Complete Trip', variant: 'success', toaster: 'b-toaster-bottom-right' });
                        });

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;

                    await vm.get();
                }
            },
        }
    }
</script>