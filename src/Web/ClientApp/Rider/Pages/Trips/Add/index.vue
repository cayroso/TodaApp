<template>
    <div v-cloak>
        Add Trip
        <div class="row">
            <div class="col-md">
                <div class="card">
                    <div class="card-header">
                        Pickup Location
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <label>Address</label>
                            <div class="form-control-static">
                                {{item.from.address}}
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Description</label>
                            <textarea rows="3" class="form-control" v-model="item.from.addressDescription"></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md">
                <div class="card">
                    <div class="card-header">
                        Destination Location
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <label>Address</label>
                            <div class="form-control-static">
                                {{item.to.address}}
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Description</label>
                            <textarea rows="3" class="form-control" v-model="item.to.addressDescription"></textarea>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div>
            {{item}}
        </div>

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
                        addressDescription: null,
                        geoX: 13.816806399999999,// 0,
                        geoY: 121.08500999999998,//0,
                    },
                    to: {
                        address: null,
                        addressDescription: null,
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
        }
    }
</script>