<template>
    <b-navbar  fixed="bottom" type="dark" variant="dark" class="d-block d-sm-none">
        <b-navbar-nav align="center" :justified="true">
            <li v-for="menu in menus" class="nav-item">
                <router-link :to="menu.to" class="nav-link">
                    <span v-bind:class="menu.icon" class="mr-1"></span>
                    <span v-text="menu.label" class="d-none d-md-inline"></span>
                </router-link>
            </li>
        </b-navbar-nav>
    </b-navbar>
</template>
<script>
    import pageMixin from '../../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
            menus: Array
        },

        data() {
            return {

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

        }
    }
</script>