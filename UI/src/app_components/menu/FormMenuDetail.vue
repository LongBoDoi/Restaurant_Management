<template>
    <VDialog max-width="800" persistent :model-value="isShow">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                Thông tin món
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem>
                <VForm ref="form">
                <!-- Tên món -->
                <VTextField 
                    density="compact" 
                    class="mt-2 flex-grow-1 flex-shrink-0" 
                    variant="outlined" 
                    v-model:model-value="menuItem.Name"
                    :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                >
                    <template v-slot:label>
                        Tên món
                        <span style="color: red;">*</span>
                    </template>
                </VTextField>

                <MLHbox>
                    <!-- Giá món -->
                    <VTextField width="60%" v-money="moneyConfig" density="compact" variant="outlined" suffix="đ" v-model:model-value="menuItem.Price">
                        <template v-slot:label>
                            Giá món
                        </template>
                    </VTextField>

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Loại món -->
                    <VSelect
                        :return-object="false"
                        v-model:model-value="menuItem.Category"
                        ref="cbMenuCategory"
                        label="Loại món"
                        width="40%"
                        variant="outlined"
                        density="compact"
                        :items="listMenuCategory"
                        item-title="Text"
                        item-value="Value"
                    />
                </MLHbox>

                <!-- Mô tả -->
                <VTextarea no-resize label="Mô tả" variant="outlined" v-model:model-value="menuItem.Description" />

                <!-- Hết hàng -->
                <VCheckbox color="primary" label="Hết hàng" v-model:model-value="menuItem.OutOfStock" />
                </VForm>
            </VCardItem>

            <VCardActions>
                <VBtn v-if="editMode === $enumeration.EnumEditMode.Edit" prepend-icon="mdi-trash-can-outline" color="error" @click="handleDeleteMenuItem">Xoá món</VBtn>

                <VSpacer />

                <VBtn @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleSaveClick">Lưu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode, EnumMenuItemCategory } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MLActionResult } from '@/models';
import MenuItem from '@/models/MenuItem';
import { menuItemStore } from '@/stores/menuItemStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
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
        handleShowDialog(menuItem: MenuItem) {
            if (menuItem) {
                if (menuItem.EditMode !== undefined) {
                    this.editMode = menuItem.EditMode;
                }
                if (menuItem.EditMode === this.$enumeration.EnumEditMode.Add) {
                    menuItem.Category = EnumMenuItemCategory.Appetizers;
                }

                this.menuItem = menuItem;
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

            this.menuItem.Price = parseFloat(this.menuItem.Price.toString().replaceAll('.', '').replaceAll(',', ''));

            if (await this.saveChanges('Lưu thành công')) {
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Xử lý xoá món
         */
        handleDeleteMenuItem() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá món',
                Message: `Bạn có chắc chắn muốn xoá món <b>${this.menuItem.Name}</b> không?`,
                ConfirmAction: async () => {
                    this.menuItem.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if (await this.saveChanges('Xoá món thành công')) {
                        this.isShow = false;
                        this.removeSelectedRecord();
                    }
                }
            });
        },

        /**
         * Lưu thông tin món
         * @param confirmMessage Câu thông báo khi lưu thành công
         */
        async saveChanges(confirmMessage: string):Promise<boolean> {
            let result:MLActionResult|undefined = undefined;

            try {
                result = await this.$service.MenuItemService.saveChanges(this.menuItem);
            } catch (e) {
                this.$commonFunction.handleException(e);
            }

            if (!result) return false;

            if (result.Success) {
                this.dataList[this.selectedIndex] = result.Data as MenuItem;
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: confirmMessage,
                    Type: 'success'
                });
            } else {
                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: result.ErrorMsg,
                    Type: 'error'
                });
            }

            return result.Success;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            menuItem: <MenuItem>{} as MenuItem,
            oldMenuItem: <MenuItem>{} as MenuItem
        }
    },

    computed: {
        ...mapState(menuItemStore, ['dataList', 'selectedIndex']),

        listMenuCategory() {
            return [
                {
                    Text: 'Khai vị',
                    Value: this.$enumeration.EnumMenuItemCategory.Appetizers
                },
                {
                    Text: 'Món chính',
                    Value: this.$enumeration.EnumMenuItemCategory.MainCourse
                },
                {
                    Text: 'Tráng miệng',
                    Value: this.$enumeration.EnumMenuItemCategory.Dessert
                },
                {
                    Text: 'Đồ uống',
                    Value: this.$enumeration.EnumMenuItemCategory.Drink
                }
            ]
        },

        moneyConfig() {
            return {
                decimal: ',',
                thousands: '.',
                precision: 0
            }
        }
    }
}
</script>