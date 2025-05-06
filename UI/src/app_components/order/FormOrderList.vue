<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2 overflow-hidden">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-2" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Tạo order</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField color="primary" density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" style="max-width: 320px;" hide-details placeholder="Tìm bàn/khách hàng..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                    @blur="options.search = $event.target.value;"
                />

                <VSelect
                    :items="[
                        {
                            Text: 'Tất cả order',
                            Value: -1
                        },
                        {
                            Text: 'Đang hoạt động',
                            Value: $enumeration.EnumOrderStatus.Active
                        },
                        {
                            Text: 'Đã thanh toán',
                            Value: $enumeration.EnumOrderStatus.Paid
                        },
                        {
                            Text: 'Đã huỷ',
                            Value: $enumeration.EnumOrderStatus.Canceled
                        }
                    ]"
                    item-title="Text"
                    item-value="Value"
                    :return-object="false"
                    v-model:model-value="options.orderStatus"

                    density="compact"
                    variant="outlined"
                    hide-details
                    color="primary"
                    style="max-width: 200px;"
                />

                <MLFilterPopup :filter-count="lstFilters.length" v-on:reset-filter="handleResetFilters" v-on:apply-filter="handleApplyFilters" width="365">
                    <div class="space-y-4">
                        <div>
                            <label class="block font-medium text-gray-700">Khoảng thời gian</label>
                            <MLHbox class="space-x-2 mt-1">
                                <div class="flex-1">
                                    <div class="text-sm font-medium text-gray-500">Từ</div>
                                    <MLDateField
                                        variant="outlined"
                                        compact
                                        hide-details
                                        color="primary"
                                        nullable
                                        style="flex-grow: 0.5;"

                                        v-model="orderFromDate"
                                        ref="customInput"
                                        :rules="[(value: Date) => {
                                            return (orderToDate === undefined || $moment(value).isSameOrBefore(orderToDate)) || 'Thời gian bắt đầu không được lớn hơn thời gian kết thúc';
                                        }]"
                                    />
                                </div>
                                
                                <div class="flex-1">
                                    <div class="text-sm font-medium text-gray-500">Đến</div>
                                    <MLDateField
                                        variant="outlined"
                                        compact
                                        hide-details
                                        color="primary"
                                        nullable
                                        style="flex-grow: 0.5;"
                                        
                                        v-model="orderToDate"
                                        :rules="[(value: Date) => {
                                            return (orderFromDate === undefined || $moment(value).isSameOrAfter(orderFromDate)) || 'Thời gian bắt đầu không được lớn hơn thời gian kết thúc';
                                        }]"
                                    />
                                </div>
                            </MLHbox>
                        </div>

                        <div>
                            <label class="block font-medium text-gray-700">Tổng tiền</label>
                            <div class="flex items-center space-x-2 mt-1">
                                <MLNumberField
                                    placeholder="Tối thiểu"
                                    variant="outlined"
                                    density="compact"
                                    hide-details
                                    suffix="đ"
                                    color="primary"
                                    nullable
                                    style="flex-grow: 0.5;"
                                    
                                    v-model="minAmount"
                                    :rules="[
                                        $commonValue.positiveNumberRule,
                                        (value: number) => {
                                            return (maxAmount === undefined || value <= maxAmount) || 'Số tiền tối thiểu không được lớn hơn số tiền tối đa';
                                        }
                                    ]"
                                />
                                <MLNumberField
                                    placeholder="Tối đa"
                                    variant="outlined"
                                    density="compact"
                                    hide-details
                                    suffix="đ"
                                    color="primary"
                                    nullable
                                    style="flex-grow: 0.5;"
                                    
                                    v-model="maxAmount"
                                    :rules="[
                                        $commonValue.positiveNumberRule,
                                        (value: number) => {
                                            return minAmount === undefined || value >= minAmount;
                                        }
                                    ]"
                                />
                            </div>
                        </div>
                    </div>
                </MLFilterPopup>

                <MLSortPopup :items="lstSortOptions" v-model:model-value="options.sort" />
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
                                <MLHbox v-if="item.Status !== $enumeration.EnumOrderStatus.Paid && item.Status !== $enumeration.EnumOrderStatus.Canceled">
                                    <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                    <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" />
                                </MLHbox>
                                <VBtn v-else icon="mdi-eye-outline" class="text-gray-700" :width="40" variant="text" @click="openDetail(item)" />
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
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { orderStore } from '@/stores/orderStore';
import moment from 'moment';
import { mapActions, mapState } from 'pinia';

export default {
    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                orderStatus: <EnumOrderStatus>EnumOrderStatus.Active,
                sort: ''
            },

            orderFromDate: <Date|undefined>undefined,
            orderToDate: <Date|undefined>undefined,
            minAmount: <number|undefined>undefined,
            maxAmount: <number|undefined>undefined,

            lstFilters: <MLFilterCondition[]>[],
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
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search, this.filterJson, this.options.sort);
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

        handleResetFilters() {
            this.orderFromDate = undefined;
            this.orderToDate = undefined;
            this.minAmount = undefined;
            this.maxAmount = undefined;

            this.lstFilters = [];
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.orderFromDate) {
                this.lstFilters.push({
                    Name: 'OrderDate',
                    Operator: '>=',
                    Value: this.orderFromDate
                } as MLFilterCondition);
            }
            
            if (this.orderToDate) {
                this.lstFilters.push({
                    Name: 'OrderDate',
                    Operator: '<',
                    Value: moment(this.orderToDate).add(1, 'days')
                } as MLFilterCondition);
            }

            if (this.minAmount) {
                this.lstFilters.push({
                    Name: 'TotalAmount',
                    Operator: '>=',
                    Value: this.minAmount
                } as MLFilterCondition);
            }

            if (this.maxAmount) {
                this.lstFilters.push({
                    Name: 'TotalAmount',
                    Operator: '<=',
                    Value: this.maxAmount
                } as MLFilterCondition);
            }

            this.getData();
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
        },
    },

    computed: {
        ...mapState(orderStore, ['dataList', 'selectedIndex', 'totalCount']),

        filterJson() {
            var result = '';

            var filterArray = [];
            if (this.options.orderStatus && this.options.orderStatus !== -1) {
                filterArray.push({
                    Name: 'Status',
                    Operator: '==',
                    Value: this.options.orderStatus
                } as MLFilterCondition);
            }

            filterArray = filterArray.concat(this.lstFilters);

            if (filterArray.length) {
                result = JSON.stringify(filterArray);
            }

            return result;
        },

        lstSortOptions():MLSortCondition[] {
            return [
                {
                    Text: 'Thời gian (Mới nhất)',
                    Name: 'OrderDate',
                    Direction: 'DESC'
                } as MLSortCondition,
                {
                    Text: 'Thời gian (Cũ nhất)',
                    Name: 'OrderDate',
                    Direction: 'ASC'
                } as MLSortCondition,
                {
                    Text: 'Tổng tiền (Tăng dần)',
                    Name: 'TotalAmount',
                    Direction: 'ASC'
                } as MLSortCondition,
                {
                    Text: 'Tổng tiền (Giảm dần)',
                    Name: 'TotalAmount',
                    Direction: 'DESC'
                } as MLSortCondition,
                {
                    Text: 'Khách hàng (A-Z)',
                    Name: 'CustomerName',
                    Direction: 'ASC'
                } as MLSortCondition,
                {
                    Text: 'Khách hàng (Z-A)',
                    Name: 'CustomerName',
                    Direction: 'DESC'
                } as MLSortCondition
            ]
        }
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