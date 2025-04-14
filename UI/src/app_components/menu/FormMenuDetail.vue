<template>
    <VDialog width="600" max-height="90vh" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 className="text-white text-xl font-bold">Thông tin món</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem class="pa-6">
                <VForm ref="form">
                    <div className="grid grid-cols-2 gap-4">
                        <div className="col-span-2">
                            <label className="block text-sm font-medium text-gray-700">Tên món <span style="color: red;">*</span></label>
                            <!-- Tên món -->
                            <VTextField 
                                density="compact"
                                class="mt-2 flex-grow-1 flex-shrink-0" 
                                variant="outlined" 
                                v-model:model-value="menuItem.Name"
                                hide-details
                                label=""
                                :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                            >
                                <template #details>
                                </template>
                            </VTextField>
                        </div>

                        <div>
                            <!-- Giá món -->
                            <label className="block text-sm font-medium text-gray-700">Giá món</label>
                            <VTextField class="w-full mt-2 text-right" v-money="$commonValue.moneyConfig" density="compact" variant="outlined" suffix="đ" v-model:model-value="menuItem.Price" hide-details />
                        </div>

                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Nhóm thực đơn</label>

                            <!-- Nhóm thực đơn -->
                            <VCombobox
                                class="mt-2"
                                :return-object="false"
                                v-model:model-value="menuItem.MenuItemCategoryID"
                                @update:model-value="onSelectCategory"
                                ref="cbMenuCategory"
                                variant="outlined"
                                density="compact"
                                :items="lstMenuItemCategory"
                                hide-details
                                item-title="MenuItemCategoryName"
                                item-value="MenuItemCategoryID"
                            />
                        </div>

                        <div className="col-span-2">
                            <!-- Mô tả -->
                            <label className="block text-sm font-medium text-gray-700">Mô tả</label>
                            <VTextarea no-resize class="mt-2" variant="outlined" v-model:model-value="menuItem.Description" hide-details />
                        </div>

                        <div className="col-span-2">
                            <!-- Ảnh món -->
                            <label className="block text-sm font-medium text-gray-700 mb-1">Ảnh món</label>
                            <div class="intro-image-box mt-2">
                                <template v-if="itemImageUrl">
                                    <VImg :src="itemImageUrl" />
                                    <VIcon class="close-icon" icon="mdi-close" color="red" @click="onRemoveMenuItemImage" />
                                </template>

                                <template v-else>
                                    <div @click="selectImage()" style="width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; cursor: pointer;">
                                        <VIcon icon="mdi-image-plus-outline" color="rgb(156, 163, 175)" />
                                    </div>
                                </template>
                                
                                <VFileInput
                                    ref="introImage"
                                    v-show="false"
                                    accept="image/*"
                                    v-model:model-value="itemImage"
                                    v-on:update:model-value="getImageUrls"
                                />
                            </div>
                        </div>

                        <div className="col-span-2" v-if="false">
                            <!-- Nguyên liệu -->
                            <label className="block text-sm font-medium text-gray-700">Nguyên liệu</label>
                            <TableMenuItemInventoryItem class="mt-2" :menu-item="menuItem" />
                        </div>

                        <div className="col-span-2">
                            <!-- Ngừng hoạt động -->
                            <VCheckbox color="primary" class="text-gray-700" style="opacity: 1;" label="Ngừng hoạt động" v-model:model-value="menuItem.Inactive" hide-details />
                        </div>

                        <div className="col-span-2" v-if="false">
                            <!-- Nguyên liệu -->
                            <label className="block text-sm font-medium text-gray-700 mb-1">Nguyên liệu</label>
                            <VCombobox
                                class="mt-1"
                                ref="cbMenuCategory"
                                variant="outlined"
                                density="compact"
                                :items="lstMenuItemCategory"
                                hide-details
                                item-title="MenuItemCategoryName"
                                item-value="MenuItemCategoryID"
                                multiple
                            />
                        </div>
                    </div>
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Lưu thay đổi</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MenuItemCategory, MLActionResult } from '@/models';
import MenuItem from '@/models/MenuItem';
import { menuItemStore } from '@/stores/menuItemStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    async created() {
        EventBus.on(this.$eventName.ShowFormMenuDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormMenuDetail);
    },

    methods: {
        ...mapActions(menuItemStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        async handleShowDialog(menuItem: MenuItem) {
            if (menuItem) {
                this.loading = true;
                this.lstMenuItemCategory = await this.$service.MenuItemCategoryService.getAll();
                this.loading = false;

                if (menuItem.EditMode !== undefined) {
                    this.editMode = menuItem.EditMode;
                }
                if (menuItem.EditMode === this.$enumeration.EnumEditMode.Add) {
                    // menuItem.MenuItemCategoryID = EnumMenuItemCategory.Appetizers.toString();
                }

                this.menuItem = menuItem;
                
                this.itemImageUrl = this.$commonFunction.getImageUrl(menuItem.ImageUrl);

                this.oldMenuItem = JSON.parse(JSON.stringify(this.menuItem));
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.menuItem.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    this.dataList[this.selectedIndex] = this.oldMenuItem;
                    break;
            }
            this.isShow = false;
        },

        /**
         * Xử lý nhấn vào nút Lưu
         */
        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            this.menuItem.Price = this.$commonFunction.getRealFloatValue(this.menuItem.Price);

            if ((await this.saveChanges('Lưu thành công')).Success) {
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Lưu thông tin món
         * @param confirmMessage Câu thông báo khi lưu thành công
         */
        async saveChanges(confirmMessage: string):Promise<MLActionResult> {
            let result:MLActionResult = {
                Success: false
            };

            result = await this.$service.MenuItemService.updateMenuItem(this.menuItem, this.itemImage);
            if (result.Success) {
                Object.assign(this.menuItem, result.Data as MenuItem);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: confirmMessage,
                    Type: 'success'
                });

                this.itemImage = undefined;
            }

            return result;
        },

        async selectImage() {
            await this.$nextTick();

            const introImage = (this.$refs.introImage as any);

            const input = introImage.$el.querySelector('input[type=file]');
            input.click();
        },

        getImageUrls(file:File|File[]) {
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.itemImage = file as File;
                    this.itemImageUrl = e.target?.result as string;
                };
                reader.readAsDataURL(file as File);
            }
        },

        onRemoveMenuItemImage() {
            this.itemImage = undefined;
            this.itemImageUrl = '';
            this.menuItem.ImageUrl = '';
        },

        onSelectCategory(menuItemCategoryID: string) {
            const menuItemCategory:MenuItemCategory|undefined = this.lstMenuItemCategory.find(mic => mic.MenuItemCategoryID === menuItemCategoryID);
            this.menuItem.MenuItemCategory = menuItemCategory;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            menuItem: <MenuItem>{} as MenuItem,
            oldMenuItem: <MenuItem>{} as MenuItem,

            itemImage: <File|undefined>undefined,
            itemImageUrl: <string>'',

            lstMenuItemCategory: <MenuItemCategory[]>[]
        }
    },

    computed: {
        ...mapState(menuItemStore, ['dataList', 'selectedIndex']),
    }
}
</script>

<style lang="scss" scoped>
.intro-image-box {
    width: 96px; 
    height: 96px; 
    border: 2px dashed rgb(209, 213, 219);
    display: flex; 
    align-items: center; 
    justify-content: center; 
    border-radius: 24px; 
    position: relative;
    flex-shrink: 0;

    .close-icon {
        position: absolute; 
        top: 0; 
        right: 0; 
        display: none;
    }

    &:hover .close-icon {
        display: block !important;
    }
}
</style>