<template>
    <div>
    <VTextField
        :variant="(variant as any)"
        :density="compact ? 'compact' : 'default'"
        :label="label"
        placeholder="HH:MM"
        append-inner-icon="mdi-clock-outline"
        :bg-color="bgColor"
        :color="color"
        :model-value="formattedValue"
        :error="error"
        ref="txtFieldRef"
        :hide-details="hideDetails"
        @click:append-inner="showTimePicker = true"
        @blur="onInputBlur"
    />

    <VMenu
        v-model="showTimePicker"
        :close-on-content-click="false"
        :activator="txtFieldRef"
        offset="-8, 195"
    >
        <template #activator="{ props }">
            <!-- Activator connects the menu to the icon -->
            <span v-bind="props"></span>
        </template>
        <VTimePicker
            color="primary"
            ref="timePicker"
            ampm-in-title
            title=""
            :model-value="formattedValue"
            v-on:update:hour="handleSelectHour"
            v-on:update:model-value="handleUpdateFromPicker"
        />
    </VMenu>
    </div>
</template>

<script lang="ts">
import Cleave from 'cleave.js';
import moment from 'moment';

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
        hideDetails: {
            type: Boolean,
            default: false
        },
        label: {
            type: String
        },
        modelValue: {
            type: [String, Date]
        },
        variant: {
            type: String,
            default: 'underlined'
        },
        compact: {
            type: Boolean
        },
        bgColor: {
            type: String
        },
        color: {
            type: String
        },
        rules: {
            type: Object as PropType<((v: Date) => boolean|string)[]>,
            default: []
        },
        minDate: {
            type: Date
        },
        required: {
            type: Boolean
        },
    },

    mounted() {
        if (this.registerInput && typeof(this.registerInput) === 'function') {
            this.registerInput(this);
        }

        const inputEl = (this.$refs.txtFieldRef as any)?.$el.querySelector('input');
        if (inputEl) {
            this.cleave = new Cleave(inputEl, {
                time: true,
                timePattern: ['h', 'm'],
                onValueChanged: (e) => {
                    this.formattedValue = e.target.value;
                    this.rawValue = e.target.rawValue;

                    this.checkValid(e.target.value);

                    if (this.updateModelValueInCleave) {
                        this.$emit('update:modelValue', this.$commonFunction.getUTCDate(this.dateValue));
                        this.updateModelValueInCleave = false;
                    }
                }
            });
        }

        if (this.modelValue) {
            this.formattedValue = this.$commonFunction.formatTime(this.modelValue);
            this.dateValue = moment.utc(this.modelValue).local().toDate();
            this.originalDateValue = moment.utc(this.modelValue).local().toDate();
        }
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
            showTimePicker: <boolean>false,

            hour: <number>0,
            minute: <number>0,

            error: <boolean>false,
                
            cleave: <Cleave|undefined>undefined,
            updateModelValueInCleave: <boolean>false,
        }
    },

    methods: {
        handleSelectHour(hour: number) {
            this.updateModelValueInCleave = true;
            if (this.dateValue) {
                this.dateValue.setHours(hour);
                this.cleave?.setRawValue(moment(this.dateValue).format('HHmm'));
            } else {
                this.cleave?.setRawValue(`${String(hour).padStart(2, '0')}00`);
            }
        },

        handleUpdateFromPicker(value: string) {
            this.updateModelValueInCleave = true;
            this.cleave?.setRawValue(value.replace(':', ''));
        },

        onInputBlur() {
            if (this.error) {
                return;
            }
            this.$emit('update:modelValue', this.$commonFunction.getUTCDate(this.dateValue));
        },

        checkValid(value: string) {
            // Kiểm tra bỏ trống
            if (!value) {
                this.dateValue?.setHours(0);
                this.dateValue?.setHours(0);
                this.error = this.required;
                return;
            }

            // Kiểm tra định dạng HH:MM
            const match = value.match(this.timeRegex);
            if (!match) {
                this.dateValue = undefined;
                this.error = true;
                return;
            };

            // Kiểm tra thời gian hợp lệ
            const year = moment(this.dateValue ?? this.originalDateValue).format('YYYY');
            const month = moment(this.dateValue ?? this.originalDateValue).format('MM');
            const date = moment(this.dateValue ?? this.originalDateValue).format('DD');
            const hour = match[1];
            const minute = match[2];

            const dateValue = new Date(`${year}-${month}-${date} ${hour}:${minute}:00`);

            if (isNaN(dateValue as any)) {
                this.dateValue = undefined;
                this.error = true;
                return;
            };

            this.dateValue = dateValue;

            if (this.minDate && dateValue < this.minDate) {
                this.error = true;
                return
            }

            const errorRule = this.rules.find((method) => method(dateValue) === false);
            this.error = errorRule !== undefined;
        },

        validate() {
            this.checkValid(this.formattedValue);
            return !this.error;
        }
    },

    computed: {
        timeRegex():RegExp {
            return /^(\d{2}):(\d{2})$/;
        },

        txtFieldRef():any {
            return this.$refs.txtFieldRef;
        },

        currentDate() {
            return moment().toDate();
        }
    },

    watch: {
        modelValue() {
            if (this.modelValue) {
                this.formattedValue = this.$commonFunction.formatTime(this.modelValue);
                this.dateValue = moment.utc(this.modelValue).local().toDate();
                this.originalDateValue = moment.utc(this.modelValue).local().toDate();
                this.cleave?.setRawValue(moment(this.dateValue).format('HHmm'));
            } else {
                this.formattedValue = '';
                this.dateValue = undefined;
                this.originalDateValue = this.currentDate;
                this.cleave?.setRawValue('');
            }
        },
    }
}
</script>