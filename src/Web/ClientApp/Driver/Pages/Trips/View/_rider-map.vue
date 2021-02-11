<style></style>
<template>
    <div :id="mapName" style="height:100%;width:100%;" v-cloak></div>
</template>
<script>
    export default {
        props: {
            mapName: String,
            draggable: Boolean,
            fixed: Boolean,
            showLocation: Boolean,
            cx: Number,
            cy: Number,

            startX: Number,
            startY: Number,
            endX: Number,
            endY: Number,

            markerClickAction: Function,
            //items: Array,
        },
        watch: {
            'cx': function (newValue, oldValue) {
                const vm = this;
                //debugger
                if (vm.map.setCenter) {
                    //var lng = vm.centerPosition.lng();
                    //debugger
                    //vm.centerPosition.lat = newValue;
                    //vm.centerPosition = new google.maps.LatLng(newValue,lng);
                    //vm.map.setCenter(vm.centerPosition);
                    //vm.setMarker();
                }
            },
            'cy': function (newValue, oldValue) {
                const vm = this;
                //debugger
                if (vm.map.setCenter) {
                    //vm.centerPosition.lng = newValue;
                    //vm.map.setCenter(vm.centerPosition);
                    //vm.setMarker();
                }
            }
        },
        data() {
            return {
                navigator: {},
                centerPosition: { lat: 13.942504351499613, lng: 120.72873957918004 },// { lat: 13.8954684059025, lng: 120.906667412659 }, //13.942504351499613, 120.72873957918004
                map: {},
                marker: {},
                infoWindow: {},
                geocode: {},
                directionsService: {},
                directionsRenderer: {},

                items: [],
                markers: [],

                markerA: {},
                markerB: {},
            };
        },
        created() {
            const vm = this;
        },
        beforeUpdate() {
            debugger;
        },
        updated() {
            debugger;
        },
        async mounted() {
            const vm = this;

            vm.navigator = navigator;
            vm.navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia || navigator.oGetUserMedia;
            vm.centerPosition = {
                lat: vm.cx, lng: vm.cy
            };
            
            var timerId = setInterval(_ => {
                if (google && google.maps) {

                    clearInterval(timerId);
                    
                    vm.initMap();

                    vm.$emit('onMapReady');
                }
            }, 250);
        },

        methods: {
            async initMap() {
                const vm = this;

                /*
                Build list of map types.
                You can also use var mapTypeIds = ["roadmap", "satellite", "hybrid", "terrain", "OSM"]
                but static lists suck when Google updates the default list of map types.
                */
                var mapTypeIds = [];
                for (var type in google.maps.MapTypeId) {
                    mapTypeIds.push(google.maps.MapTypeId[type]);
                }
                //mapTypeIds.push("OSM");

                const lastZoom = Number.parseInt(localStorage.getItem('zoom')) || 15;

                vm.map = new google.maps.Map(document.getElementById(vm.mapName), {
                    center: vm.centerPosition,//{ lat: 13.948779, lng: 120.733035 }, //13.948779,120.733035
                    zoom: lastZoom,
                    //mapTypeId: "OSM",
                    mapTypeControl: true,
                    streetViewControl: false,
                    mapTypeControlOptions: {
                        mapTypeIds: mapTypeIds
                    }
                });

                vm.map.mapTypes.set("OSM", new google.maps.ImageMapType({
                    getTileUrl: function (coord, zoom) {
                        // "Wrap" x (longitude) at 180th meridian properly
                        // NB: Don't touch coord.x: because coord param is by reference, and changing its x property breaks something in Google's lib
                        var tilesPerGlobe = 1 << zoom;
                        var x = coord.x % tilesPerGlobe;
                        if (x < 0) {
                            x = tilesPerGlobe + x;
                        }
                        // Wrap y (latitude) in a like manner if you want to enable vertical infinite scrolling

                        return "https://tile.openstreetmap.org/" + zoom + "/" + x + "/" + coord.y + ".png";
                    },
                    tileSize: new google.maps.Size(256, 256),
                    name: "Open Street Map",
                    maxZoom: 18
                }));

                vm.infoWindow = new google.maps.InfoWindow;
                vm.geocoder = new google.maps.Geocoder;
                vm.directionsService = new google.maps.DirectionsService();
                vm.directionsRenderer = new google.maps.DirectionsRenderer({
                    suppressMarkers: true,
                    //draggable: true
                });

                vm.directionsRenderer.setMap(vm.map);
                
                if (vm.navigator.geolocation) {

                    if (vm.cx === 0 && vm.cy === 0) {
                        await vm.getCurrentLocation();
                    }
                    else {
                        
                        vm.map.setCenter(vm.centerPosition);
                        vm.setMarker();
                    }
                    google.maps.event.addListener(vm.map, 'zoom_changed', function (arg) {
                        localStorage.setItem('zoom', this.zoom);
                    });

                } else {
                    // Browser doesn't support Geolocation
                    vm.handleLocationError(false, vm.infoWindow, vm.map.getCenter());
                    debugger;
                }
            },

            handleLocationError(browserHasGeolocation, infoWindow, pos) {
                const vm = this;

                vm.infoWindow.setPosition(pos);
                vm.infoWindow.setContent(browserHasGeolocation ?
                    'Error: The Geolocation service failed.' :
                    'Error: Your browser doesn\'t support geolocation.');
                vm.infoWindow.open(map);
            },

            geocodeLatLng(event, marker) {
                const vm = this;

                vm.geocoder.geocode({ 'location': marker.position }, function (results, status) {
                    if (status === 'OK') {
                        vm.$emit(event, results[0], { lat: vm.centerPosition.lat, lng: vm.centerPosition.lng });
                    } else {
                        window.alert('Geocoder failed due to: ' + status);
                    }
                });
            },

            calculateAndDisplayRoute() {
                const vm = this;

                vm.directionsService.route(
                    {
                        origin: vm.markerA.position,
                        destination: vm.markerB.position,
                        travelMode: google.maps.TravelMode.DRIVING,
                    },
                    (response, status) => {                        
                        if (status === "OK") {
                            var tripInfo = response.routes[0].legs[0];
                            vm.$emit('onCalculatedTrip', tripInfo);

                            vm.directionsRenderer.setDirections(response);
                        } else {
                            window.alert("Directions request failed due to " + status);
                        }
                    }
                );
            },
            async getCurrentLocation() {
                const vm = this;

                await vm.navigator.geolocation.getCurrentPosition(function (position) {

                    //if (!vm.fixed) {
                    vm.centerPosition = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };

                    //}

                    vm.map.setCenter(vm.centerPosition);

                    if (vm.showLocation) {

                        vm.setMarker();
                        //vm.marker = new google.maps.Marker({
                        //    draggable: vm.draggable,
                        //    //animation: google.maps.Animation.BOUNCE,

                        //    position: vm.centerPosition,
                        //    map: vm.map,
                        //    //title: "Your Current Location",
                        //    //label: {
                        //    //    text: 'You',
                        //    //    //fontFamily: 'Fontawesome',
                        //    //},
                        //});

                        ////var latlng = new google.maps.LatLng(40.748774, -73.985763);
                        //vm.marker.setPosition(vm.centerPosition);

                        //google.maps.event.addListener(vm.marker, 'dragend', function (event) {
                        //    vm.centerPosition = this.getPosition();
                        //    vm.geocodeLatLng();
                        //});
                    }

                });

            },

            setMarker() {
                const vm = this;

                if (vm.marker && vm.marker.setMap)
                    vm.marker.setMap(null);

                //vm.marker = new google.maps.Marker({
                //    draggable: vm.draggable,
                //    //animation: google.maps.Animation.BOUNCE,

                //    position: vm.centerPosition,
                //    map: vm.map,
                //    //title: "Your Current Location",
                //    //label: {
                //    //    text: 'You',
                //    //    //fontFamily: 'Fontawesome',
                //    //},
                //});

                ////var latlng = new google.maps.LatLng(40.748774, -73.985763);
                //vm.marker.setPosition(vm.centerPosition);

                //google.maps.event.addListener(vm.marker, 'dragend', function (event) {
                //    vm.centerPosition = this.getPosition();
                //    //    vm.geocodeLatLng();
                //});

                //  MARKER A                
                vm.markerA = new google.maps.Marker({
                    draggable: vm.draggable,
                    //animation: google.maps.Animation.BOUNCE,

                    //position:  vm.centerPosition,
                    position: new google.maps.LatLng(vm.startX, vm.startY),
                    //position: {
                    //    lat: vm.startX, lng: vm.startY
                    //},
                    map: vm.map,
                    //title: "Your Current Location",
                    label: {
                        text: 'Pick up',
                        //    //fontFamily: 'Fontawesome',
                    },
                });

                //var latlng = new google.maps.LatLng(40.748774, -73.985763);
                //vm.markerA.setPosition(vm.centerPosition);

                google.maps.event.addListener(vm.markerA, 'dragend', function (event) {
                    //vm.centerPosition = this.getPosition();
                    //vm.geocodeLatLng('onFromAddress', vm.markerA);
                    //vm.calculateAndDisplayRoute();
                });

                //  MARKER B
                vm.markerB = new google.maps.Marker({
                    draggable: vm.draggable,
                    //animation: google.maps.Animation.BOUNCE,

                    //position: vm.centerPosition,
                    position: new google.maps.LatLng(vm.endX, vm.endY),
                    //position: {
                    //    lat: vm.endX, lng: vm.endY
                    //},
                    map: vm.map,
                    //title: "Your Current Location",
                    label: {
                        text: 'Destination',
                        //    //fontFamily: 'Fontawesome',
                    },
                });

                //var latlng = new google.maps.LatLng(40.748774, -73.985763);
                //vm.markerB.setPosition(vm.centerPosition);

                google.maps.event.addListener(vm.markerB, 'dragend', function (event) {
                    //vm.centerPosition = this.getPosition();
                    //vm.geocodeLatLng('onToAddress', vm.markerB);
                    //vm.calculateAndDisplayRoute();
                });

                vm.calculateAndDisplayRoute();
            },

            placeMarkers(items, recenter) {
                //debugger;
                const vm = this;
                let markers = vm.markers;

                if (markers && markers.length > 0) {
                    markers.forEach(marker => marker.setMap(null));
                }

                if (items && items.length) {

                    markers = [];

                    items.forEach(item => {
                        var marker = new google.maps.Marker({
                            draggable: false,
                            //animation: google.maps.Animation.BOUNCE,

                            position: {
                                lat: item.geoX,
                                lng: item.geoY,
                            },
                            map: vm.map,
                            item: item,
                            title: `${item.firstName} ${item.lastName}`,
                            //label: {
                            //    text: 'You',
                            //    //fontFamily: 'Fontawesome',
                            //},
                        });

                        google.maps.event.addListener(marker, 'click', function (event) {

                            if (vm.markerClickAction)
                                vm.markerClickAction(this.item);
                        });

                        markers.push(marker);
                    });

                    vm.markers = markers;
                }


                //if (recenter) {
                vm.map.setCenter(vm.centerPosition);
                //}
            }
        }
    }
</script>