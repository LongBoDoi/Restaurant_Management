<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="height: calc(100vh - 64px); padding: 16px 32px 16px 16px; display: flex; flex-direction: column;">
        <div style="flex-shrink: 0;" class="d-flex flex-column">
        <VLabel style="font-weight: bold; font-size: 2rem;">Order</VLabel>

        <VBtn width="fit-content" class="mt-4" color="primary" prepend-icon="mdi-plus" @click="handleAddNewClick">Tạo order</VBtn>

        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <VTabs v-model:model-value="orderStatusFilter" color="primary" @update:model-value="getDataList()">
            <VTab :value="$enumeration.EnumOrderStatus.Active">Đang hoạt động</VTab>
            <VTab :value="$enumeration.EnumOrderStatus.Paid">Đã thanh toán</VTab>
        </VTabs>
        </div>
        
        <VSpacer style="height: 16px; flex-shrink: 0; flex-grow: 0;" />

        <VTabsWindow v-model:model-value="orderStatusFilter" style="overflow: hidden; height: 100%;">
            <VTabsWindowItem :value="$enumeration.EnumOrderStatus.Active" style="display: flex; flex-grow: 1; flex-direction: column; overflow: hidden;">
                <VProgressLinear color="primary" indeterminate v-if="loading" />

                <span v-else-if="!dataList.length" class="text-center">Không có dữ liệu.</span>

                <MLVbox style="flex-grow: 1; overflow: hidden;">
                    <div class="pl-2 pt-4" style="display: ruby; flex-grow: 1; overflow-y: auto;">
                        <VCard v-for="o, index in dataList" min-width="200" width="15%" link class="mr-4 mb-4" @click="setSelectedIndex(index); handleOpenFormDetail();">
                            <template v-if="o">
                                <VCardItem class="d-flex" style="flex-direction: column;">
                                    <VIcon color="primary" size="140" icon="mdi-food-fork-drink" />
                                </VCardItem>
                                <VCardText>
                                    <div style="display: flex; flex-direction: column; align-items: center;">
                                        <b style="font-size: 1.5rem;">{{ o.OrderName }}</b>
                                        <span v-if="o.CustomerName"><VIcon class="mr-1" icon="mdi-account" color="primary" />{{ o.CustomerName }}</span>
                                    </div>
                                </VCardText>
                                <VCardActions>
                                    <VIcon color="primary" icon="mdi-clock-outline" />
                                    {{ getOrderServingTime(o.OrderDate) }}
                                </VCardActions>
                            </template>
                        </VCard>
                    </div>
                </MLVbox>

                <VDataTableServer
                    :items-length="totalCount"
                    items-per-page-text="Số bản ghi"
                    :items-per-page-options="[10, 25, 50, 100]"
                    v-model:options="options"
                    :items="dataList"
                    @update:options="getDataList"
                    sticky
                >
                    <template v-slot:headers />
                    <template v-slot:body>
                    </template>
                </VDataTableServer>
            </VTabsWindowItem>

            <VTabsWindowItem :value="$enumeration.EnumOrderStatus.Paid" style="display: flex; flex-grow: 1;">
                <VDataTableServer
                    :items-length="totalCount"
                    :loading="loading"
                    loading-text="Đang tải dữ liệu..." 
                    no-data-text="Không có dữ liệu" 
                    items-per-page-text="Số bản ghi" 
                    :headers="[
                        {
                            title: 'Tên order',
                            value: 'OrderName',
                        },
                        {
                            title: 'Khách hàng',
                            value: 'CustomerName',
                            width: 250,
                        },
                        {
                            title: 'Thời gian',
                            value: 'OrderDate',
                            align: 'center'
                        },
                        {
                            title: 'Tổng tiền',
                            value: 'TotalAmount',
                            align: 'end'
                        },
                    ]"
                    :items="(dataList as Order[])"
                    style="flex-grow: 1; height: 100%;"
                    :items-per-page-options="[10, 25, 50, 100]"
                    :hover="true"
                    v-model:options="options"
                    @update:options="getDataList"
                >
                    <template v-slot:item="{ item, index }">
                    <tr v-if="item !== null" style="cursor: pointer;" :class="{'selected-row': index === selectedIndex}" @click="setSelectedIndex(index)" @dblclick="handleOpenFormDetail">
                        <!-- <td>
                        <v-checkbox
                            :value="item"
                            hide-details
                            color="primary"
                        ></v-checkbox>
                        </td> -->
                        <td>{{ item.OrderName }}</td>
                        <td>{{ item.CustomerName }}</td>
                        <td style="text-align: center;">{{ $commonFunction.formatDateTime(item.OrderDate) }}</td>
                        <td style="text-align: end;">{{ item.TotalAmount }} đ</td>
                    </tr>
                    </template>
                </VDataTableServer>
            </VTabsWindowItem>
        </VTabsWindow>
    </VSheet>
</template>

<script lang="ts">
import { EnumOrderStatus, EnumRole } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Order } from '@/models';
import { orderStore } from '@/stores/orderStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getDataList();
    },

    data() {
        return {
            loading: <boolean>false,

            orderStatusFilter: <EnumOrderStatus>EnumOrderStatus.Active,
            options: <any>{
                page: 1,
                itemsPerPage: 10
            }
        }
    },

    methods: {
        ...mapActions(orderStore as any, ['addNewRecord', 'setSelectedIndex', 'getOrdersByStatus']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = {
                EditMode: this.$enumeration.EnumEditMode.Add
            } as Order;
            
            EventBus.emit(this.$eventName.ShowFormOrderDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getDataList() {
            this.loading = true;
            await this.getOrdersByStatus(this.orderStatusFilter, this.options.page, this.options.itemsPerPage);
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        handleOpenFormDetail() {
            const selectedRecord = this.dataList[this.selectedIndex];
            selectedRecord.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormOrderDetail, selectedRecord);
        },

        getRoleName(role: EnumRole) {
            switch (role) {
                case EnumRole.Manager:
                    return 'Quản lý';
                case EnumRole.Cashier:
                    return 'Thu ngân';
                case EnumRole.Waiter:
                    return 'Phục vụ';
            }
        },

        getOrderServingTime(orderDate: Date) {
            const seconds:number = Math.floor(((this.currentTime as any) - (moment.utc(orderDate).local() as any)) / 1000);
            return this.$commonFunction.formatTimeBySecond(seconds);
        }
    },

    computed: {
        ...mapState(orderStore, ['dataList', 'selectedIndex', 'totalCount']),

        currentTime():Date {
            return new Date()
        },
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>