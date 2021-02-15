<template>
    <div v-cloak>
        <div class="row row-cols-2 row-cols-sm-4">
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Drivers</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.drivers}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-motorcycle"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Riders</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.riders}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-street-view"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Trips</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.totalComplatedTripCount}} / {{item.totalTripsCount}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-map-marked"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Total Fare</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.totalEarnings|toCurrency}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-money-bill"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="mb-2">
            <div class="card">
                
                <div class="table-responsive mb-0">
                    <table class="table">
                        <thead>
                            <tr>
                                <th></th>
                                <th>#</th>
                                <th>Name</th>
                                <th>Fare</th>
                                <th>Trip</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th colspan="5">Top Drivers</th>
                            </tr>

                            <tr v-for="(t,index) in item.topDrivers">
                                <td></td>
                                <td>{{index+1}}</td>
                                <td>{{t.rider}}</td>
                                <td>{{t.totalFare|toCurrency}}</td>
                                <td>{{t.totalTrip}}</td>
                            </tr>
                            <tr>
                                <th colspan="5">Top Riders</th>
                            </tr>
                            <tr v-for="(t,index) in item.topRiders">
                                <td></td>
                                <td>{{index+1}}</td>
                                <td>{{t.rider}}</td>
                                <td>{{t.totalFare|toCurrency}}</td>
                                <td>{{t.totalTrip}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

            </div>
        </div>

        

    </div>
</template>
<script>
    import pageMixin from '../../_Core/Mixins/pageMixin';

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

            await vm.get();
        },

        methods: {
            async get() {
                const vm = this;

                try {
                    await vm.$util.axios.get(`/api/administrators/default/dashboard`)
                        .then(resp => vm.item = resp.data);
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>