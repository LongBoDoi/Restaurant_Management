<template>
    <div>
        <VTextField
            :variant="(variant as any)"
            placeholder="DD/MM/YYYY"
            :label="label"
            append-inner-icon="mdi-calendar-month-outline"
            :bg-color="bgColor"
            :base-color="baseColor"
            :color="color"
            :model-value="formattedValue"
            @click:append-inner="showDatePicker = true"
            :density="compact ? 'compact' : 'default'"
            ref="txtFieldRef"
            :hide-details="hideDetails"
            :error="formatError || validateError"
            @blur="onInputBlur"
            @keypress.enter="onInputBlur"
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
                :model-value="dateValue"
                @update:model-value="handleUpdateValueFromDatePicker"
                scrollable
                color="primary"
                hide-header
                @click.stop
                :first-day-of-week="1"
                :min="minDate?.toDateString()"
            />
        </VMenu>
    </div>
</template>

<script lang="ts">
import Cleave from 'cleave.js';
import moment from 'moment';
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
            type: Object as PropType<((v: Date) => boolean|string)[]>,
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

    mounted() {
        if (this.registerInput && typeof(this.registerInput) === 'function') {
            this.registerInput(this);
        }

        const inputEl = (this.$refs.txtFieldRef as any)?.$el.querySelector('input');
        if (inputEl) {
            this.cleave = new Cleave(inputEl, {
                date: true,
                datePattern: ['d', 'm', 'Y'],
                onValueChanged: (e) => {
                    this.formattedValue = e.target.value;
                    this.rawValue = e.target.rawValue;

                    this.checkValid(e.target.value);
                }
            })
        }

        if (this.modelValue) {
            this.formattedValue = this.$commonFunction.formatDate(this.modelValue);
            this.dateValue = moment.utc(this.modelValue).local().toDate();
            this.originalDateValue = moment.utc(this.modelValue).local().toDate();
        }
        // this.cleave?.setRawValue(this.modelValue);
        // this.value = this.cleave?.getFormattedValue();
    },

    unmounted() {
        if (this.unregisterInput && typeof(this.unregisterInput) === 'function') {
            this.unregisterInput(this);
        }
    },

    data() {
        return {
            formattedValue: <string>'',
            rawValue: <any>'',
            dateValue: <Date|undefined>undefined,
            originalDateValue: <Date>(moment().startOf('day') as any)._d as Date,

            cleave: <Cleave|undefined>undefined,

            showDatePicker: <boolean>false,
            formatError: <boolean>false,
            validateError: <boolean>false,
            errorMessage: <string>''
        }
    },

    methods: {
        handleUpdateValueFromDatePicker(value: Date) {
            value.setHours(this.originalDateValue.getHours());
            value.setMinutes(this.originalDateValue.getMinutes());
            value.setSeconds(this.originalDateValue.getSeconds());

            this.$emit('update:modelValue', this.$commonFunction.getUTCDate(value));

            this.cleave?.setRawValue(moment(value).format('DDMMYYYY'));
        },

        onInputBlur() {
            if (this.formatError) {
                return;
            }
            this.$emit('update:modelValue', this.dateValue ? this.$commonFunction.getUTCDate(this.dateValue) : undefined);
        },

        checkValid(value: string) {
            this.formatError = false;
            this.validateError = false;
            if (!value) {
                this.dateValue = undefined;
                this.formatError = this.required;
                return;
            }

            // Kiểm tra định dạng DD/MM/YYYY
            const match = value.match(this.dateRegex);
            if (!match) {
                this.dateValue = undefined;
                this.formatError = true;
                return;
            };

            // Kiểm tra ngày hợp lệ
            const day = match[1];
            const month = match[2];
            const year = match[3];

            const dateValue = new Date(`${year}-${month}-${day} ${this.dateValue?.getHours() ?? '00'}:${this.dateValue?.getMinutes() ?? '00'}:${this.dateValue?.getSeconds() ?? '00'}`);

            if (isNaN(dateValue as any)) {
                this.dateValue = undefined;
                this.formatError = true;
                return;
            }

            this.dateValue = dateValue;

            if (this.minDate && dateValue < this.minDate) {
                this.validateError = true;
                return
            }

            const ruleResults = this.rules.map((method) => method(dateValue));
            const errorResult = ruleResults.find(r => r !== true);
            if (errorResult !== undefined) {
                this.validateError = true;
                
                if (typeof(errorResult) === 'string') {
                    this.errorMessage = errorResult;
                }
            }
        },

        validate() {
            this.checkValid(this.formattedValue);
            return (!this.formatError && !this.validateError) || this.errorMessage || false;
        }
    },

    computed: {
        txtFieldRef():any {
            return this.$refs.txtFieldRef;
        },

        dateRegex():RegExp {
            return /^(\d{2})\/(\d{2})\/(\d{4})$/;
        },
    },

    watch: {
        modelValue() {
            if (this.modelValue) {
                this.formattedValue = this.$commonFunction.formatDate(this.modelValue);
                this.dateValue = moment.utc(this.modelValue).local().toDate();
                this.originalDateValue = moment.utc(this.modelValue).local().toDate();
                this.cleave?.setRawValue(moment.utc(this.modelValue).local().format('DDMMYYYY'));
            } else {
                this.formattedValue = '';
                this.dateValue = undefined;
                this.originalDateValue = (moment().startOf('day') as any)._d as Date;
                this.cleave?.setRawValue('');
            }
        },
    },
}
</script>