<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-2" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Tạo order</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div className="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm tên order/khách hàng..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />
            </div>

            <!-- Bảng dữ liệu -->
            <VDataTableServer
                sticky
                fixed-header
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as Order[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Bàn</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="">Thời gian</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="">Khách hàng</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="">Tổng tiền</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="">Trạng thái</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 128px; min-width: 128px;">Thao tác</th>
                    </tr>
                </template>

                <template #body v-if="loading">
                    <div class="absolute mt-4 text-center w-full">Đang tải dữ liệu...</div>
                </template>

                <template #no-data>
                    <div class="absolute mt-4 text-center w-full">Không có dữ liệu.</div>
                </template>

                <template v-slot:item="{ item, index }">
                    <span v-if="loading">Đang tải dữ liệu</span>
                    <tr v-if="item !== null" style="cursor: pointer;" class="hover:bg-green-50 transition-colors duration-150" 
                        :class="[
                            {'bg-gray-50' : index % 2 !== 0},
                            {'bg-green-100' : selectedIndex === index}
                        ]" 
                        @click="setSelectedIndex(index)" @dblclick="openDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.TableName }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatDateTime(item.OrderDate) }}</td>
                        <td class="py-4 px-6">{{ item.CustomerName }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatThousands(item.TotalAmount) }} đ</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="[
                                    {'bg-green-300 text-green-700' : item.Status === $enumeration.EnumOrderStatus.Paid },
                                    {'bg-blue-100 text-blue-800' : item.Status === $enumeration.EnumOrderStatus.Active },
                                    {'bg-red-100 text-red-700' : item.Status === $enumeration.EnumOrderStatus.Canceled }
                                ]"
                            >
                                {{ getOrderStatusName(item.Status) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-eye-outline" class="text-gray-700" :width="40" variant="text" @click="openDetail(item)" />

                                <MLHbox v-if="item.Status !== $enumeration.EnumOrderStatus.Paid && item.Status !== $enumeration.EnumOrderStatus.Canceled">
                                    <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                    <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" />
                                </MLHbox>
                            </MLHbox>
                        </td>
                    </tr>
                </template>

                <template #bottom>
                    <MLDataTableFooter :options="options" :total-count="totalCount" />
                </template>
            </VDataTableServer>
        </VCard>
    </VSheet>
</template>

<script lang="ts">
import { EnumEditMode, EnumOrderStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MLActionResult, Order } from '@/models';
import { orderStore } from '@/stores/orderStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getData();
    },

    data() {
        return {
            loading: <boolean>false,

            orderStatusFilter: <EnumOrderStatus>EnumOrderStatus.Active,
            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: ''
            }
        }
    },

    methods: {
        ...mapActions(orderStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormOrderDetail, newRecord);
        },

        /**
         * Lấy danh sách món
         */
        async getData() {
            this.loading = true;
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search);
            this.loading = false;
        },

        /**
         * Xử lý mở form chi tiết khách hàng
         */
        openDetail(record: Order) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormOrderDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: Order) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận huỷ order',
                Message: `Bạn có chắc chắn muốn huỷ order này không?`,
                ConfirmAction: async () => {
                    var saveObject:Order = {} as Order;
                    Object.assign(saveObject, item);

                    saveObject.Status = EnumOrderStatus.Canceled;
                    saveObject.EditMode = EnumEditMode.Edit;

                    const actionResult:MLActionResult = await this.$service.OrderService.saveChanges(saveObject);
                    if (actionResult.Success) {
                        Object.assign(item, actionResult.Data as Order);

                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Huỷ order thành công.',
                            Type: 'success'
                        });
                    }
                }
            });
        },

        getOrderServingTime(orderDate: Date) {
            const seconds:number = Math.floor(((this.currentTime as any) - (moment.utc(orderDate).local() as any)) / 1000);
            return this.$commonFunction.formatTimeBySecond(seconds);
        },

        getOrderStatusName(orderStatus: EnumOrderStatus) {
            switch (orderStatus) {
                case EnumOrderStatus.Active:
                    return 'Đang hoạt động';
                case EnumOrderStatus.Paid:
                    return 'Đã thanh toán';
                case EnumOrderStatus.Canceled:
                    return 'Đã huỷ';
            }
        }
    },

    computed: {
        ...mapState(orderStore, ['dataList', 'selectedIndex', 'totalCount']),

        currentTime():Date {
            return new Date()
        },
    },

    watch: {
        options: {
            handler() {
                this.getData();
            },
            deep: true
        }
    },
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>