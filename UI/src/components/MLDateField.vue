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
            :error="error"
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
                :min="new Date().toDateString()"
            />
        </VMenu>
    </div>
</template>

<script lang="ts">
import Cleave from 'cleave.js';
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
        const inputEl = (this.$refs.txtFieldRef as any)?.$el.querySelector('input');
        if (inputEl) {
            this.cleave = new Cleave(inputEl, {
                date: true,
                datePattern: ['d', 'm', 'Y'],
                onValueChanged: (e) => {
                    this.formattedValue = e.target.value;
                    this.rawValue = e.target.rawValue;

                    if (!e.target.value) {
                        this.dateValue = undefined;
                        this.error = this.required;
                        return;
                    }

                    // Kiểm tra định dạng DD/MM/YYYY
                    const match = e.target.value.match(this.dateRegex);
                    if (!match) {
                        this.dateValue = undefined;
                        this.error = true;
                        return;
                    };

                    // Kiểm tra ngày hợp lệ
                    const day = match[1];
                    const month = match[2];
                    const year = match[3];

                    const dateValue = new Date(`${year}-${month}-${day} ${this.dateValue?.getHours() ?? '00'}:${this.dateValue?.getMinutes() ?? '00'}:${this.dateValue?.getSeconds() ?? '00'}`);

                    if (isNaN(dateValue as any)) {
                        this.dateValue = undefined;
                        this.error = true;
                        return;
                    }

                    this.dateValue = dateValue;

                    if (this.minDate && dateValue < this.minDate) {
                        this.error = true;
                        return
                    }

                    const errorRule = this.rules.find((method) => method(dateValue) === false);
                    this.error = errorRule !== undefined;
                    // if (e.target.rawValue === '') {
                    //     e.target.value = '0';
                    //     this.cleave?.setRawValue('0');
                    // }
                    // this.value = e.target.value;
                    
                    // const rawValue = parseFloat(this.cleave?.getRawValue() ?? '0');
                    // this.$emit('update:modelValue', rawValue);

                    // const errorRule = this.rules.find((method) => method(rawValue) === false);
                    // this.error = errorRule !== undefined;
                }
            })
        }

        if (this.modelValue) {
            this.formattedValue = this.$commonFunction.formatDate(this.modelValue);
            this.dateValue = new Date(this.modelValue);
            this.originalDateValue = new Date(this.modelValue);
        }
        // this.cleave?.setRawValue(this.modelValue);
        // this.value = this.cleave?.getFormattedValue();
    },

    data() {
        return {
            formattedValue: <string>'',
            rawValue: <any>'',
            dateValue: <Date|undefined>undefined,
            originalDateValue: <Date>(moment().startOf('day') as any)._d as Date,

            cleave: <Cleave|undefined>undefined,

            showDatePicker: <boolean>false,
            error: <boolean>false
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
            if (this.error) {
                return;
            }
            this.$emit('update:modelValue', this.$commonFunction.getUTCDate(this.dateValue));
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
                this.dateValue = new Date(this.modelValue);
            }
            this.cleave?.setRawValue(this.cleave?.getRawValue());
        },
    },
}
</script>