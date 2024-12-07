<template>
    <MLVbox class="align-center">
        <span style="font-size: 1.5rem; font-weight: bold; text-align: center;">Bàn 1</span>
        <VCardText style="width: 75%; flex-grow: 0;">
            <div style="width: 100%; display: flex;">
                <span><b>Khách hàng:&nbsp</b>{{ order.CustomerName }}</span>
                <VSpacer />
                <span><b>Thời gian:&nbsp</b>{{ $commonFunction.formatDateTime(order.OrderDate) }}</span>
            </div>
        </VCardText>

        <div style="width: 100%; flex-grow: 1; padding: 0.625rem 1rem; display: flex; flex-direction: column;">
            <VDataTable
                hide-default-footer
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi" 
                :headers="[
                    {
                        title: 'STT',
                        width: '50',
                    },
                    {
                        title: 'Tên món',
                        value: 'PhoneNumber',
                        width: 250,
                    },
                    {
                        title: 'Số lượng',
                        value: 'Quantity',
                        align: 'end'
                    },
                    {
                        title: 'Thành tiền',
                        value: 'Price',
                        align: 'end'
                    },
                ]"
                :items="order.OrderDetails"
                style="width: 100%; flex-grow: 1;"
                :items-per-page-options="[10, 25, 50, 100]"
                :hover="true"
            >
                <template v-slot:item="{ item, index }">
                    <tr v-if="item" style="cursor: pointer;">
                        <td>
                            {{ index + 1 }}
                        </td>
                        <td>{{ item.MenuItem?.Name }}</td>
                        <td style="text-align: end;">{{ item.Quantity }}</td>
                        <td style="text-align: end;">{{ item.Price }} đ</td>
                    </tr>
                </template>
            </VDataTable>

            <VDivider />

            <MLHbox style="width: 100%; font-size: 1.25rem; padding: 0.5rem 0;">
                <VSpacer />

                <MLHbox style="flex-grow: 0.5;">
                    <b>Tổng tiền:</b>
                    <VSpacer />
                    <b>{{ order.TotalAmount ?? 0 }} đ</b>
                </MLHbox>
            </MLHbox>
        </div>
    </MLVbox>
</template>

<script lang="ts">
import { Order } from '@/models';
import { PropType } from 'vue';

export default {
    props: {
        order: {
            type: Object as PropType<Order>,
            required: true
        }
    }
}
</script>