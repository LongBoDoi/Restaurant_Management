<template>
    <div>
        <VTextField
            :variant="(variant as any)"
            v-mask="'##/##/####'"
            placeholder="DD/MM/YYYY"
            :label="label"
            append-inner-icon="mdi-calendar-month-outline"
            :bg-color="bgColor"
            :base-color="baseColor"
            :color="color"
            v-model:model-value="txtValue"
            @click:append-inner="showDatePicker = true"
            :density="compact ? 'compact' : 'default'"
            ref="txtFieldRef"
            :hide-details="hideDetails"
            :error="invalid"
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
import { PropType } from 'vue';

export default {
    props: {
        label: {
            type: String,
        },
        compact: {
            type: Boolean
        },
        modelValue: {
            type: [Date, String]
        },
        variant: {
            type: String,
            default: 'underlined'
        },
        required: {
            type: Boolean
        },
        bgColor: {
            type: String
        },
        baseColor: {
            type: String
        },
        rules: {
            type: Object as PropType<((v:string) => boolean|string)[]>,
            default: []
        },
        hideDetails: {
            type: Boolean,
            default: true,
        },
        minDate: {
            type: Date
        },
        color: {
            type: String
        },
    },

    created() {
        this.configVariablesFromModelValue();
    },

    data() {
        return {
            txtValue: <string>'',

            showDatePicker: <boolean>false,
            dateValue: <Date>new Date(),

            invalid: <boolean>false
        }
    },

    methods: {
        handleUpdateValueFromDatePicker(value:Date) {
            this.txtValue = this.$commonFunction.formatDate(value);
        },

        configVariablesFromModelValue() {
            //Check value type
            if (this.modelValue) {
                this.dateValue = (moment.utc(this.modelValue).local() as any)._d as Date;
                this.txtValue = moment(this.dateValue).format('DD/MM/YYYY');
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
                (str:string):boolean|string => {
                    // Bắt buộc
                    console.log('Changed')
                    if (this.required && !str) {
                        this.$emit('update:modelValue', null);
                        return false;
                    }

                    // Kiểm tra định dạng DD/MM/YYYY
                    const match = str.match(this.dateRegex);
                    if (!match) {
                        this.$emit('update:modelValue', null);
                        return false;
                    };

                    // Kiểm tra ngày hợp lệ
                    const day = match[1];
                    const month = match[2];
                    const year = match[3];

                    const dateValue = new Date(`${year}-${month}-${day} 00:00:00`);
                    this.dateValue = dateValue;

                    if (isNaN(dateValue as any)) {
                        this.$emit('update:modelValue', null);
                        return false;
                    }
                    
                    this.$emit('update:modelValue', moment(dateValue).utc().format());

                    // Kiểm tra ngày nhỏ nhất
                    if (this.minDate && dateValue < this.minDate) {
                        return false;
                    }
                    
                    return true;
                }
            ].concat(this.rules);
        }
    },

    watch: {
        modelValue() {
            this.configVariablesFromModelValue();
        },

        txtValue() {
            // Bắt buộc
            if (this.required && !this.txtValue) {
                this.$emit('update:modelValue', null);
                this.invalid = true;
                return;
            }

            // Kiểm tra định dạng DD/MM/YYYY
            const match = this.txtValue.match(this.dateRegex);
            if (!match) {
                this.$emit('update:modelValue', null);
                this.invalid = true;
                return;
            };

            // Kiểm tra ngày hợp lệ
            const day = match[1];
            const month = match[2];
            const year = match[3];

            const dateValue = new Date(`${year}-${month}-${day} ${this.dateValue.getHours()}:${this.dateValue.getMinutes()}:00`);
            this.dateValue = dateValue;

            if (isNaN(dateValue as any)) {
                this.$emit('update:modelValue', null);
                this.invalid = true;
                return;
            }

            this.$emit('update:modelValue', moment(dateValue).utc().format());

            this.invalid = false;
        },

        dateValue() {
            // Kiểm tra ngày nhỏ nhất
            if (this.minDate && this.dateValue < this.minDate) {
                this.invalid = true;
                return;
            }

            this.invalid = false;
        }
    }
}
</script>