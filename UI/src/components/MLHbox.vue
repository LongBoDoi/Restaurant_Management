<template>
    <div :class="[
        'ml-hbox',
        alignClassName,
        {'stretch-all': stretch}
    ]"
    >
        <slot></slot>
    </div>
</template>

<script lang="ts">
export default {
    name: 'MLHbox',

    //Props
    props: {
        align: {
            type: String
        },
        stretch: {
            type: Boolean
        }
    },

    computed: {
        alignClassName() {
            return this.align ? `align-items-${this.align}` : '';
        }
    }
};
</script>

<style lang="scss">
.ml-hbox {
    display: flex;
    flex-direction: row;
    position: relative;

    &.align-items-center {
        align-items: center;
    }

    &.stretch-all > * {
        height: 100%;
    }
}
</style>