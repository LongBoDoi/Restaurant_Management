<template>
    <VCard class="bg-gray-50 rounded-xl pa-6 shadow-md hover:shadow-lg transition-all duration-300" :disabled="loading">
        <template #loader>
            <VProgressLinear color="primary" indeterminate v-if="loading" />
        </template>

        <div class="flex justify-between items-center">
            <h3 class="text-lg font-bold">Order gần đây</h3>
            <button class="text-emerald-600 hover:text-emerald-700 flex items-center gap-1 text-sm transition duration-150" @click="expand=!expand" v-if="expandItems.length">
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="expand">chevron_left</span>
                {{ expand ? 'Thu gọn' : 'Xem tất cả' }}
                <span class="material-symbols-outlined text-sm" style="margin-top: 2px;" v-if="!expand">chevron_right</span>
            </button>
        </div>

        <div class="text-center mt-4" v-if="!data.length">
            Không có dữ liệu.
        </div>

        <VDataTable
            v-else
            class="mt-2"
            hide-default-footer
            :items="data"
            fixed-header
            :height="'fit-content'"
            style="background-color: transparent; border-radius: 0px;"
            no-data-text="Không có dữ liệu."
        >
            <template #headers>
                <tr>
                    <th class="font-medium">Bàn</th>
                    <th class="font-medium">Số món</th>
                    <th class="font-medium text-right">Tổng</th>
                    <th class="font-medium">Trạng thái</th>
                </tr>
            </template>

            <template #body>
                <tr v-for="item in displayItems" class="border-b bg-gray-50 hover:bg-gray-100 transition duration-150">
                    <td class="py-3">{{ item.TableName }}</td>
                    <td class="py-3">{{ item.OrderDetails.length }} món</td>
                    <td class="py-3 font-medium text-right">{{ $commonFunction.formatThousands(item.TotalAmount) }} đ</td>
                    <td class="py-3">
                        <span class="px-2 py-1 rounded-full text-xs" :class="getOrderStatusClass(item.Status)">
                            {{ getOrderStatusName(item.Status) }}
                        </span>
                    </td>
                </tr>
            </template>

            <template #body.append v-if="expand">
                <tr v-for="item in expandItems" class="border-b bg-gray-50 hover:bg-gray-100 transition-all duration-150">
                        <td class="py-3">{{ item.TableName }}</td>
                        <td class="py-3">{{ item.OrderDetails.length }} món</td>
                        <td class="py-3 font-medium text-right">{{ $commonFunction.formatThousands(item.TotalAmount) }} đ</td>
                        <td class="py-3">
                            <span class="px-2 py-1 rounded-full text-xs" :class="getOrderStatusClass(item.Status)">
                                {{ getOrderStatusName(item.Status) }}
                            </span>
                        </td>
                    </tr>
            </template>
        </VDataTable>
    </VCard>
</template>

<script lang="ts">
import { EnumOrderStatus } from '@/common/Enumeration';
import { Order } from '@/models';

export default {
    data() {
        return {
            loading: false,
            expand: <boolean>false,

            data: <Order[]>[]
        }
    },

    async created() {
        this.loading = true;

        const objectData = await this.$service.DashboardService.getRecentOrders();
        this.data = objectData;

        this.loading = false;
    },

    methods: {
        getOrderStatusName(orderStatus: EnumOrderStatus) {
            switch (orderStatus) {
                case EnumOrderStatus.Active:
                    return 'Đang hoạt động';
                case EnumOrderStatus.Paid:
                    return 'Đã thanh toán';
                case EnumOrderStatus.Canceled:
                    return 'Đã huỷ';
            }
        },

        getOrderStatusClass(orderStatus: EnumOrderStatus) {
            switch (orderStatus) {
                case EnumOrderStatus.Active:
                    return 'bg-blue-100 text-blue-800';
                case EnumOrderStatus.Paid:
                    return 'bg-green-100 text-green-800';
                case EnumOrderStatus.Canceled:
                    return 'bg-red-100 text-red-800';
            }
        }
    },

    computed: {
        displayItems() {
            return this.data.slice(0, 4);
        },

        expandItems() {
            return this.data.slice(4, this.data.length);
        }
    }
}
</script>