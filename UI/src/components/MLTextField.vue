<template>
    <VTextField 
        :error-messages="errorMessages"
        :autofocus="autofocus"
        :type="type"
        ref="textField"
        density="compact"
        variant="outlined"
        :placeholder="placeholder"
        :rules="validationRules"

        :model-value="textValue"
        @update:model-value="updateValue"
    />
</template>

<script lang="ts">
export default {
    props: {
        modelValue: {
            type: String,
            default: ''
        },
        placeholder: {
            type: String
        },
        required: {
            type: Boolean
        },
        type: {
            type: String
        },
        autofocus: {
            type: Boolean
        }
    },

    data() {
        return {
            textValue: <string>this.modelValue,
            errorMessages: ''
        }
    },

    computed: {
        validationRules() {
            const rules = [];
            if (this.required) {
                rules.push((x:any) => !!x || 'This field is required.');
            }
            return rules;
        },

        isValid() {
            return (this.$refs.textField as any).isValid;
        }
    },

    methods: {
        getValue() {
            return this.textValue;
        },

        validate() {
            (this.$refs.textField as any).validate();
            return this.isValid;
        },

        focus() {
            (this.$refs.textField as any).focus();
        },

        updateValue(newValue:string) {
            this.textValue = newValue;
            this.$emit('update:modelValue', newValue);
        }
    }
}
</script>

<style lang="scss" scoped>
:deep() {
    .v-field {
        border-radius: 6px;
    }

    .v-input__details {
        display: none;
    }
}
</style>