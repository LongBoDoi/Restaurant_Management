<template>
    <VDialog max-width="800" persistent :model-value="isShow">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                Thông tin nhân viên
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <VCardItem>
                <VForm ref="form"> 

                <MLHbox class="mt-2">
                    <!-- Mã nhân viên -->
                    <VTextField density="compact" variant="outlined" v-model:model-value="record.EmployeeCode"
                        width="40%"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Mã nhân viên
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Đơn vị tính -->
                    <VTextField width="60%" density="compact" variant="outlined" v-model:model-value="record.EmployeeName"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Tên nhân viên
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>
                </MLHbox>

                <MLHbox>
                    <!-- Số điện thoại -->
                    <VTextField width="50%" density="compact" variant="outlined" v-model:model-value="record.PhoneNumber"
                        :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                    >
                        <template v-slot:label>
                            Số điện thoại
                            <span style="color: red;">*</span>
                        </template>
                    </VTextField>

                    <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                    <!-- Vai trò -->
                    <VSelect
                        :return-object="false"
                        v-model:model-value="record.Role"
                        ref="cbMenuCategory"
                        label="Vai trò"
                        width="50%"
                        variant="outlined"
                        density="compact"
                        :items="listRole"
                        item-title="Text"
                        item-value="Value"
                    />
                </MLHbox>

                <VCheckbox v-if="editMode === $enumeration.EnumEditMode.Edit && record.UserLogin" color="primary" label="Đổi mật khẩu" v-model:model-value="record.UserLogin.IsChangePassword" />

                <MLVbox v-if="record.UserLogin && ((editMode === $enumeration.EnumEditMode.Add) || (editMode === $enumeration.EnumEditMode.Edit && record.UserLogin.IsChangePassword))">
                    <MLHbox v-if="editMode === $enumeration.EnumEditMode.Edit">
                        <!-- Mật khẩu -->
                        <VTextField width="50%" type="password" density="compact" variant="outlined" v-model:model-value="record.UserLogin.OldPassword"
                            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        >
                            <template v-slot:label>
                                Mật khẩu cũ
                                <span style="color: red;">*</span>
                            </template>
                        </VTextField>

                        <VSpacer style="width: 50%; margin-left: 8px;" class="flex-shrink-0 flex-grow-1" />
                    </MLHbox>
                    <MLHbox>
                        <!-- Mật khẩu -->
                        <VTextField width="50%" type="password" density="compact" variant="outlined" v-model:model-value="record.UserLogin.Password"
                            :rules="[(v:string|undefined) => v !== undefined && v !== '']"
                        >
                            <template v-slot:label>
                                Mật khẩu
                                <span style="color: red;">*</span>
                            </template>
                        </VTextField>

                        <VSpacer style="width: 16px;" class="flex-shrink-0 flex-grow-0" />

                        <!-- Xác nhận mật khẩu -->
                        <VTextField width="50%" type="password" density="compact" variant="outlined"
                            :rules="[
                                (v:string|undefined) => v !== undefined && v !== '',
                                (v:string) => v === record.UserLogin?.Password || 'Mật khẩu không trùng khớp.'
                            ]"
                        >
                            <template v-slot:label>
                                Xác nhận mật khẩu
                                <span style="color: red;">*</span>
                            </template>
                        </VTextField>
                    </MLHbox>
                </MLVbox>
                </VForm>
            </VCardItem>

            <VCardActions>
                <VBtn v-if="editMode === $enumeration.EnumEditMode.Edit" prepend-icon="mdi-trash-can-outline" color="error" @click="handleDeleteRecord">Xoá nhân viên</VBtn>

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
import { Customer, Employee, MLActionResult, UserLogin } from '@/models';
import { employeeStore } from '@/stores/employeeStore';
import { mapActions, mapState } from 'pinia';
import { VMoney } from 'v-money';

export default {
    directives: {money: VMoney},

    created() {
        EventBus.on(this.$eventName.ShowFormEmployeeDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormEmployeeDetail);
    },

    methods: {
        ...mapActions(employeeStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(employee: Employee) {
            if (employee) {
                if (employee.EditMode !== undefined) {
                    this.editMode = employee.EditMode;
                }
                if (!employee.UserLogin) {
                    employee.UserLogin = {
                        Username: '',
                        Password: ''
                    } as UserLogin;
                }

                if (employee.EditMode === this.$enumeration.EnumEditMode.Add) {
                    employee.Role = this.$enumeration.EnumRole.Waiter;
                }
                this.record = employee;
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
            if (this.record.UserLogin) {
                this.record.UserLogin.Username = this.record.EmployeeCode;
            }
            const success = await this.saveChanges('Lưu thành công');
            if (success) {
                this.isShow = false;
            }
            this.loading = false;
        },

        /**
         * Xử lý xoá món
         */
        handleDeleteRecord() {
            this.$commonFunction.showDialog({
                Title: 'Xác nhận xoá nhân viên',
                Message: `Bạn có chắc chắn muốn xoá nhân viên <b>${this.record.EmployeeCode}</b> không?`,
                ConfirmAction: async () => {
                    this.record.EditMode = this.$enumeration.EnumEditMode.Delete;
                    const success = await this.saveChanges('Xoá nhân viên thành công');

                    if (success) {
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
                result = await this.$service.EmployeeService.saveChanges(this.record);
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
            };

            return result.Success;
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <Employee>{} as Employee,
            oldRecord: <Employee>{} as Employee
        }
    },

    computed: {
        ...mapState(employeeStore, ['dataList', 'selectedIndex']),

        listRole() {
            return [
                {
                    Text: 'Quản lý',
                    Value: this.$enumeration.EnumRole.Manager
                },
                {
                    Text: 'Thu ngân',
                    Value: this.$enumeration.EnumRole.Cashier
                },
                {
                    Text: 'Phục vụ',
                    Value: this.$enumeration.EnumRole.Waiter
                }
            ]
        }
    }
}
</script>