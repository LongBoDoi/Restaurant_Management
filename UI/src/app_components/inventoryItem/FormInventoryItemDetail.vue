<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Nguyên liệu</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <!-- Tên nguyên liệu -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Tên nguyên liệu <span style="color: red;">*</span></label>
                        
                        <VTextField color="primary" class="mt-2" density="compact" variant="outlined" v-model:model-value="record.Name" hide-details
                            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        />
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <!-- Số lượng tồn -->
                        <div>
                            <label class="block text-gray-700 text-sm font-medium">Số lượng tồn</label>
                            <MLNumberField color="primary" class="mt-2 text-right" hide-spin-buttons density="compact" variant="outlined" v-model:model-value="record.Quantity" hide-details />
                        </div>

                        <!-- Đơn vị tính -->
                        <div>
                            <label class="block text-gray-700 text-sm font-medium">Đơn vị tính <span style="color: red;">*</span></label>
                            <!-- Đơn vị tính -->
                            <VTextField color="primary" density="compact" variant="outlined" class="mt-2" v-model:model-value="record.Unit" hide-details
                                :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                            />
                        </div>
                    </div>

                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">
                            Số lượng tồn kho cảnh báo
                        </label>
                        <div class="flex items-center mt-2">
                            <MLNumberField color="primary" class="text-right w-full" hide-spin-buttons density="compact" variant="outlined" v-model:model-value="record.WarningStockQuantity" hide-details
                                :rules="[(v: number) => v >= 0]"
                            />
                            <div class="ml-2 text-gray-500 text-sm">
                                <i>Nguyên liệu sẽ được đánh dấu là "Sắp hết hàng" khi tồn kho ít hơn số lượng này.</i>
                            </div>
                        </div>
                    </div>

                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">
                            Nhóm nguyên liệu
                        </label>
                        
                        <VSelect
                            color="primary"
                            :items="lstInventoryItemCategories"
                            item-title="InventoryItemCategoryName"
                            item-value="InventoryItemCategoryID"
                            v-model:model-value="record.InventoryItemCategoryID"
                            v-on:update:model-value="(value: string) => {
                                const inventoryItemCategory = lstInventoryItemCategories.find(x => x.InventoryItemCategoryID === value);
                                if (inventoryItemCategory) {
                                    record.InventoryItemCategory = inventoryItemCategory;
                                }
                            }"

                            density="compact"
                            variant="outlined"
                            hide-details
                            class="mt-2"
                            no-data-text="Không có dữ liệu."
                        />
                    </div>

                    <VCheckbox color="primary" class="text-gray-700" style="opacity: 1;" label="Ngừng hoạt động" hide-details v-model:model-value="record.Inactive" />
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Lưu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { InventoryItem, MLActionResult } from '@/models';
import InventoryItemCategory from '@/models/InventoryItemCategory';
import { inventoryItemStore } from '@/stores/inventoryItemStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormInventoryItemDetail, this.handleShowDialog as any);


    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormInventoryItemDetail);
    },

    methods: {
        ...mapActions(inventoryItemStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: InventoryItem) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.$service.InventoryItemCategoryService.getAll().then((data: InventoryItemCategory[]) => {
                    this.lstInventoryItemCategories = data.filter(x => !x.Inactive);
                });

                this.record = record;
                Object.assign(this.oldRecord, this.record);
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
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
            
            const actionResult:MLActionResult = await this.$service.InventoryItemService.saveChanges(this.record);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: 'Lưu thành công.',
                    Type: 'success'
                });

                this.isShow = false;
            }

            this.loading = false;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <InventoryItem>{} as InventoryItem,
            oldRecord: <InventoryItem>{} as InventoryItem,

            lstInventoryItemCategories: <InventoryItemCategory[]>[]
        }
    },

    computed: {
        ...mapState(inventoryItemStore, ['dataList', 'selectedIndex']),
    }
}
</script>