<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-tachometer-alt mr-1"></i>Dashboard
                </h1>
            </div>
            <div class="col-auto">
                <button @click="get" class="btn btn-outline-primary">
                    <i class="fas fa-fw fa-sync"></i>
                </button>
            </div>
        </div>

        <div class="mt-2">
            <div class="row">
                <div class="col-sm">
                    <div class="card">
                        <div class="card-header">
                            Fare and Trip Summary
                        </div>
                        <div class="table-responsive mb-0">
                            <table class="table">
                                <tbody>
                                    <tr>
                                        <td></td>
                                        <th>Month</th>
                                        <th>Week</th>
                                        <th>Today</th>
                                    </tr>
                                    <tr>
                                        <th>Earnings</th>
                                        <td>
                                            {{item.sumMonthEarnings|toCurrency}}
                                        </td>
                                        <td>
                                            {{item.sumWeekEarnings|toCurrency}}
                                            <i v-if="item.sumWeekEarnings>=item.sumLastWeekEarnings" class="fas fa-fw fa-arrow-up text-danger"></i>
                                            <i v-else class="fas fa-fw fa-arrow-down text-success"></i>
                                        </td>
                                        <td>
                                            {{item.sumTodayEarnings|toCurrency}}
                                            <i v-if="item.sumTodayEarnings>=item.sumYesterdayEarnings" class="fas fa-fw fa-arrow-up text-danger"></i>
                                            <i v-else class="fas fa-fw fa-arrow-down text-success"></i>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th># of Trips</th>
                                        <td>
                                            {{item.totalMonthTrips}}
                                        </td>
                                        <td>
                                            {{item.totalWeekTrips}}
                                            <i v-if="item.totalWeekTrips>=item.totalLastWeekTrips" class="fas fa-fw fa-arrow-up text-danger"></i>
                                            <i v-else class="fas fa-fw fa-arrow-down text-success"></i>
                                        </td>
                                        <td>
                                            {{item.totalTodayTrips}}
                                            <i v-if="item.totalWeekTrips>=item.totalLastWeekTrips" class="fas fa-fw fa-arrow-up text-danger"></i>
                                            <i v-else class="fas fa-fw fa-arrow-down text-success"></i>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <div class="d-block d-sm-none p-2">

                </div>

                <div class="col-sm">
                    <div class="card">
                        <div class="card-header">
                            Top Riders
                        </div>
                        <div class="table-responsive mb-0">
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th>Driver</th>
                                        <th>Total Fare</th>
                                        <th># of Trips</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(r,index) in item.topDrivers">
                                        <td>{{index+1}}</td>
                                        <td>{{r.rider}}</td>
                                        <td>{{r.totalFare|toCurrency}}</td>
                                        <td>{{r.totalTrip}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
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
        components: {
            //modalViewTask, modalChangeTheme
        },
        data() {
            return {
                moment: moment,

                togglesKey: `dashboard/${this.uid}/toggles`,
                toggles: {
                    tasks: true,
                    contacts: true,
                    attachments: true,
                },

                item: {
                    newTasks: [],
                    upcomingTasks: [],
                    overdueTasks: [],

                    newContacts: [],

                    recentAttachments: []
                }
            };
        },
        computed: {
            //emptyTasks() {
            //    const vm = this;

            //    return vm.item.newTasks.length === 0
            //        && vm.item.upcomingTasks.length === 0
            //        && vm.item.overdueTasks.length === 0;
            //},

            //emptyContacts() {
            //    const vm = this;

            //    return vm.item.newContacts.length === 0;
            //},

            //emptyAttachments() {
            //    const vm = this;

            //    return vm.item.recentAttachments.length === 0;
            //},
        },


        async created() {
            const vm = this;

            const toggles = JSON.parse(localStorage.getItem(vm.togglesKey)) || null;

            if (toggles)
                vm.toggles = toggles;

            await vm.get();
        },

        async mounted() {
            //const vm = this;
            //await vm.get();
        },

        methods: {
            async get() {
                const vm = this;

                if (vm.busy)
                    return;

                vm.busy = true;

                try {
                    await vm.$util.axios.get(`/api/riders/default/dashboard`)
                        .then(resp => {
                            vm.item = resp.data;
                        });
                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            }
        }
    }
</script>