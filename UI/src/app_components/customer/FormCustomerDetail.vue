<template>
    <VDialog max-width="800" persistent :model-value="isShow">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                Thông tin khách hàng
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem>
                <VForm ref="form"> 
                <MLHbox class="mt-2">
                    <!-- Tên khách hàng -->
                    <VTextField width="70%" density="compact" variant="outlined" suffix="đ" v-model:model-value="customer.CustomerName"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Tên khách hàng
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Số điện thoại -->
                    <VTextField width="70%" density="compact" variant="outlined" suffix="đ" v-model:model-value="customer.PhoneNumber"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Số điện thoại
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>
                </MLHbox>

                <!-- Địa chỉ -->
                <VTextField 
                    density="compact" 
                    class="flex-grow-1 flex-shrink-0" 
                    variant="outlined"
                    v-model:model-value="customer.Address"
                >
                    <template v-slot:label>
                        Địa chỉ
                    </template>
                </VTextField>
                </VForm>
            </VCardItem>

            <VCardActions>
                <VBtn v-if="editMode === $enumeration.EnumEditMode.Edit" prepend-icon="mdi-trash-can-outline" color="error" @click="handleDeleteMenuItem">Xoá khách hàng</VBtn>

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
        handleShowDialog(customer: Customer) {
            if (customer) {
                if (customer.EditMode !== undefined) {
                    this.editMode = customer.EditMode;
                }

                this.customer = customer;
                this.oldCustomer = JSON.parse(JSON.stringify(this.customer));
                this.isShow = true;
            }
        },

        handleCloseDialog() {
            switch (this.customer.EditMode) {
                case this.$enumeration.EnumEditMode.Add:
                    this.removeSelectedRecord();
                    break;
                case this.$enumeration.EnumEditMode.Edit:
                    this.dataList[this.selectedIndex] = this.oldCustomer;
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
            await this.saveChanges('Lưu thành công');
            this.loading = false;

            this.isShow = false;
        },

        /**
         * Xử lý xoá món
         */
        handleDeleteMenuItem() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá khách hàng',
                Message: `Bạn có chắc chắn muốn xoá khách hàng <b>${this.customer.CustomerName}</b> không?`,
                ConfirmAction: async () => {
                    this.customer.EditMode = this.$enumeration.EnumEditMode.Delete;
                    await this.saveChanges('Xoá khách hàng thành công');

                    this.isShow = false;
                    this.removeSelectedRecord();
                }
            });
        },

        /**
         * Lưu thông tin khách hàng
         * @param confirmMessage Câu thông báo khi lưu thành công
         */
        async saveChanges(confirmMessage: string) {
            let result:MLActionResult|undefined = undefined;

            try {
                result = await this.$service.CustomerService.saveChanges(this.customer);
            } catch (e) {
                this.$commonFunction.handleException(e);
            }

            if (!result) return;

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
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            customer: <Customer>{} as Customer,
            oldCustomer: <Customer>{} as Customer
        }
    },

    computed: {
        ...mapState(customerStore, ['dataList', 'selectedIndex']),
    }
}
</script>