<template>
    <VTextField
        :model-value="formattedValue"
        ref="txtFieldRef"
        :error="error"
        @blur="onInputBlur"
    />
</template>

<script lang="ts">
import Cleave from 'cleave.js';
import { PropType } from 'vue';

export default {
    props: {
        modelValue: {
            type: Number
        },

        rules: {
            type: Object as PropType<((v:number) => boolean)[]>,
            default: []
        }
    },

    mounted() {
        const inputEl = (this.$refs.txtFieldRef as any)?.$el.querySelector('input');
        if (inputEl) {
            this.cleave = new Cleave(inputEl, {
                numeral: true,
                numeralThousandsGroupStyle: 'thousand',
                numeralDecimalMark: ',',
                delimiter: '.',
                onValueChanged: (e) => {
                    if (e.target.rawValue === '') {
                        this.cleave?.setRawValue('0');
                        return;
                    }
                    this.formattedValue = e.target.value;
                    
                    const rawValue = parseFloat(this.cleave?.getRawValue() ?? '0');
                    this.rawValue = rawValue;

                    const errorRule = this.rules.find((method) => method(rawValue) === false);
                    this.error = errorRule !== undefined;
                }
            })
        }

        this.cleave?.setRawValue(this.modelValue?.toString() ?? '0');
        this.rawValue = this.modelValue ?? 0;

        this.$nextTick().then(() => {
            this.init = true;
        });
    },

    data() {
        return {
            rawValue: <number>0,
            formattedValue: <string>'0',

            cleave: <Cleave|undefined>undefined,
            error: <boolean>false,
            init: <boolean>false
        }
    },

    watch: {
        modelValue() {
            if (!this.init) {
                return;
            }
            this.cleave?.setRawValue(this.modelValue?.toString() ?? '0');
        }
    },

    methods: {
        onInputBlur() {
            this.$emit('update:modelValue', this.rawValue);
        }
    }
}
</script>