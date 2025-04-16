<!-- Màn danh sách thực đơn -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2 overflow-hidden">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mr-2 mt-1" prepend-icon="mdi-plus" @click="handleAddNewRecord" rounded>Thêm nhóm thực đơn</VBtn>
        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6" :disabled="saveSorting">
            <div class="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm nhóm thực đơn..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <VSpacer />

                <button class="pa-2 hover:bg-green-100 hover:text-green-700 rounded-lg transition-all duration-200 group relative overflow-hidden"
                    @click="enableSort"
                    :class="[{'bg-green-100 text-green-700' : sorting}]"
                >
                    <div style="width: 24px; height: 24px;" class="relative overflow-hidden flex items-center justify-center">
                        <span class="material-symbols-outlined absolute transition-transform duration-300 transform translate-x-0"
                            :class="[{'translate-x-[-100%]' : sorting}]"
                        >
                            swap_vert
                        </span>
                        <span class="material-symbols-outlined absolute transition-transform duration-300 transform translate-x-[100%]"
                            :class="[{'translate-x-0' : sorting}]"
                        >
                            save
                        </span>
                    </div>
                </button>
            </div>

            <VDataTableServer
                sticky
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..."
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="lstRecord"
                style="height: 100%;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 200px;">Tên nhóm thực đơn</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="min-width: 200px;">Số lượng món</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Màu sắc</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 100%; min-width: 250px;">Mô tả</th>
                        <th class="py-3 px-6 text-center font-medium text-gray-500" style="min-width: 150px;">Ngừng hoạt động</th>
                        <th class="py-3 px-6 text-center font-medium text-gray-500 w-24">Thao tác</th>
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
                            {'bg-green-100' : selectedIndex === item.MenuItemCategoryID}
                        ]"
                        @click="setSelectedIndex(item.MenuItemCategoryID)"
                        @dblclick="openDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.MenuItemCategoryName }}</td>
                        <td class="py-4 px-6" style="text-align: end;">{{ item.ItemCount }}</td>
                        <td class="py-4 px-6">
                            <div class="relative flex items-center justify-center w-fit">
                                <span style="opacity: 0.2; width: 32px; height: 16px;" class="rounded-full"
                                    :style="{'background-color' : item.Color}"
                                />
                                <span style="width: 8px; height: 8px;" class="absolute rounded-full mr-1.5"
                                    :style="{'background-color' : item.Color}"
                                />
                            </div>
                        </td>
                        <td class="py-4 px-6 text-sm text-gray-600">{{ item.Description }}</td>
                        <td class="px-6 text-sm text-gray-600">
                            <MLVbox class="align-center">
                                <VCheckbox hide-details style="height: fit-content;" color="primary" :model-value="item.Inactive" readonly />
                            </MLVbox>
                        </td>
                        <td class="py-4 px-6">
                            <MLHbox>
                                <VBtn icon="mdi-pencil-outline" variant="text" color="rgb(37, 99, 235)" :width="40" @click="openDetail(item)" />
                                <VBtn icon="mdi-trash-can-outline" variant="text" color="rgb(220, 38, 38)" :width="40" @click="handleDeleteRecord(item)" />
                                
                                <VSlideXReverseTransition>
                                    <MLHbox v-if="sorting">
                                        <VBtn icon="mdi-arrow-up" class="text-gray-600" variant="text" :width="40" :disabled="index <= 0" @click.stop="handleSort(item, -1)" />
                                        <VBtn icon="mdi-arrow-down" class="text-gray-600" variant="text" :width="40" :disabled="index >= lstRecord.length - 1" @click.stop="handleSort(item, 1)" />
                                    </MLHbox>
                                </VSlideXReverseTransition>
                            </MLHbox>
                        </td>
                    </tr>
                </template>

                <template #bottom>
                    <MLDataTableFooter :total-count="totalCount" :options="options" />
                </template>
            </VDataTableServer>
        </VCard>
    </VSheet>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { MenuItemCategory } from '@/models';
import { menuItemCategoryStore } from '@/stores/menuItemCategoryStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getData();
    },

    data() {
        return {
            loading: <boolean>false,
            saveSorting: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: ''
            },

            sorting: <boolean>false,
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

    methods: {
        ...mapActions(menuItemCategoryStore as any, ['getDataPaging', 'setSelectedIndex', 'addNewRecord']),

        /**
         * Lấy dữ liệu phân trang
         */
         async getData() {
            this.loading = true;
            
            await this.getDataPaging(this.options.page, this.options.itemsPerPage, this.options.search);

            this.loading = false;
        },

        /**
         * Thêm bản ghi mới
         */
        handleAddNewRecord() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormMenuCategoryDetail, newRecord);
        },

        /**
         * Xử lý mở form chi tiết
         */
        openDetail(item:MenuItemCategory) {
            item.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormMenuCategoryDetail, item);
        },

        /**
         * Xử lý xoá bản ghi
         */
         handleDeleteRecord(item:MenuItemCategory) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá món',
                Message: `Bạn có chắc chắn muốn xoá Nhóm thực đơn <b>${item.MenuItemCategoryName}</b> không? Các món ăn thuộc nhóm này sẽ bị gỡ ra khỏi nhóm.`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.MenuItemCategoryService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });
                        
                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },

        async enableSort() {
            if (!this.sorting) {
                this.sorting = true;
                return;
            }

            this.saveSorting = true;
            this.dataList.forEach((record: MenuItemCategory) => {
                record.EditMode = this.$enumeration.EnumEditMode.Edit;
            });
            await this.$service.MenuItemCategoryService.saveChangesMultiple(this.dataList as MenuItemCategory[]);
            this.saveSorting = false;

            this.sorting = false;
        },

        handleSort(record: MenuItemCategory, direction: 1|-1) {
            const otherRecord:MenuItemCategory = this.lstRecord[this.lstRecord.indexOf(record) + direction];

            const otherRecordSortOrder:number = otherRecord.SortOrder;
            otherRecord.SortOrder = record.SortOrder;
            record.SortOrder = otherRecordSortOrder;

            this.setSelectedIndex(record.MenuItemCategoryID);
        }
    },

    computed: {
        ...mapState(menuItemCategoryStore as any, ['dataList', 'totalCount', 'selectedIndex']),

        lstRecord():MenuItemCategory[] {
            return this.dataList.toSorted((a:MenuItemCategory, b:MenuItemCategory) => a.SortOrder - b.SortOrder);
        }
    }
}
</script>

<style lang="scss" scoped>
</style>