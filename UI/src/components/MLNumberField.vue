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
    inject: {
        registerInput: {
            from: 'registerInput',
            default: undefined
        },
        unregisterInput: {
            from: 'unregisterInput',
            default: undefined
        }
    },
    props: {
        modelValue: {
            type: Number||undefined
        },

        rules: {
            type: Object as PropType<((v:number) => boolean|string)[]>,
            default: []
        },

        nullable: {
            type: Boolean,
            default: false
        }
    },

    mounted() {
        if (this.registerInput && typeof(this.registerInput) === 'function') {
            this.registerInput(this);
        }
        const inputEl = (this.$refs.txtFieldRef as any)?.$el.querySelector('input');
        if (inputEl) {
            this.cleave = new Cleave(inputEl, {
                numeral: true,
                numeralThousandsGroupStyle: 'thousand',
                numeralDecimalMark: ',',
                delimiter: '.',
                onValueChanged: (e) => {
                    if (!this.nullable && e.target.rawValue === '') {
                        this.cleave?.setRawValue('0');
                        return;
                    }
                    this.formattedValue = e.target.value;
                    
                    const rawValue = parseFloat(this.cleave?.getRawValue() ?? this.defaultValue);
                    this.rawValue = rawValue;

                    this.checkValid(rawValue);
                }
            })
        }

        this.cleave?.setRawValue(this.modelValue?.toString() ?? this.defaultValue);
        this.rawValue = this.modelValue ?? 0;

        this.$nextTick().then(() => {
            this.init = true;
        });
    },

    unmounted() {
        if (this.unregisterInput && typeof(this.unregisterInput) === 'function') {
            this.unregisterInput(this);
        }
    },

    data() {
        return {
            rawValue: <number>0,
            formattedValue: <string>'',

            cleave: <Cleave|undefined>undefined,
            error: <boolean>false,
            errorMessage: <string>'',
            init: <boolean>false
        }
    },

    watch: {
        modelValue() {
            if (!this.init) {
                return;
            }
            this.cleave?.setRawValue(this.modelValue?.toString() ?? this.defaultValue);
        }
    },

    methods: {
        onInputBlur() {
            if (this.formattedValue === '') {
                this.$emit('update:modelValue', undefined);
                return;
            }
            this.$emit('update:modelValue', this.rawValue);
        },

        checkValid(number: number) {
            this.error = false;
            if (this.nullable && this.formattedValue === '') {
                this.error = false;
                return;
            }

            const ruleResults = this.rules.map((method) => method(number));
            const errorResult = ruleResults.find(r => r !== true);
            if (errorResult !== undefined) {
                this.error = true;
                
                if (typeof(errorResult) === 'string') {
                    this.errorMessage = errorResult;
                }
            }
        },

        validate() {
            this.checkValid(this.rawValue);
            return (!this.error || this.errorMessage || false);
        }
    },

    computed: {
        defaultValue() {
            return this.nullable ? '' : '0';
        }
    }
}
</script>