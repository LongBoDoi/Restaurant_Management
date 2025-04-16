<!-- Màn danh sách khách hàng -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2 overflow-hidden">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 text-white ml-auto mt-4" prepend-icon="mdi-plus" rounded @click="handleAddNewClick">Thêm nhóm nguyên liệu</VBtn>

        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <!-- Toolbar -->
            <div className="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm nhóm nguyên liệu..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />
            </div>

            <!-- Bảng dữ liệu -->
            <VDataTableServer
                sticky
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as InventoryItemCategory[])"
                style="height: 100%;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 250px; width: 100%;">Tên nguyên liệu</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="min-width: 250px;">Số lượng nguyên liệu</th>
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
                    <span v-if="loading">Đang tải dữ liệu</span>
                    <tr v-if="item !== null" style="cursor: pointer;" class="hover:bg-green-50 transition-colors duration-150" 
                        :class="[
                            {'bg-gray-50' : index % 2 !== 0},
                            {'bg-green-100' : selectedIndex === index}
                        ]" 
                        @click="setSelectedIndex(index)" @dblclick="openDetail(item)"
                    >
                        <td class="py-4 px-6">{{ item.InventoryItemCategoryName }}</td>
                        <td class="py-4 px-6 text-right">{{ item.InventoryItemCount }}</td>
                        <td class="py-4 px-6">
                            <span class="px-2.5 py-1 rounded-full text-xs font-medium"
                                :class="[
                                    {'bg-green-300 text-green-700' : !item.Inactive},
                                    {'bg-red-100 text-red-700' : item.Inactive}
                                ]"
                            >
                                {{ item.Inactive ? 'Ngừng hoạt động' : 'Đang hoạt động' }}
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
import EventBus from '@/common/EventBus';
import InventoryItemCategory from '@/models/InventoryItemCategory';
import { inventoryItemCategoryStore } from '@/stores/inventoryItemCategoryStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getData();
    },

    data() {
        return {
            loading: <boolean>false,

            options: <any>{
                page: 1,
                itemsPerPage: 10,
                search: ''
            }
        }
    },

    methods: {
        ...mapActions(inventoryItemCategoryStore as any, ['addNewRecord', 'setSelectedIndex', 'getDataPaging']),

        /**
         * Xử lý thêm khách hàng mới
         */
        handleAddNewClick() {
            const newRecord = this.addNewRecord();
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemCategoryDetail, newRecord);
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
        openDetail(record: InventoryItemCategory) {
            record.EditMode = this.$enumeration.EnumEditMode.Edit;
            
            EventBus.emit(this.$eventName.ShowFormInventoryItemCategoryDetail, record);
        },

        /**
        * Xử lý xoá bản ghi
        */
        handleDeleteRecord(item: InventoryItemCategory) {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá món',
                Message: `Bạn có chắc chắn muốn xoá Nhóm nguyên liệu <b>${item.InventoryItemCategoryName}</b> không?`,
                ConfirmAction: async () => {
                    item.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if ((await this.$service.InventoryItemCategoryService.saveChanges(item)).Success) {
                        EventBus.emit(this.$eventName.ShowToastMessage, {
                            Message: 'Xoá thành công.',
                            Type: 'success'
                        });
                        
                        this.dataList.splice(this.dataList.indexOf(item), 1);
                    }
                }
            });
        },
    },

    computed: {
        ...mapState(inventoryItemCategoryStore as any, ['dataList', 'selectedIndex', 'totalCount']),
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