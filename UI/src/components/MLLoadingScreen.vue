<template>
    <VOverlay :contained="contained" persistent v-model="visible" class="ml-loading-screen" z-index="10">
        <VProgressCircular color="primary" :size="45" indeterminate>
            <span v-if="this.value !== undefined" style="font-size: 13px;">
                {{`${Math.round(this.value)}%`}}
            </span>
        </VProgressCircular>
    </VOverlay>
</template>

<script>
import { VOverlay } from 'vuetify/components';

export default {
    props: {
        value: {
            type: Number
        },
        activator: {
            type: String
        },
        contained: {
            type: Boolean,
            default: false
        }
    },

    data() {
        return {
            visible: true
        }
    }
}
</script>

<style lang="scss" scoped>
.ml-loading-screen {
    display: flex;
    align-items: center;
    justify-content: center;

    .ml-loading-screen-background {
        position: absolute;
        background-color: #A7ADB2;
        opacity: 0.7;
        width: 100%;
        height: 100%;
    }
}
</style>