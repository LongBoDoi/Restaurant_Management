<template>
    <VDialog max-width="800" persistent :model-value="isShow">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                Thông tin nguyên liệu
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem>
                <VForm ref="form"> 
                <!-- Tên nguyên liệu -->
                <VTextField class="mt-2" density="compact" variant="outlined" v-model:model-value="record.Name"
                    :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                >
                    <template v-slot:label>
                        Tên nguyên liệu
                        <span style="color: red;">*</span>
                    </template>
                </VTextField>

                <MLHbox>
                    <!-- Số lượng tồn -->
                    <VTextField width="40%" type="number" hide-spin-buttons density="compact" variant="outlined" v-model:model-value="record.Quantity"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Số lượng tồn
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Đơn vị tính -->
                    <VTextField width="60%" density="compact" variant="outlined" v-model:model-value="record.Unit"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Đơn vị tính
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>
                </MLHbox>
                </VForm>
            </VCardItem>

            <VCardActions>
                <VBtn v-if="editMode === $enumeration.EnumEditMode.Edit" prepend-icon="mdi-trash-can-outline" color="error" @click="handleDeleteRecord">Xoá nguyên liệu</VBtn>

                <VSpacer />

                <VBtn @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleSaveClick">Lưu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, InventoryItem, MLActionResult } from '@/models';
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
        handleShowDialog(inventoryItem: InventoryItem) {
            if (inventoryItem) {
                if (inventoryItem.EditMode !== undefined) {
                    this.editMode = inventoryItem.EditMode;
                }

                this.record = inventoryItem;
                this.oldRecord = JSON.parse(JSON.stringify(this.record));
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.record.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    this.dataList[this.selectedIndex] = this.oldRecord;
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
            if (await this.saveChanges('Lưu thành công')) {
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Xử lý xoá món
         */
        handleDeleteRecord() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá nguyên liệu',
                Message: `Bạn có chắc chắn muốn xoá nguyên liệu <b>${this.record.Name}</b> không?`,
                ConfirmAction: async () => {
                    this.record.EditMode = this.$enumeration.EnumEditMode.Delete;
                    if (await this.saveChanges('Xoá nguyên liệu thành công')) {
                        this.isShow = false;
                        this.removeSelectedRecord();
                    }
                }
            });
        },

        /**
         * Lưu thông tin khách hàng
         * @param confirmMessage Câu thông báo khi lưu thành công
         */
        async saveChanges(confirmMessage: string):Promise<boolean> {
            let result:MLActionResult|undefined = undefined;

            try {
                result = await this.$service.InventoryItemService.saveChanges(this.record);
            } catch (e) {
                this.$commonFunction.handleException(e);
            }

            if (!result) return false;

            if (result.Success) {
                this.dataList[this.selectedIndex] = result.Data as Customer;
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

            record: <InventoryItem>{} as InventoryItem,
            oldRecord: <InventoryItem>{} as InventoryItem
        }
    },

    computed: {
        ...mapState(inventoryItemStore, ['dataList', 'selectedIndex']),
    }
}
</script>