<template>
    <v-form ref="form" v-model="formValid">
        <slot />
    </v-form>
</template>

<script lang="ts">
export default {
    provide() {
        return {
            registerInput: this.registerInput,
            unregisterInput: this.unregisterInput,
        }
    },

    data() {
        return {
            formValid: true,
            inputs: <any>[],
            errorMessages: <string[]>[]
        }
    },

    methods: {
        registerInput(input: any) {
            this.inputs.push(input)
        },
        unregisterInput(input: any) {
            this.inputs = this.inputs.filter((i:any) => i !== input)
        },
        async validate() {
            const inputsValid = this.inputs.map((input:any) => input.validate());
            if (!inputsValid.every((valid: any) => valid === true)) {
                this.errorMessages = inputsValid.filter((validateResult: any) => typeof(validateResult) === 'string');

                return false;
            }

            const formValid = await ((this.$refs.form as any).validate());
            return formValid.valid;
        },

        getErrorMessages() {
            return this.errorMessages;
        }
    },
}
</script>