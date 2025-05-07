<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <MLHbox class="mt-4 space-x-4">
            <VBtn class="ml-auto hover:scale-105 shadow-md bg-blue-500 hover:bg-blue-600 text-white" rounded prepend-icon="mdi-clipboard-plus-outline" @click="handleUpdateStockClick">
                Cập nhật tồn kho
            </VBtn>
            <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white mr-3" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm nguyên liệu</VBtn>
        </MLHbox>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm nguyên liệu..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <MLFilterPopup :filter-count="filterCount" v-on:reset-filter="handleResetFilters" v-on:apply-filter="handleApplyFilters">
                    <div class="space-y-4">
                        <div>
                            <label class="block font-medium text-gray-700">Nhóm nguyên liệu</label>
                            <VSelect
                                :items="filterCategories"
                                item-title="InventoryItemCategoryName"
                                item-value="InventoryItemCategoryID"
                                v-model:model-value="categoryFilter"

                                hide-details
                                density="compact"
                                variant="outlined"
                                color="primary"
                                class="mt-1"
                            />
                        </div>

                        <div>
                            <label class="block font-medium text-gray-700">Trạng thái tồn kho</label>
                            <VSelect
                                :items="[
                                    {
                                        Text: 'Tất cả',
                                        Value: -1
                                    },
                                    {
                                        Text: 'Còn hàng',
                                        Value: 0
                                    },
                                    {
                                        Text: 'Sắp hết hàng',
                                        Value: 1,
                                    },
                                    {
                                        Text: 'Hết hàng',
                                        Value: 2,
                                    },
                                    {
                                        Text: 'Thiếu hàng',
                                        Value: 3
                                    }
                                ]"
                                item-title="Text"
                                item-value="Value"
                                v-model:model-value="statusFilter"

                                hide-details
                                density="compact"
                                variant="outlined"
                                color="primary"
                                class="mt-1"
                            />
                        </div>

                        <div>
                            <label class="block font-medium text-gray-700">Trạng thái hoạt động</label>
                            <VSelect
                                :items="[
                                    {
                                        Text: 'Tất cả',
                                        Value: -1
                                    },
                                    {
                                        Text: 'Đang hoạt động',
                                        Value: false
                                    },
                                    {
                                        Text: 'Ngừng hoạt động',
                                        Value: true
                                    },
                                ]"
                                item-title="Text"
                                item-value="Value"
                                :return-object="false"
                                v-model:model-value="inactiveFilter"

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
                :items="(dataList as InventoryItem[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px; width: 100%;">Tên nguyên liệu</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Nhóm nguyên liệu</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Tồn kho</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 100px;">Đơn vị tính</th>
                        <th class="py-3 px-6 text-center font-medium text-gray-500" style="min-width: 128px;">Ngừng hoạt động</th>
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
                        <td class="py-4 px-6">{{ item.Name }}</td>
                        <td class="py-4 px-6">{{ item.InventoryItemCategory?.InventoryItemCategoryName ?? '' }}</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="[
                                    {'bg-green-300 text-green-700' : item.Quantity > item.WarningStockQuantity},
                                    {'bg-yellow-100 text-yellow-700' : item.Quantity > 0 && item.Quantity <= item.WarningStockQuantity},
                                    {'bg-red-100 text-red-700' : item.Quantity <= 0}
                                ]"
                            >
                                {{ getStockStatus(item) }}
                            </span>
                        </td>
                        <td class="py-4 px-6">{{ item.Unit }}</td>
                        <td class="py-4 px-6">
                            <MLVbox class="align-center">
                                <VCheckboxBtn color="primary" v-model:model-value="item.Inactive" readonly />
                            </MLVbox>
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
import EventBus from '@/common/EventBus';
import { InventoryItem } from '@/models';
import InventoryItemCategory from '@/models/InventoryItemCategory';
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { inventoryItemStore } from '@/stores/inventoryItemStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.$service.InventoryItemCategoryService.getAll().then((data: InventoryItemCategory[]) => {
            this.allInventoryItemCategories = data;
        });

        EventBus.on(this.$eventName.ReloadInventoryItemData, this.getData);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ReloadInventoryItemData);
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                sort: ''
            },

            allInventoryItemCategories: <InventoryItemCategory[]>[],

            categoryFilter: <string>'',
            statusFilter: <any>-1,
            inactiveFilter: <any>-1,

            lstFilters: <MLFilterCondition[]>[]
        }
    },

    methods: {
        ...mapActions(inventoryItemStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord:InventoryItem = this.addNewRecord();
            newRecord.Quantity = 0;
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemDetail, newRecord);
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
        openDetail(record: InventoryItem) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item:InventoryItem) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá nguyên liệu',
                Message: `Bạn có chắc chắn muốn xoá Nguyên liệu <b>${item.Name}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.InventoryItemService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });
                        
                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },

        handleResetFilters() {
            this.categoryFilter = '';
            this.statusFilter = -1;
            this.inactiveFilter = -1;

            this.lstFilters = [];
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.categoryFilter) {
                this.lstFilters.push({
                    Name: 'InventoryItemCategoryID',
                    Operator: '==',
                    Value: this.categoryFilter
                } as MLFilterCondition);
            }
            if (this.statusFilter !== -1) {
                switch (this.statusFilter) {
                    case 3:
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '<',
                            Value: 0
                        } as MLFilterCondition);
                        break;
                    case 2:
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '==',
                            Value: 0
                        } as MLFilterCondition);
                        break;
                    case 1:
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '>',
                            Value: 0
                        } as MLFilterCondition);
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '<=',
                            Value: 'WarningStockQuantity',
                            CompareProperties: true
                        } as MLFilterCondition);
                        break;
                    case 0:
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '>',
                            Value: 0
                        } as MLFilterCondition);
                        this.lstFilters.push({
                            Name: 'Quantity',
                            Operator: '>',
                            Value: 'WarningStockQuantity',
                            CompareProperties: true
                        } as MLFilterCondition);
                        break;
                }
            }
            if (this.inactiveFilter !== -1) {
                this.lstFilters.push({
                    Name: 'Inactive',
                    Operator: '==',
                    Value: this.inactiveFilter
                } as MLFilterCondition);
            }

            this.getData();
        },

        /**
         * Hiển thị trạng thái tồn kho
         */
        getStockStatus(item: InventoryItem) {
            var statusName = 'Còn hàng';
            if (item.Quantity < 0) {
                statusName = 'Thiếu hàng';
            } else if (item.Quantity == 0) {
                statusName = 'Hết hàng';
            } else if (item.Quantity <= item.WarningStockQuantity) {
                statusName = 'Sắp hết hàng';
            }

            return `${statusName} (${this.$commonFunction.formatThousands(item.Quantity)})`;
        },

        handleUpdateStockClick() {
            EventBus.emit(this.$eventName.ShowFormUpdateStock);
        }
    },

    computed: {
        ...mapState(inventoryItemStore as any, ['dataList', 'selectedIndex', 'totalCount']),

        filterCategories():InventoryItemCategory[] {
            return [
                {
                    InventoryItemCategoryID: '',
                    InventoryItemCategoryName: 'Tất cả'
                } as InventoryItemCategory,
                ...this.allInventoryItemCategories
            ]
        },

        sortOptionList():MLSortCondition[] {
            return [
                {
                    Text: 'Tên nguyên liệu (A-Z)',
                    Name: 'Name',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên nguyên liệu (Z-A)',
                    Name: 'Name',
                    Direction: 'DESC'
                }
            ]
        },

        filterJson() {
            if (this.lstFilters.length) {
                return JSON.stringify(this.lstFilters);
            }
            return '';
        },

        filterCount() {
            const filterSet = new Set(this.lstFilters.map(f => f.Name));
            return filterSet.size;
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