<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm tên sushi/khách hàng..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
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
                                        class="mt-1"

                                        v-model="minDateFilter"
                                        ref="customInput"
                                        :rules="[(value: Date) => {
                                            return (maxDateFilter === undefined || $moment(value).isSameOrBefore(maxDateFilter)) || 'Thời gian bắt đầu không được lớn hơn thời gian kết thúc';
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
                                        class="mt-1"
                                        
                                        v-model="maxDateFilter"
                                        :rules="[(value: Date) => {
                                            return (minDateFilter === undefined || $moment(value).isSameOrAfter(minDateFilter)) || 'Thời gian bắt đầu không được lớn hơn thời gian kết thúc';
                                        }]"
                                    />
                                </div>
                            </MLHbox>
                        </div>

                        <div>
                            <label class="block font-medium text-gray-700">Trạng thái</label>
                            <VSelect
                                :items="[
                                    {
                                        Text: 'Tất cả',
                                        Value: -1
                                    },
                                    {
                                        Text: 'Chờ xác nhận',
                                        Value: $enumeration.EnumCustomMenuRequestStatus.Pending
                                    },
                                    {
                                        Text: 'Đã xác nhận',
                                        Value: $enumeration.EnumCustomMenuRequestStatus.Approved
                                    },
                                    {
                                        Text: 'Đã huỷ',
                                        Value: $enumeration.EnumCustomMenuRequestStatus.Rejected
                                    },
                                ]"
                                item-title="Text"
                                item-value="Value"
                                :return-object="false"
                                v-model:model-value="statusFilter"

                                density="compact"
                                variant="outlined"
                                hide-details
                                color="primary"
                                class="mt-1"
                            />
                        </div>
                    </div>
                </MLFilterPopup>

                <MLSortPopup :items="sortOptionList" v-model="options.sort" />
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
                :items="(dataList as CustomMenuRequest[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px; width: 100%;">Khách hàng</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Tên món</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Ngày tạo</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Trạng thái</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 128px;">Thao tác</th>
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
                        <td class="py-4 px-6">{{ item.CustomerName }}</td>
                        <td class="py-4 px-6">{{ item.MenuItemName }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatDateTime(item.CreatedDate) }}</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="getStatusClass(item.Status)"
                            >
                                {{ getStatusName(item.Status) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" v-if="item.Status === $enumeration.EnumCustomMenuRequestStatus.Pending" />
                                <VBtn icon="mdi-eye-outline" class="text-gray-700" :width="40" variant="text" @click="openDetail(item)" v-else />
                                <!-- <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" /> -->
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
import { EnumCustomMenuRequestStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { CustomMenuRequest } from '@/models';
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { customMenuRequestStore } from '@/stores/customMenuRequestStore';
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
                sort: ''
            },

            minDateFilter: <string|undefined>undefined,
            maxDateFilter: <string|undefined>undefined,
            statusFilter: <any>-1,

            lstFilters: <MLFilterCondition[]>[]
        }
    },

    methods: {
        ...mapActions(customMenuRequestStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormCustomMenuDetail, newRecord);
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
        openDetail(record: CustomMenuRequest) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormCustomMenuDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: CustomMenuRequest) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá yêu cầu',
                Message: `Bạn có chắc chắn muốn từ chối yêu cầu tạo món <b>${item.MenuItemName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.CustomMenuRequestService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Thành công.',
                            Type: 'success'
                        });
                        
                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },

        handleResetFilters() {
            this.minDateFilter = undefined;
            this.maxDateFilter = undefined;
            this.statusFilter = -1;

            this.lstFilters = [];
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.minDateFilter !== undefined) {
                this.lstFilters.push({
                    Name: 'CreatedDate',
                    Operator: '>=',
                    Value: this.minDateFilter
                } as MLFilterCondition);
            }
            if (this.maxDateFilter !== undefined) {
                this.lstFilters.push({
                    Name: 'CreatedDate',
                    Operator: '<',
                    Value: moment(this.maxDateFilter).add(1, 'days')
                } as MLFilterCondition);
            }
            if (this.statusFilter !== -1) {
                this.lstFilters.push({
                    Name: 'Status',
                    Operator: '==',
                    Value: this.statusFilter
                } as MLFilterCondition);
            }
            
            this.getData();
        },

        /**
         * Hiển thị trạng thái
         */
        getStatusName(status: EnumCustomMenuRequestStatus) {
            switch (status) {
                case EnumCustomMenuRequestStatus.Approved:
                    return 'Đã xác nhận';
                case EnumCustomMenuRequestStatus.Pending:
                    return 'Chờ xác nhận';
                case EnumCustomMenuRequestStatus.Rejected:
                    return 'Đã huỷ';
            }
        },

        /**
         * Hiển thị trạng thái
         */
         getStatusClass(status: EnumCustomMenuRequestStatus) {
            switch (status) {
                case EnumCustomMenuRequestStatus.Approved:
                    return 'bg-green-300 text-green-700';
                case EnumCustomMenuRequestStatus.Pending:
                    return 'bg-yellow-100 text-yellow-700';
                case EnumCustomMenuRequestStatus.Rejected:
                    return 'bg-red-100 text-red-700';
            }
        },
    },

    computed: {
        ...mapState(customMenuRequestStore as any, ['dataList', 'selectedIndex', 'totalCount']),

        filterJson() {
            if (this.lstFilters) {
                return JSON.stringify(this.lstFilters);
            }
            return '';
        },

        sortOptionList():MLSortCondition[] {
            return [
                {
                    Text: 'Khách hàng (A-Z)',
                    Name: 'CustomerName',
                    Direction: 'ASC'
                },
                {
                    Text: 'Khách hàng (Z-A)',
                    Name: 'CustomerName',
                    Direction: 'DESC'
                },
                {
                    Text: 'Tên món (A-Z)',
                    Name: 'MenuItemName',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên món (Z-A)',
                    Name: 'MenuItemName',
                    Direction: 'DESC'
                },
                {
                    Text: 'Ngày tạo (Mới nhất)',
                    Name: 'CreatedDate',
                    Direction: 'DESC'
                },
                {
                    Text: 'Ngày tạo (Cũ nhất)',
                    Name: 'CreatedDate',
                    Direction: 'ASC'
                }
            ];
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
</style>