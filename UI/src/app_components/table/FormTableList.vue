<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4 mr-2" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm bàn</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm bàn..."
                    color="primary"
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <VSelect
                    :items="areaList"
                    item-title="AreaName"
                    item-value="AreaID"
                    :return-object="false"
                    v-model:model-value="options.areaID"

                    density="compact"
                    variant="outlined"
                    hide-details
                    color="primary"
                    style="max-width: 200px;"
                />

                <MLFilterPopup :filter-count="lstFilters.length" :width="'auto'" v-on:reset-filter="handleResetFilters" v-on:apply-filter="handleApplyFilters">
                    <div class="space-y-4">
                        <div>
                            <label class="block font-medium text-gray-700">Số ghế</label>
                            <MLNumberField
                                hide-details
                                variant="outlined"
                                density="compact"
                                class="mt-1"
                                color="primary"
                                nullable
                                :rules="[(value: number) => {
                                    return value > 0;
                                }]"

                                v-model="seatCountFilter"
                            />
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
                                        Text: 'Còn trống',
                                        Value: $enumeration.EnumTableStatus.Available
                                    },
                                    {
                                        Text: 'Hết chỗ',
                                        Value: $enumeration.EnumTableStatus.Occupied
                                    },
                                    {
                                        Text: 'Đã đặt chỗ',
                                        Value: $enumeration.EnumTableStatus.Reserved
                                    }
                                ]"
                                item-title="Text"
                                item-value="Value"
                                hide-details
                                variant="outlined"
                                density="compact"
                                class="mt-1"
                                color="primary"

                                v-model:model-value="tableStatusFilter"
                            />
                        </div>
                    </div>
                </MLFilterPopup>

                <MLSortPopup :items="sortConditionList" v-model:model-value="options.sort" />
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
                :items="(dataList as Table[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px; width: 100%;">Tên bàn</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Số ghế</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Khu vực</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Trạng thái</th>
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
                    <tr v-if="item !== null" style="cursor: pointer;" class="hover:bg-green-50 transition-colors duration-150" 
                        :class="[
                            {'bg-gray-50' : index % 2 !== 0},
                            {'bg-green-100' : selectedIndex === index}
                        ]" 
                        @click="setSelectedIndex(index)" @dblclick="openDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.TableName }}</td>
                        <td class="py-4 px-6">{{ $commonFunction.formatThousands(item.SeatCount) }}</td>
                        <td class="py-4 px-6">{{ item.Area?.AreaName ?? '' }}</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="getTableStatusClass(item.Status)"
                            >
                                {{ getTableStatus(item.Status) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="openDetail(item)" />
                                <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteRecord(item)" />
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
import { EnumTableStatus } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Area, Table } from '@/models';
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { tableStore } from '@/stores/tableStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.$service.AreaService.getAll().then((data: Area[]) => {
            this.allAreas = data;
        });
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                areaID: '',
                sort: ''
            },

            seatCountFilter: <number|undefined>undefined,
            tableStatusFilter: <EnumTableStatus|number>-1,
            lstFilters: <MLFilterCondition[]>[],

            allAreas: <Area[]>[],
        }
    },

    methods: {
        ...mapActions(tableStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormTableDetail, newRecord);
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
        openDetail(record: Table) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormTableDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: Table) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá bàn',
                Message: `Bạn có chắc chắn muốn xoá Bàn <b>${item.TableName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.TableService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });
                        
                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },

        /**
         * Hiển thị trạng thái bàn
         */
        getTableStatus(status: EnumTableStatus) {
            switch (status) {
                case EnumTableStatus.Available:
                    return 'Còn trống';
                case EnumTableStatus.Occupied:
                    return 'Hết chỗ';
                case EnumTableStatus.Reserved:
                    return 'Đã đặt chỗ';
            }
        },

        getTableStatusClass(status: EnumTableStatus) {
            switch (status) {
                case EnumTableStatus.Available:
                    return 'bg-green-300 text-green-700';
                case EnumTableStatus.Occupied:
                    return 'bg-red-100 text-red-700';
                case EnumTableStatus.Reserved:
                    return 'bg-yellow-100 text-yellow-700';
            }
        },

        handleResetFilters() {
            this.seatCountFilter = undefined;
            this.tableStatusFilter = -1;

            this.lstFilters = [];
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.seatCountFilter !== undefined) {
                this.lstFilters.push({
                    Name: 'SeatCount',
                    Operator: '==',
                    Value: this.seatCountFilter
                } as MLFilterCondition);
            }
            if (this.tableStatusFilter !== -1) {
                this.lstFilters.push({
                    Name: 'Status',
                    Operator: '==',
                    Value: this.tableStatusFilter
                } as MLFilterCondition);
            }

            this.getData();
        }
    },

    computed: {
        ...mapState(tableStore as any, ['dataList', 'selectedIndex', 'totalCount']),

        areaList() {
            return [{
                AreaID: '',
                AreaName: 'Tất cả khu vực'
            } as Area,
            ...this.allAreas];
        },

        sortConditionList() {
            return [
                {
                    Text: 'Tên bàn (A-Z)',
                    Name: 'TableName',
                    Direction: 'ASC'
                } as MLSortCondition,
                {
                    Text: 'Tên bàn (Z-A)',
                    Name: 'TableName',
                    Direction: 'DESC'
                } as MLSortCondition,
                {
                    Text: 'Số ghế (Tăng dần)',
                    Name: 'SeatCount',
                    Direction: 'ASC'
                } as MLSortCondition,
                {
                    Text: 'Số ghế (Giảm dần)',
                    Name: 'SeatCount',
                    Direction: 'DESC'
                } as MLSortCondition,
            ]
        },

        filterJson() {
            var result = '';

            var filterArray = [];
            if (this.options.areaID) {
                filterArray.push({
                    Name: 'AreaID',
                    Operator: '==',
                    Value: this.options.areaID
                } as MLFilterCondition);
            }

            filterArray = filterArray.concat(this.lstFilters);

            if (filterArray.length) {
                result = JSON.stringify(filterArray);
            }

            return result;
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
</style>