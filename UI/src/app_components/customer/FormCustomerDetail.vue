<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            
            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Khách hàng</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form"> 
                    <div class="space-y-4">
                        <!-- Tên khách hàng -->
                        <div>
                            <label class="block text-sm font-medium text-gray-700">Tên khách hàng <span style="color: red;">*</span></label>
                            <VTextField class="mt-1" density="compact" variant="outlined" v-model:model-value="record.CustomerName" hide-details
                                color="primary"
                                :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                            />
                        </div>

                        <div>
                            <label class="block text-sm font-medium text-gray-700">Số điện thoại <span style="color: red;">*</span></label>
                            <VTextField class="mt-1" density="compact" variant="outlined" v-model:model-value="record.PhoneNumber" hide-details
                                v-mask="'0### ### ###'"
                                color="primary"
                                :rules="[
                                    (v:string|undefined) => v !== undefined && v !== ''
                                ]"
                            />
                        </div>

                        <div>
                            <label class="block text-sm font-medium text-gray-700">Email</label>
                            <VTextField class="mt-1" type="email" density="compact" variant="outlined" v-model:model-value="record.Email" hide-details
                                color="primary"
                                :rules="[
                                    (v:string|undefined) => !v || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)
                                ]"
                            />
                        </div>

                        <div>
                            <label class="block text-sm font-medium text-gray-700">Địa chỉ</label>
                            <VTextField class="mt-1" type="email" density="compact" variant="outlined" v-model:model-value="record.Address" hide-details color="primary" />
                        </div>
                    </div>
                </VForm>
            </VCardItem>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="handleCloseDialog">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Lưu</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import { EnumEditMode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, MLActionResult } from '@/models';
import { customerStore } from '@/stores/customerStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormCustomerDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormCustomerDetail);
    },

    methods: {
        ...mapActions(customerStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: Customer) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

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

            this.record.PhoneNumber = this.record.PhoneNumber.replace(/\D/g, "");
            
            const actionResult:MLActionResult = await this.$service.CustomerService.saveChanges(this.record);
            if (actionResult.Success) {
                Object.assign(this.record, actionResult.Data);

                EventBus.emit(this.$eventName.ShowToastMessage, {
                    Message: 'Lưu thành công.',
                    Type: 'success'
                });

                this.isShow = false;
            }

            this.loading = false;
        },
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <Customer>{} as Customer,
            oldRecord: <Customer>{} as Customer
        }
    },

    computed: {
        ...mapState(customerStore, ['dataList', 'selectedIndex']),
    }
}
</script>