<template>
    <div>
    <VTextField
        :variant="(variant as any)"
        density="compact"
        v-mask="'##:##'"
        placeholder="HH:MM"
        label="Giờ"
        append-inner-icon="mdi-clock-outline"
        v-model:model-value="txtValue"
        :rules="validationRules"
        ref="txtFieldRef"
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
        modelValue: {
            type: [String, Date]
        },
        variant: {
            type: String,
            default: 'underlined'
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
            minute: <number>0
        }
    },

    methods: {
        handleSelectHour(hour: number) {
            this.dateValue.setHours(hour);

            this.$emit('update:modelValue', moment(this.dateValue).utc().format());
        },

        handleUpdateFromPicker() {
            const match = this.txtValue.match(this.timeRegex);
            if (!match) return false;

            const hour = match[1];
            const minute = match[2];

            this.dateValue.setHours(parseInt(hour));
            this.dateValue.setMinutes(parseInt(minute));

            this.$emit('update:modelValue', moment(this.dateValue).utc().format());
        },

        /**
         * Config các biến để hiển thị từ giá trị Date truyền vào
         */
        configVariablesFromModelValue() {
            this.dateValue = (moment.utc(this.modelValue).local() as any)._d as Date;
            this.txtValue = this.$commonFunction.formatTime(this.dateValue, true);
        }
    },

    computed: {
        timeRegex():RegExp {
            return /^(\d{2}):(\d{2})$/;
        },

        validationRules() {
            return [
                (str:string) => {
                    const match = str.match(this.timeRegex);
                    if (!match) return false;

                    const hour = match[1];
                    const minute = match[2];

                    return !isNaN(new Date(`2000-01-01 ${hour}:${minute}`) as any);
                }
            ]
        },

        txtFieldRef():any {
            return this.$refs.txtFieldRef;
        },

        timeZone() {
            return Intl.DateTimeFormat().resolvedOptions().timeZone;
        }
    },

    watch: {
        modelValue() {
            this.configVariablesFromModelValue();
        }
    }
}
</script>