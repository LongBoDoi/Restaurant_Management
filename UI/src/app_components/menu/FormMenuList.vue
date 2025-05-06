<!-- Màn danh sách thực đơn -->

<template>
    <VSheet style="display: flex; flex-direction: column; overflow: hidden;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 mr-2 mt-1 text-white ml-auto" prepend-icon="mdi-plus" rounded @click="handleAddNewMenuItem">Thêm món mới</VBtn>
        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm món ăn..."
                    color="primary"
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <VSelect density="compact" variant="outlined" hide-details class="ml-4" style="max-width: 160px;"
                    color="primary"
                    :items="lstItemCategory"
                    item-title="MenuItemCategoryName"
                    item-value="MenuItemCategoryID"
                    v-model:model-value="options.categoryID"
                />

                <MLFilterPopup :filter-count="lstFilters.length" v-on:apply-filter="handleApplyFilters" v-on:reset-filter="handleResetFilters">
                    <div class="space-y-4">
                        <div>
                            <label class="block font-medium text-gray-700">Khoảng giá</label>
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
                                    
                                    v-model="minPrice"
                                    :rules="[
                                        $commonValue.positiveNumberRule,
                                        (value: number) => {
                                            return (maxPrice === undefined || value <= maxPrice) || 'Giá tối thiểu không được lớn hơn giá tối đa.';
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
                                    
                                    v-model="maxPrice"
                                    :rules="[
                                        $commonValue.positiveNumberRule,
                                        (value: number) => {
                                            return minPrice === undefined || value >= minPrice;
                                        }
                                    ]"
                                />
                            </div>
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
                                v-model:model-value="status"

                                density="compact"
                                variant="outlined"
                                hide-details
                                color="primary"
                                class="mt-1"
                            />
                        </div>
                    </div>
                </MLFilterPopup>

                <MLSortPopup :items="lstSortOptions" v-model="options.sort" />
            </div>

            <VDataTableServer
                sticky
                fixed-header
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as MenuItem[])"
                style="flex-grow: 1; overflow: hidden;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Tên món</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="min-width: 150px;">Giá món</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px;">Nhóm thực đơn</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 400px; width: 100%;">Mô tả</th>
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
                        @click="setSelectedIndex(index)" @dblclick="handleOpenMenuDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.Name }}</td>
                        <td class="py-4 px-6" style="text-align: end;">{{ $commonFunction.formatThousands(item.Price) }} đ</td>
                        <td class="py-4 px-6">{{ item.MenuItemCategory?.MenuItemCategoryName }}</td>
                        <td class="py-4 px-6 text-sm text-gray-600">{{ item.Description }}</td>
                        <td class="py-4 px-6">
                            <MLVbox class="align-center">
                                <VCheckboxBtn color="primary" v-model:model-value="item.Inactive" readonly />
                            </MLVbox>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-pencil-outline" :width="40" variant="text" color="rgb(37, 99, 235)" @click="handleOpenMenuDetail(item)" />
                                <VBtn icon="mdi-trash-can-outline" :width="40" variant="text" color="rgb(220, 38, 38)" @click="handleDeleteMenuItem(item)" />
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
import { MenuItem, MenuItemCategory } from '@/models';
import MLFilterCondition from '@/models/MLFilterCondition';
import MLSortCondition from '@/models/MLSortCondition';
import { menuItemStore } from '@/stores/menuItemStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.$service.MenuItemCategoryService.getAll().then((data: MenuItemCategory[]) => {
            this.itemCategories = data;
        });
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: '',
                categoryID: '',
                sort: ''
            },

            lstFilters: <MLFilterCondition[]>[],
            minPrice: <number|undefined>undefined,
            maxPrice: <number|undefined>undefined,
            status: <any>-1,

            itemCategories: <MenuItemCategory[]>[],
        }
    },

    watch: {
        options: {
            handler() {
                this.getMenuItems();
            },
            deep: true
        }
    },

    methods: {
        ...mapActions(menuItemStore as any, ['getDataPaging', 'setSelectedIndex', 'addNewRecord']),
        
        /**
         * Xử lý thêm món ăn mới
         */
         handleAddNewMenuItem() {
            const newMenuItem = this.addNewRecord() as MenuItem;

            EventBus.emit(this.$eventName.ShowFormMenuDetail, newMenuItem);
        },

        /**
         * Lấy danh sách món
         */
        async getMenuItems() {
            this.loading = true;
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search, this.filterJson, this.options.sort);
            this.loading = false;
        },

        /**
         * Xử lý mở chi tiết món ăn
         */
        handleOpenMenuDetail(item:MenuItem) {
            item.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormMenuDetail, item);
        },

        /**
         * Xử lý xoá món
         */
         handleDeleteMenuItem(item:MenuItem) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá món',
                Message: `Bạn có chắc chắn muốn xoá món <b>${item.Name}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.MenuItemService.saveChanges(item)).Success) {
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
            this.minPrice = undefined;
            this.maxPrice = undefined;
            this.status = -1;
        },

        handleApplyFilters() {
            this.lstFilters = [];
            if (this.minPrice !== undefined) {
                this.lstFilters.push({
                    Name: 'Price',
                    Operator: '>=',
                    Value: this.minPrice
                } as MLFilterCondition);
            }
            if (this.maxPrice !== undefined) {
                this.lstFilters.push({
                    Name: 'Price',
                    Operator: '<=',
                    Value: this.maxPrice
                } as MLFilterCondition);
            }
            if (this.status !== -1) {
                this.lstFilters.push({
                    Name: 'Inactive',
                    Operator: '==',
                    Value: this.status
                } as MLFilterCondition);
            }

            this.getMenuItems();
        }
    },

    computed: {
        ...mapState(menuItemStore, ['dataList', 'totalCount', 'selectedIndex']),

        lstItemCategory() {
            return [{
                MenuItemCategoryID: '',
                MenuItemCategoryName: 'Tất cả'
            } as MenuItemCategory].concat(this.itemCategories);
        },

        filterJson() {
            var result = '';

            var filterArray = [];
            if (this.options.categoryID) {
                filterArray.push({
                    Name: 'MenuItemCategoryID',
                    Operator: '==',
                    Value: this.options.categoryID
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
                    Text: 'Tên món (A-Z)',
                    Name: 'Name',
                    Direction: 'ASC'
                },
                {
                    Text: 'Tên món (Z-A)',
                    Name: 'Name',
                    Direction: 'DESC'
                },
                {
                    Text: 'Giá món (Tăng dần)',
                    Name: 'Price',
                    Direction: 'ASC'
                },
                {
                    Text: 'Giá món (Giảm dần)',
                    Name: 'Price',
                    Direction: 'DESC'
                }
            ]
        }
    }
}
</script>

<style lang="scss" scoped>
.selected-row {
    background-color: #81c784;
    color: white;
}
</style>