<template>
    <div style="position: relative;">
        <VSkeletonLoader :color="color" class="ml-skeleton" />
        <slot></slot>
    </div>
</template>

<script lang="ts">
export default {
    props: {
        color: {
            type: String,
            default: '#A7ADB2'
        }
    }
}
</script>

<style lang="scss" scoped>
.ml-skeleton {
    position: absolute;
    width: 100%;
    height: 100%;
    left: 0;
    top: 0;
}
</style>