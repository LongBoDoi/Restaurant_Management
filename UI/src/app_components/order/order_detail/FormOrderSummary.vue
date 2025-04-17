<template>
    <div>
        <div class="grid grid-cols-3 gap-6 mb-6">
            <!-- Khách hàng -->
            <div class="border rounded-lg p-4 hover:shadow-md transition duration-200">
                <h4 class="text-sm text-gray-500 mb-1">Khách hàng</h4>
                <p class="font-bold">{{ order.CustomerName }}</p>

                <div v-if="false">
                    <p class="text-sm text-gray-600">Regular customer</p>
                    <p class="text-xs text-gray-500 mt-2">8 orders this month</p>
                </div>
            </div>

            <!-- Thông tin order -->
            <div class="border rounded-lg p-4 hover:shadow-md transition duration-200">
                <h4 class="text-sm text-gray-500 mb-1">Thông tin order</h4>
                <p class="font-bold">{{ order.TableName }}</p>
                <p class="text-sm text-gray-600">{{ order.OrderDetails?.length ?? 0 }} món</p>
                <p class="text-xs text-gray-500 mt-2">{{ $commonFunction.formatDateTime(order.OrderDate) }}</p>
            </div>

            <!-- Trạng thái -->
            <div class="border rounded-lg p-4 hover:shadow-md transition duration-200">
                <h4 class="text-sm text-gray-500">Trạng thái</h4>
                <div class="mt-2 flex items-center gap-1 mb-2">
                    <span class="px-2 py-0.5 rounded-full text-xs" :class="orderStatusClass">
                        {{ orderStatusName }}
                    </span>
                </div>
                <div v-if="order.Status === $enumeration.EnumOrderStatus.Paid">
                    <p class="font-bold">{{ $commonFunction.formatThousands(order.PaidAmount) }} đ</p>
                    <p class="text-sm text-gray-600">{{ orderPaymentMethodName }}</p>
                </div>
            </div>
        </div>

        <div class="mb-6">
            <!-- Món order -->
            <h4 class="font-bold mb-3">Món Order</h4>
            <div class="border rounded-lg overflow-hidden">
                <table class="w-full text-left">
                    <thead>
                        <tr class="bg-gray-50 border-b">
                            <th class="px-4 py-3 font-medium text-sm">Tên món</th>
                            <th class="px-4 py-3 font-medium text-sm text-right">Số lượng</th>
                            <th class="px-4 py-3 font-medium text-sm text-right">Đơn giá</th>
                            <th class="px-4 py-3 font-medium text-sm text-right">Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="orderDetail in order.OrderDetails" class="border-b hover:bg-gray-50 transition duration-150">
                            <td class="px-4 py-3">
                                <div class="flex items-center gap-3">
                                    <img
                                        v-if="orderDetail.MenuItem?.ImageUrl"
                                        :src="$commonFunction.getImageUrl(orderDetail.MenuItem?.ImageUrl)"
                                        class="w-10 h-10 object-cover rounded-md"
                                    />
                                    <div v-else class="bg-gray-200 w-10 h-10 rounded-md flex items-center justify-center">
                                        <VIcon icon="mdi-food" class="text-gray-400" :size="16" />
                                    </div>
                                    <div>
                                        <p class="font-medium">{{ orderDetail.MenuItem?.Name ?? orderDetail.MenuItemName }}</p>
                                        <p class="text-xs text-gray-500">{{ orderDetail.MenuItem?.Description }}</p>
                                    </div>
                                </div>
                            </td>
                            <td class="px-4 py-3 text-right">{{ $commonFunction.formatThousands(orderDetail.Quantity) }}</td>
                            <td class="px-4 py-3 text-right">{{ $commonFunction.formatThousands(orderDetail.Price) }} đ</td>
                            <td class="px-4 py-3 font-bold text-right">{{ $commonFunction.formatThousands(orderDetail.Amount) }} đ</td>
                        </tr>
                    </tbody>
                </table>
                
                <!-- Tổng tiền -->
                <div class="bg-gray-50">
                    <div style="max-width: 400px;" class="ml-auto pa-4 grid gap-2">
                        <div class="grid grid-cols-2 gap-2">
                            <div>Tiền hàng:</div>
                            <div class="text-right">{{ $commonFunction.formatThousands(order.NetAmount) }} đ</div>
                        </div>
                        <div class="grid grid-cols-2 gap-2">
                            <div>Tip:</div>
                            <div class="text-right">{{ $commonFunction.formatThousands(order.TipAmount) }} đ</div>
                        </div>
                        <div class="grid grid-cols-2 gap-2 font-bold text-lg">
                            <div>Tổng:</div>
                            <div class="text-right">{{ $commonFunction.formatThousands(order.TotalAmount) }} đ</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Ghi chú -->
        <div class="grid grid-cols-2 gap-6 mb-6">
            <div>
                <h4 className="font-medium mb-2">Ghi chú</h4>
                <div class="border pa-4 rounded-lg" style="min-height: 100px;">
                    {{ order.SpecialRequest }}
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { EnumOrderStatus, EnumPaymentMethod } from '@/common/Enumeration';
import { Order } from '@/models';

export default {
    props: {
        order: {
            type: Object as PropType<Order>,
            required: true
        },
    },

    computed: {
        orderStatusName() {
            switch (this.order.Status) {
                case EnumOrderStatus.Paid:
                    return 'Đã thanh toán';
                case EnumOrderStatus.Canceled:
                    return 'Đã huỷ';
            }
        },

        orderStatusClass() {
            switch (this.order.Status) {
                case EnumOrderStatus.Paid:
                    return 'bg-green-100 text-green-700';
                case EnumOrderStatus.Canceled:
                    return 'bg-red-100 text-red-700';
            }
        },

        orderPaymentMethodName() {
            switch (this.order.PaymentMethod) {
                case EnumPaymentMethod.Cash:
                    return 'Tiền mặt';
                case EnumPaymentMethod.Transaction:
                    return 'Chuyển khoản';
                case EnumPaymentMethod.Card:
                    return 'Thẻ';
            }
        }
    }
}
</script>