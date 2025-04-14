<template>
    <VTextField
        v-model:model-value="dataValue"
        v-money="config"
        :rules="rules"
    >
        <template #append-inner>
        </template>
    </VTextField>
</template>

<script lang="ts">
import CommonValue from '@/common/CommonValue';
import { PropType } from 'vue';

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

        rules: {
            type: Object as PropType<((v:any) => boolean)[]>,
            default: []
        }
    },

    created() {
        this.stopWatchDataValue = true;
        this.dataValue = (this.modelValue ?? 0) * Math.pow(10, this.config.precision ?? 0);
    },

    data() {
        return {
            dataValue: <any>0,

            stopWatchModelValue: <boolean>false,
            stopWatchDataValue: <boolean>false,
        }
    },

    watch: {
        dataValue() {
            if (this.stopWatchDataValue) {
                this.stopWatchDataValue = false;
                return;
            }

            const value = this.$commonFunction.getRealFloatValue(this.dataValue.toString());
            this.stopWatchModelValue = true;
            this.$emit('update:modelValue', value);
        },

        modelValue() {
            if (this.stopWatchModelValue) {
                this.stopWatchModelValue = false;
                return;
            }

            const value = (this.modelValue ?? 0) * Math.pow(10, this.config.precision ?? 0);
            this.stopWatchDataValue = true;
            this.dataValue = value;
        }
    },
}
</script>