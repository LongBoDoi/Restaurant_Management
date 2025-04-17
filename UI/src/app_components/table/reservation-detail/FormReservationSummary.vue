<template>
    <div class="flex items-center mb-6">
        <div class="w-12 h-12 rounded-full flex items-center justify-center mr-4"
            :class="[`bg-${mainColor}-100`]"
        >
            <span class="material-symbols-outlined"
                :class="[`text-${mainColor}-700`]"
            >person</span>
        </div>
        <div>
            <h4 class="text-xl font-bold text-gray-800">{{ record.CustomerName }}</h4>
        </div>
        <span class="ml-auto px-3 py-1 rounded-full text-sm font-medium"
            :class="[`bg-${mainColor}-100`, `text-${mainColor}-700`]"
        >
            {{ statusName }}
        </span>
    </div>

    <div class="grid grid-cols-2 gap-4 mb-6">
        <div class="bg-gray-50 p-4 rounded-lg border border-gray-100 hover:border-red-300 transition-all duration-200">
            <p class="text-sm text-gray-500 mb-1">Ngày, giờ</p>
            <div class="flex items-center">
                <span class="material-symbols-outlined mr-2"
                    :class="[`text-${mainColor}-600`]"
                >event</span>
                <p class="font-medium">{{ $commonFunction.formatDateTime(record.ReservationDate) }}</p>
            </div>
        </div>

        <div class="bg-gray-50 p-4 rounded-lg border border-gray-100 hover:border-yellow-300 transition-all duration-200">
            <p class="text-sm text-gray-500 mb-1">Số người</p>
            <div class="flex items-center">
                <span class="material-symbols-outlined mr-2"
                    :class="[`text-${mainColor}-600`]"
                >group</span>
                <p class="font-medium">{{ record.GuestCount }} người</p>
            </div>
        </div>

        <div class="bg-gray-50 p-4 rounded-lg border border-gray-100 hover:border-yellow-300 transition-all duration-200">
            <p class="text-sm text-gray-500 mb-1">Bàn</p>
            <div class="flex items-center">
                <span class="material-symbols-outlined mr-2"
                    :class="[`text-${mainColor}-600`]"
                >chair</span>
                <p class="font-medium">{{ record.TableName }}</p>
            </div>
        </div>

        <div class="bg-gray-50 p-4 rounded-lg border border-gray-100 hover:border-yellow-300 transition-all duration-200">
            <p class="text-sm text-gray-500 mb-1">Số điện thoại</p>
            <div class="flex items-center">
                <span class="material-symbols-outlined mr-2"
                    :class="[`text-${mainColor}-600`]"
                >call</span>
                <p class="font-medium">{{ $commonFunction.formatPhoneNumber(record.CustomerPhoneNumber) }}</p>
            </div>
        </div>
    </div>

    <div class="p-4 rounded-lg border mb-6" :class="[`bg-${mainColor}-50`, `border-${mainColor}-100`]">
        <h4 class="font-medium mb-2 flex items-center" :class="[`text-${mainColor}-800`]">
            <span class="material-symbols-outlined mr-2">info</span>
            Yêu cầu đặc biệt
        </h4>
        <p :class="[`text-${mainColor}-700`]">
            {{ record.Note }}
        </p>
    </div>
</template>

<script lang="ts">
import { Reservation } from '@/models';
import { PropType } from 'vue';

export default {
    props: {
        record: {
            type: Object as PropType<Reservation>,
            required: true
        }
    },

    computed: {
        mainColor():string {
            switch (this.record.Status) {
                case this.$enumeration.EnumReservationStatus.Canceled:
                    return 'red';
                case this.$enumeration.EnumReservationStatus.Received:
                    return 'green';
            }

            return '';
        },

        statusName() {
            switch (this.record.Status) {
                case this.$enumeration.EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case this.$enumeration.EnumReservationStatus.Confirmed:
                    return 'Đã xác nhận';
                case this.$enumeration.EnumReservationStatus.Canceled:
                    return 'Đã huỷ';
                case this.$enumeration.EnumReservationStatus.Received:
                    return 'Đã nhận bàn';
            }
        }
    }
}
</script>