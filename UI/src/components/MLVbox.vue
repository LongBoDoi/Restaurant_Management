<template>
    <div :class="[
        'ml-vbox',
        alignclass,
        {'stretch-all': stretch}
    ]"
        ref="div"
    >
        <slot></slot>
    </div>
</template>

<script lang="ts">
export default {
    name: 'MLVbox',

    //Props
    props: {
        align: {
            type: String
        },
        stretch: {
            type: Boolean
        }
    },

    methods: {
        getDiv() {
            return this.$refs.div;
        }
    },

    computed: {
        alignclass() {
            return this.align ? `align-items-${this.align}` : '';
        }
    }
};
</script>

<style lang="scss" scoped>
.ml-vbox {
    display: flex;
    flex-direction: column;
    position: relative;

    &.align-items-center {
        align-items: center;
    }

    &.stretch-all > * {
        width: 100%;
    }
}
</style>