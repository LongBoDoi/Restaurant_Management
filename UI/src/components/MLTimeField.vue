<template>
    <div>
    <VTextField
        :variant="(variant as any)"
        :density="compact ? 'compact' : 'default'"
        v-mask="'##:##'"
        :label="label"
        placeholder="HH:MM"
        append-inner-icon="mdi-clock-outline"
        :bg-color="bgColor"
        :color="color"
        v-model:model-value="txtValue"
        :error="invalid"
        ref="txtFieldRef"
        :hide-details="hideDetails"
        @click:append-inner="showTimePicker = true"
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
            v-model="txtValue"
            v-on:update:hour="handleSelectHour"
            v-on:update:model-value="handleUpdateFromPicker"
        />
    </VMenu>
    </div>
</template>

<script lang="ts">
import moment from 'moment';

export default {
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
            type: Object as PropType<((v:string) => boolean)[]>,
            default: []
        },
        minDate: {
            type: Date
        }
    },

    created() {
        this.configVariablesFromModelValue();
    },

    data() {
        return {
            txtValue: <string>'',
            dateValue: <Date>new Date(),
            showTimePicker: <boolean>false,

            hour: <number>0,
            minute: <number>0,

            invalid: <boolean>false,
        }
    },

    methods: {
        handleSelectHour(hour: number) {
            this.txtValue = `${String(hour).padStart(2, '0')}:${String(this.dateValue.getMinutes()).padStart(2, '0')}`;
        },

        handleUpdateFromPicker() {
        },

        /**
         * Config các biến để hiển thị từ giá trị Date truyền vào
         */
        configVariablesFromModelValue() {
            if (this.modelValue) {
                this.dateValue = (moment.utc(this.modelValue).local() as any)._d as Date;
                this.txtValue = this.$commonFunction.formatTime(this.dateValue, true);
            }
        }
    },

    computed: {
        timeRegex():RegExp {
            return /^(\d{2}):(\d{2})$/;
        },

        txtFieldRef():any {
            return this.$refs.txtFieldRef;
        },
    },

    watch: {
        modelValue() {
            this.configVariablesFromModelValue();
        },

        txtValue() {
            const match = this.txtValue.match(this.timeRegex);
            if (!match) {
                this.invalid = true;
                return;
            };

            const hour = match[1];
            const minute = match[2];

            if (isNaN(new Date(`2000-01-01 ${hour}:${minute}`) as any)) {
                this.invalid = true;
                return;
            };

            this.dateValue.setHours(parseInt(hour));
            this.dateValue.setMinutes(parseInt(minute));

            this.$emit('update:modelValue', moment(this.dateValue).utc().format());

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