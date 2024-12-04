<template>
    <div>
        <VTextField
            :variant="(variant as any)"
            v-mask="'##/##/####'"
            placeholder="DD/MM/YYYY"
            label="Ngày"
            append-inner-icon="mdi-calendar-month-outline"
            v-model:model-value="txtValue"
            @click:append-inner="showDatePicker = true"
            density="compact"
            ref="txtFieldRef"
            :rules="validationRules"
        />
        
        <VMenu
            v-model="showDatePicker"
            :close-on-content-click="false"
            :activator="txtFieldRef"
            offset="-8, 40"
        >
            <template #activator="{ props }">
                <!-- Activator connects the menu to the icon -->
                <span v-bind="props"></span>
            </template>
            <VDatePicker
                v-model="dateValue"
                @update:model-value="handleUpdateValueFromDatePicker"
                scrollable
                color="primary"
                hide-header
                @click.stop
                :first-day-of-week="1"
                :min="new Date().toDateString()"
            />
        </VMenu>
    </div>
</template>

<script lang="ts">
import moment from 'moment';

export default {
    props: {
        modelValue: {
            type: [Date, String]
        },
        variant: {
            type: String,
            default: 'underlined'
        }
    },

    created() {
        //Check value type
        if (this.modelValue) {
            this.dateValue = (moment.utc(this.modelValue).local() as any)._d as Date;
            this.txtValue = moment(this.dateValue).format('DD/MM/YYYY');
        }
    },

    data() {
        return {
            txtValue: <string>'',

            showDatePicker: <boolean>false,
            dateValue: <Date>new Date()
        }
    },

    methods: {
        handleUpdateValueFromDatePicker(value:Date) {
            this.txtValue = this.$commonFunction.formatDate(value);

            if (this.dateValue) {
                this.$emit('update:modelValue', moment(this.dateValue).utc().format());
            }
        }
    },

    computed: {
        txtFieldRef():any {
            return this.$refs.txtFieldRef;
        },

        dateRegex():RegExp {
            return /^(\d{2})\/(\d{2})\/(\d{4})$/;
        },

        validationRules() {
            return [
                (str:string) => {
                    const match = str.match(this.dateRegex);
                    if (!match) return false;

                    const day = match[1];
                    const month = match[2];
                    const year = match[3];

                    const dateValue = new Date(`${year}-${month}-${day}`);
                    if (!isNaN(dateValue as any)) {
                        return true;
                    }

                    return false;
                }
            ]
        }
    }
}
</script>