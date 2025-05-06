<template>
    <VDialog width="600" max-height="90vh" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Nhóm thực đơn</h2>
                <VBtn variant="text" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem class="pa-6">
                <VForm ref="form">
                    <!-- Tên nhóm thực đơn -->
                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div class="col-span-2">
                            <label class="block text-sm font-medium text-gray-700">Tên nhóm thực đơn <span style="color: red;">*</span></label>
                            <!-- Tên nhóm thực đơn -->
                            <VTextField
                                color="primary"
                                density="compact"
                                class="mt-2 flex-grow-1 flex-shrink-0" 
                                variant="outlined" 
                                v-model:model-value="record.MenuItemCategoryName"
                                hide-details
                                label=""
                                :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                            />
                        </div>
                    </div>

                    <!-- Mô tả -->
                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div class="col-span-2">
                            <label class="block text-sm font-medium text-gray-700">Mô tả</label>
                            <VTextarea
                                color="primary"
                                no-resize
                                density="compact"
                                class="mt-2 flex-grow-1 flex-shrink-0" 
                                variant="outlined" 
                                v-model:model-value="record.Description"
                                hide-details
                                label=""
                            />
                        </div>
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div class="col-span-2">
                            <label class="block text-sm font-medium text-gray-700">Màu sắc</label>

                            <MLHbox class="gap-2 ml-1 mt-2">
                                <div v-for="color in lstColors" style="width: 32px; height: 32px; border-style: solid; outline-offset: 2px; cursor: pointer;" class="rounded-full border-2 border-transparent transition-all duration-200 transform hover:scale-110"
                                    :class="[{'hover:border-gray-400' : record.Color !== color}]"
                                    :style="[
                                        {'background-color': color},
                                        {'outline': record.Color === color ? `2px solid ${color}` : 'unset'}
                                    ]"
                                    @click="record.Color = color;"
                                />

                                <div style="width: 32px; height: 32px; cursor: pointer;" class="border flex items-center justify-center rounded-full bg-gray-100 border-2 border-gray-300 hover:bg-gray-200 transition-all duration-200 hover:scale-110"
                                    @click="openFormSelectColor"
                                >
                                    <span class="material-symbols-outlined text-sm text-gray-600">add</span>
                                </div>
                            </MLHbox>
                        </div>
                    </div>

                    <VCheckbox color="primary" class="text-gray-700" hide-details label="Ngừng hoạt động" v-model:model-value="record.Inactive" />
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Lưu thay đổi</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>

    <FormSelectMenuCategoryColor v-on:addColor="onAddNewColor" />
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { MenuItemCategory, MLActionResult } from '@/models';
import { menuItemCategoryStore } from '@/stores/menuItemCategoryStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormMenuCategoryDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormMenuCategoryDetail);
    },

    methods: {
        ...mapActions(menuItemCategoryStore as any, ['setSelectedIndex']),

        /**
         * Xử lý mở form
         */
        async handleShowDialog(record: MenuItemCategory) {
            if (record) {
                this.loading = true;
                this.lstColors = JSON.parse(this.$commonFunction.getSettingValue('MenuCategoryColors'));
                this.loading = false;

                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.record = record;

                Object.assign(this.oldRecord, record);
                this.isShow = true;
            }
        },

        /**
         * Xử lý đóng form
         */
        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.dataList.splice(this.dataList.indexOf(this.record), 1);

                    this.setSelectedIndex(this.dataList[this.dataList.length - 1]?.MenuItemCategoryID ?? '');
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    Object.assign(this.record, this.oldRecord);
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

            const actionResult:MLActionResult = await this.$service.MenuItemCategoryService.saveChanges(this.record);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                this.setSelectedIndex(this.record.MenuItemCategoryID);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: 'Lưu thành công.',
                    Type: 'success'
                });

                this.isShow = false;
            }

            this.loading = false;
        },

        openFormSelectColor() {
            EventBus.emit(this.$eventName.ShowFormAddMenuCategoryColor)
        },

        onAddNewColor(colorValue: string) {
            this.lstColors.push(colorValue);

            this.record.Color = colorValue;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <MenuItemCategory>{} as MenuItemCategory,
            oldRecord: <MenuItemCategory>{} as MenuItemCategory,

            lstColors: <string[]>[]
        }
    },

    computed: {
        ...mapState(menuItemCategoryStore as any, ['selectedIndex', 'dataList']),
    }
}
</script>

<style lang="scss" scoped>
</style>