<template>
    <VTextField
        :model-value="realModelValue"
        v-on:update:model-value="onInput"
        v-money="config"
    />
</template>

<script lang="ts">
import CommonValue from '@/common/CommonValue';

export default {
    props: {
        modelValue: {
            type: Number,
            default: 0
        },

        config: {
            type: Object as any,
            default: CommonValue.quantityConfig
        },
    },

    methods: {
        onInput(v: string) {
            const value = this.$commonFunction.getRealFloatValue(v);
            this.$emit('update:modelValue', value);
        }
    },

    computed: {
        realModelValue() {
            return (this.modelValue ?? 0) * Math.pow(10, this.config.precision ?? 0);
        }
    },
}
</script>