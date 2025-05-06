<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition-all duration-300" :disabled="loading" >
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-bold">Đặt bàn hôm nay</h3>
            <button class="text-emerald-600 hover:text-emerald-700 flex items-center gap-1 text-sm transition duration-150" @click="expanded=!expanded" v-if="expandItems.length">
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="expanded">chevron_left</span>
                {{ expanded ? 'Thu gọn' : 'Xem tất cả' }}
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="!expanded">chevron_right</span>
            </button>
        </div>

        <div v-if="!data.length" class="text-center">Không có dữ liệu.</div>
        
        <div v-else>
            <div class="space-y-3 overflow-hidden transition-all duration-300 pb-3">
                <div v-for="reservation in displayItems" class="flex pa-3 border rounded-lg hover:shadow-md transition duration-150" 
                    :class="`border-${getReservationStatusColor(reservation.Status)}-200 bg-${getReservationStatusColor(reservation.Status)}-50`"
                >
                    <div class="flex-shrink-0 flex flex-col items-center justify-center rounded-lg pa-3 mr-4" :class="`bg-${getReservationStatusColor(reservation.Status)}-100 text-${getReservationStatusColor(reservation.Status)}-800`">
                        <span class="text-xl font-bold">{{ $commonFunction.formatTime(reservation.ReservationDate) }}</span>
                    </div>
                    <div class="flex-1">
                        <div class="flex justify-between items-start">
                            <div>
                                <h4 class="font-medium">{{ reservation.Customer?.CustomerName ?? reservation.CustomerName }}</h4>
                                <p class="text-sm text-gray-600">{{ reservation.GuestCount }} người • {{ reservation.TableName }}</p>
                            </div>
                            <span class="px-2 py-1 rounded-full text-xs" :class="`bg-${getReservationStatusColor(reservation.Status)}-100 text-${getReservationStatusColor(reservation.Status)}-800`">
                                {{ getReservationStatus(reservation.Status) }}
                            </span>
                        </div>
                        <div v-if="false" class="flex gap-2 mt-2">
                            <button class="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                <span class="material-symbols-outlined text-sm">call</span>
                                Call
                            </button>
                            <button class="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                <span class="material-symbols-outlined text-sm">edit</span>
                                Edit
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <VExpandTransition>
                <div class="space-y-3" v-if="expanded">
                    <div v-for="reservation in expandItems" class="flex pa-3 border rounded-lg hover:shadow-md transition duration-150" 
                        :class="`border-${getReservationStatusColor(reservation.Status)}-200 bg-${getReservationStatusColor(reservation.Status)}-50`"
                    >
                        <div class="flex-shrink-0 flex flex-col items-center justify-center rounded-lg pa-3 mr-4" :class="`bg-${getReservationStatusColor(reservation.Status)}-100 text-${getReservationStatusColor(reservation.Status)}-800`">
                            <span class="text-xl font-bold">{{ $commonFunction.formatTime(reservation.ReservationDate) }}</span>
                        </div>
                        <div class="flex-1">
                            <div class="flex justify-between items-start">
                                <div>
                                    <h4 class="font-medium">{{ reservation.Customer?.CustomerName ?? reservation.CustomerName }}</h4>
                                    <p class="text-sm text-gray-600">{{ reservation.GuestCount }} người • {{ reservation.TableName }}</p>
                                </div>
                                <span class="px-2 py-1 rounded-full text-xs" :class="`bg-${getReservationStatusColor(reservation.Status)}-100 text-${getReservationStatusColor(reservation.Status)}-800`">
                                    {{ getReservationStatus(reservation.Status) }}
                                </span>
                            </div>
                            <div v-if="false" class="flex gap-2 mt-2">
                                <button class="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span class="material-symbols-outlined text-sm">call</span>
                                    Call
                                </button>
                                <button class="px-2 py-1 text-xs bg-white border border-gray-300 rounded hover:bg-gray-50 transition duration-150 flex items-center gap-1">
                                    <span class="material-symbols-outlined text-sm">edit</span>
                                    Edit
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </VExpandTransition>
        </div>
    </VCard>
</template>

<script lang="ts">
import { EnumReservationStatus } from '@/common/Enumeration';
import { Reservation } from '@/models';

export default {
    data() {
        return {
            loading: <boolean>false,
            data: <Reservation[]>[],

            expanded: <boolean>false
        }
    },

    async created() {
        this.loading = true;

        const objectData = await this.$service.DashboardService.getTodayReservations();
        this.data = objectData;

        this.loading = false;
    },

    methods: {
        /**
         * Tên trạng thái đặt bàn
         */
         getReservationStatus(status: EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumReservationStatus.Confirmed:
                    return 'Đã xác nhận';
                case EnumReservationStatus.Canceled:
                    return 'Đã huỷ';
                case EnumReservationStatus.Received:
                    return 'Đã nhận bàn'
            }
        },

        getReservationStatusColor(status: EnumReservationStatus) {
            switch (status) {
                case EnumReservationStatus.Pending:
                    return 'yellow';
                case EnumReservationStatus.Confirmed:
                    return 'blue';
                case EnumReservationStatus.Canceled:
                    return 'red';
                case EnumReservationStatus.Received:
                    return 'green';
            }
        }
    },

    computed: {
        allData() {
            return this.data;
        },

        displayItems() {
            return this.allData.slice(0, 4);
        },

        expandItems() {
            return this.allData.slice(4, this.allData.length);
        }
    }
}
</script>

<style lang="scss" scoped>
</style>