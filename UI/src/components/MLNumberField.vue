<template>
    <VTextField
        v-model:model-value="dataValue"
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
        }
    },

    created() {
        this.dataValue = (this.modelValue ?? 0) * Math.pow(10, this.config.precision ?? 0);
    },

    data() {
        return {
            dataValue: <any>0
        }
    },

    watch: {
        dataValue() {
            this.$emit('update:modelValue', this.$commonFunction.getRealFloatValue(this.dataValue.toString()));
        }
    },
}
</script>