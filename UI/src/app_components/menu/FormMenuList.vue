<!-- Màn danh sách thực đơn -->

<template>
    <VSheet style="display: flex; flex-direction: column;" class="h-full pb-2">
        <VBtn width="fit-content" class="bg-green-500 hover:bg-green-600 hover:scale-105 mr-2 mt-1 text-white ml-auto" prepend-icon="mdi-plus" rounded @click="handleAddNewMenuItem">Thêm món mới</VBtn>
        <VCard style="width: 100% ; height: 100%;" color="rgb(249, 250, 251)" class="rounded-lg d-flex flex-column shadow-md border mt-6">
            <div className="flex items-center space-x-4 px-6 py-4 border-b">
                <VTextField density="compact" variant="outlined" prepend-inner-icon="mdi-magnify" class="focus:outline-green-500" style="max-width: 320px;" hide-details placeholder="Tìm kiếm món ăn..."
                    :model-value="options.search"
                    @keypress.enter="options.search = $event.target.value;"
                />

                <VSelect density="compact" variant="outlined" hide-details class="ml-4" style="max-width: 160px;"
                    :items="lstItemCategory"
                    item-title="MenuItemCategoryName"
                    item-value="MenuItemCategoryID"
                    v-model:model-value="options.categoryID"
                />
            </div>

            <VDataTableServer
                sticky
                :items-length="totalCount"
                :loading="loading"
                loading-text="Đang tải dữ liệu..." 
                no-data-text="Không có dữ liệu" 
                items-per-page-text="Số bản ghi"
                :items="(dataList as MenuItem[])"
                style="height: 100%;"
                hide-default-footer
            >
                <template #loader>
                    <VProgressLinear color="primary" indeterminate />
                </template>

                <template #headers>
                    <tr class="bg-gray-50">
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="min-width: 150px;">Tên món</th>
                        <th class="py-3 px-6 text-right font-medium text-gray-500" style="width: 200px;">Giá món</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 250px;">Nhóm thực đơn</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 400px;">Mô tả</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 128px;">Hết hàng</th>
                        <th class="py-3 px-6 text-left font-medium text-gray-500" style="width: 128px;">Thao tác</th>
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
                                <VCheckboxBtn color="primary" v-model:model-value="item.OutOfStock" readonly />
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
                    <div className="px-6 py-4 bg-gray-50 border-t flex items-center justify-between">
                        <div className="text-gray-600">{{ recordNumberText }}</div>
                        <div className="flex align-center">
                            <span class="text-gray-600">Số bản ghi</span>

                            <VSelect
                                class="ml-4" 
                                style="width: 100px;" 
                                density="compact" 
                                variant="outlined" 
                                hide-details 
                                :items="[10, 25, 50, 100]"
                                v-model:model-value="options.itemsPerPage"
                            />

                            <VBtn icon="mdi-skip-previous" variant="outlined" class="text-gray-600 ml-10" :width="40" :disabled="options.page <= 1" @click="options.page = 1;" />
                            <VBtn icon="mdi-chevron-left" variant="outlined" class="text-gray-600 ml-2" :width="40" :disabled="options.page <= 1" @click="options.page--;" />

                            <VTextField type="number" hide-spin-buttons style="width: 72px;" class="ml-2 text-center" density="compact" variant="outlined" hide-details :model-value="options.page" 
                                :rules="[(v:string) => {
                                    return parseInt(v) > 0 && parseInt(v) <= Math.ceil(totalCount / options.itemsPerPage);
                                }]"
                                @blur="onPageNumberBlur"
                            />

                            <VBtn icon="mdi-chevron-right" variant="outlined" class="text-gray-600 ml-2" :width="40" :disabled="options.page >= Math.ceil(totalCount / options.itemsPerPage)" @click="options.page++;" />
                            <VBtn icon="mdi-skip-next" variant="outlined" class="text-gray-600 ml-2" :width="40" :disabled="options.page >= Math.ceil(totalCount / options.itemsPerPage)" @click="options.page = Math.ceil(totalCount / options.itemsPerPage);" />
                        </div>
                    </div>
                </template>
            </VDataTableServer>
        </VCard>
    </VSheet>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { MenuItem, MenuItemCategory } from '@/models';
import { menuItemStore } from '@/stores/menuItemStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        this.getMenuItems();

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
                categoryID: this.$commonValue.GuidEmpty
            },

            itemCategories: <MenuItemCategory[]>[]
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
        ...mapActions(menuItemStore as any, ['getMenuItemPaging', 'setSelectedIndex', 'addNewRecord']),
        
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
            await this.getMenuItemPaging(this.options.page, this.options.itemsPerPage, this.options.search, this.options.categoryID);
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

        onPageNumberBlur(event: any) {
            const pageNum = parseInt(event.target.value);
            if (pageNum > 0 && pageNum <= Math.ceil(this.totalCount / this.options.itemsPerPage)) {
                this.options.page = pageNum;
            }
        }
    },

    computed: {
        ...mapState(menuItemStore, ['dataList', 'totalCount', 'selectedIndex']),

        tableHeaders():any {
            return [
                { title: 'Tên món', value: 'Name', width: 300, color: 'primary' },
                { title: 'Giá món', value: 'Price', align: 'end', width: 150 },
                { title: 'Loại món', value: 'Category', width: 200 },
                { title: 'Mô tả', value: 'Description' },
                { title: 'Hết hàng', value: 'OutOfStock', align: 'center', width: 150 }
            ]
        },

        recordNumberText() {
            if (this.totalCount === 0) {
                return 'Hiển thị 0-0 trên 0 bản ghi';
            }

            const startIndex:number = (this.options.page - 1) * (this.options.itemsPerPage) + 1;
            const endIndex:number = startIndex + Math.min(this.options.itemsPerPage, this.totalCount) - 1;
            return `Hiển thị ${startIndex}-${endIndex} trên ${this.totalCount} bản ghi`;
        },

        lstItemCategory() {
            return [{
                MenuItemCategoryID: this.$commonValue.GuidEmpty,
                MenuItemCategoryName: 'Tất cả'
            } as MenuItemCategory].concat(this.itemCategories);
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