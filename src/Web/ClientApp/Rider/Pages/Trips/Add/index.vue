<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-map-marked mr-1"></i>Add Trip
                </h1>
            </div>
            <div class="col-auto">
                <button @click="save" class="btn btn-primary">
                    <i class="fas fa-fw fa-save"></i>
                </button>
                <button @click="close" class="btn btn-secondary">
                    <i class="fas fa-fw fa-times"></i>
                </button>
            </div>
        </div>
        <div class="mt-2">
            <div class="form-row">
                <div class="form-group col-sm">
                    <label>
                        <i class="fas fa-fw fa-map-marker"></i>Pickup
                    </label>
                    <div readonly>
                        {{item.from.address}}
                    </div>
                    <textarea rows="2" class="form-control" v-model="item.from.addressDescription" placeholder="Enter description"></textarea>
                </div>

                <div class="form-group col-sm">
                    <label>
                        <i class="fas fa-fw fa-map-marker"></i>Destination
                    </label>
                    <div readonly>
                        {{item.to.address}}
                    </div>
                    <textarea rows="2" class="form-control" v-model="item.to.addressDescription" placeholder="Enter description"></textarea>
                </div>
            </div>


        </div>
        <div class="mt-2">
            <div style="height:500px;">
                <rider-map map-name="add-trip"
                           :fixed="false"
                           :show-location="true"
                           :cx="item.geoX" :cy="item.geoY"
                           :fromX="item.from.geoX"
                           :fromY="item.from.geoY"
                           :toX="item.to.geoX"
                           :toY="item.to.geoY"
                           :draggable="true"
                           @onMapReady="onMapReady"
                           @onFromAddress="onFromAddress"
                           @onToAddress="onToAddress">
                </rider-map>
            </div>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../../_Core/Mixins/pageMixin';
    import RiderMap from './_rider-map.vue';

    export default {
        mixins: [pageMixin],
        components: {
            RiderMap
        },
        props: {
            uid: String,
        },

        data() {
            return {
                item: {
                    from: {
                        address: null,
                        addressDescription: '',
                        geoX: 13.816806399999999,// 0,
                        geoY: 121.08500999999998,//0,
                    },
                    to: {
                        address: null,
                        addressDescription: '',
                        geoX: 13.816806399999999,
                        geoY: 121.08500999999998,
                    },

                    geoX: 0, geoY: 0
                }
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
            async onMapReady() {
                const vm = this;
                //debugger;
                //if (vm.id) {
                //    await vm.get();
                //}
            },

            onFromAddress(address, location) {
                const vm = this;
                //debugger
                vm.item.from.address = address.formatted_address;
                vm.item.from.geoX = location.lat;
                vm.item.from.geoY = location.lng;
            },
            onToAddress(address, location) {
                const vm = this;
                //debugger
                vm.item.to.address = address.formatted_address;
                vm.item.to.geoX = location.lat;
                vm.item.to.geoY = location.lng;
            },

            async save() {
                const vm = this;

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = {
                        startAddress: item.from.address || '',
                        startAddressDescription: item.from.addressDescription || '',
                        startX: item.from.geoX,
                        startY: item.from.geoY,

                        endAddress: item.to.address || '',
                        endAddressDescription: item.to.addressDescription || '',
                        endX: item.to.geoX,
                        endY: item.to.geoY
                    };

                    await vm.$util.axios.post(`/api/trips/rider/add`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('Trip created.', { title: 'Add Trip', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            setTimeout(() => {
                                vm.$router.push({ name: 'tripsView', params: { id: resp.data } });
                            }, 500);
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