<template>
    <VTextField
        :model-value="value"
        ref="txtFieldRef"
    />
</template>

<script lang="ts">
import Cleave from 'cleave.js';

export default {
    props: {
        modelValue: {
            type: Number
        },
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
                        e.target.value = '0';
                        this.cleave?.setRawValue('0');
                    }
                    this.value = e.target.value;
                    
                    this.$emit('update:modelValue', parseInt(this.cleave?.getRawValue() ?? '0'));
                }
            })
        }

        this.cleave?.setRawValue(this.modelValue?.toString() ?? '');
        this.value = this.cleave?.getFormattedValue();
    },

    data() {
        return {
            value: <any>'',

            cleave: <Cleave|undefined>undefined
        }
    },
}
</script>