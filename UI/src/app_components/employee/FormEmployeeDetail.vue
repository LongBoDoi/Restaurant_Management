<template>
    <VDialog width="600" max-height="90vh" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Nhân viên</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6">
                <VForm ref="form">
                    <!-- Ảnh nhân viên -->
                    <div class="mb-6 flex items-center justify-center">
                        <div class="relative">
                            <div style="width: 96px; height: 96px; border: 4px solid rgb(220 252 231);" class="rounded-full bg-gray-200 flex items-center justify-center overflow-hidden border-4 border-green-100">
                                <img v-if="imageUrl" :src="imageUrl" >
                                <VIcon v-else class="text-gray-400" icon="mdi-account-outline" style="font-size: 48px;" />
                            </div>

                            <VBtn 
                                style="width: fit-content !important; height: fit-content !important; padding: 8px !important; font-size: 10px !important;"
                                class="absolute bottom-0 right-0 bg-green-500 text-white hover:bg-green-600 transition-all duration-200 transform hover:scale-105"
                                icon="mdi-pencil-outline" 
                                @click="selectImage"
                            />

                            <VFileInput
                                ref="imageSelector"
                                v-show="false"
                                accept="image/*"
                                v-model:model-value="imageFile"
                                v-on:update:model-value="getImageUrls"
                            />
                        </div>
                    </div>
                    
                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div>
                            <!-- Mã nhân viên -->
                            <label class="block text-sm font-medium text-gray-700">Mã nhân viên <span style="color: red;">*</span></label>
                            <VTextField class="mt-1" density="compact" variant="outlined" v-model:model-value="record.EmployeeCode" hide-details color="primary"
                                :rules="[$commonValue.textFieldRequireRule, $commonValue.textNoSpaceRule]"
                            />
                            <div class="text-xs text-gray-500 mt-1">
                                <i>Mã nhân viên sẽ được dùng làm tên đăng nhập cho nhân viên.</i>
                            </div>
                        </div>

                        <div>
                            <!-- Họ tên -->
                            <label class="block text-sm font-medium text-gray-700">Họ tên <span style="color: red;">*</span></label>
                            <VTextField class="mt-1" density="compact" variant="outlined" v-model:model-value="record.EmployeeName" hide-details color="primary"
                                :rules="[$commonValue.textFieldRequireRule]"
                            />
                        </div>
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div>
                            <!-- Email -->
                            <label class="block text-sm font-medium text-gray-700">Email</label>
                            <VTextField class="mt-1" density="compact" variant="outlined" v-model:model-value="record.Email" hide-details color="primary" :rules="[$commonValue.textEmailRule]" />
                        </div>

                        <div>
                            <!-- Số điện thoại -->
                            <label class="block text-sm font-medium text-gray-700">Số điện thoại</label>
                            <VTextField class="mt-1" density="compact" variant="outlined" hide-details color="primary"
                                v-mask="'0### ### ###'"
                                :model-value="record.PhoneNumber"
                                v-on:update:model-value="record.PhoneNumber = $commonFunction.getRealPhoneNumberValue($event);"
                            />
                        </div>
                    </div>

                    <div class="grid grid-cols-2 gap-4 mb-4">
                        <div>
                            <!-- Trạng thái -->
                            <label class="block text-sm font-medium text-gray-700">Trạng thái làm việc</label>
                            <VSelect
                                :items="lstWorkStatus"
                                item-title="Text"
                                item-value="Value"
                                :return-object="false"
                                hide-details
                                v-model:model-value="record.WorkStatus"

                                density="compact"
                                variant="outlined"
                                color="primary"
                                class="mt-1"
                            />
                        </div>

                        <div>
                            <!-- Vai trò -->
                            <label class="block text-sm font-medium text-gray-700">Vai trò <span style="color: red;">*</span></label>
                            <VSelect
                                :items="allRoles"
                                item-title="RoleName"
                                item-value="RoleID"
                                :return-object="true"
                                hide-details
                                multiple
                                chips
                                v-model:model-value="record.Roles"

                                density="compact"
                                variant="outlined"
                                color="primary"
                                class="mt-1"
                                :rules="[(roles: Role[]) => {
                                    return roles.length > 0;
                                }]"
                            />
                        </div>
                    </div>

                    <div v-if="editMode === $enumeration.EnumEditMode.Add" class="mb-4">
                        <div class="grid grid-cols-2 gap-4">
                            <div>
                                <!-- Mật khẩu -->
                                <label class="block text-sm font-medium text-gray-700">Mật khẩu</label>
                                <VTextField type="password" class="mt-1" density="compact" variant="outlined" hide-details color="primary"
                                    :rules="[$commonValue.textFieldRequireRule, $commonValue.textPasswordRule]"
                                    v-on:update:model-value="(value: string) => {
                                        if (record.UserLogin) {
                                            record.UserLogin.Password = value;
                                        }
                                    }"
                                />
                            </div>

                            <div>
                                <!-- Xác nhận mật khẩu -->
                                <label class="block text-sm font-medium text-gray-700">Xác nhận mật khẩu</label>
                                <VTextField type="password" class="mt-1" density="compact" variant="outlined" hide-details color="primary"
                                    :rules="[
                                        (v) => {
                                            return v !== undefined && v !== '' && v === record.UserLogin?.Password;
                                        }
                                    ]"
                                />
                            </div>
                        </div>

                        <div class="text-xs text-gray-500 mt-1">
                            <i>Mật khẩu phải chứa ít nhất 08 ký tự, bao gồm ít nhất 01 ký tự viết hoa, 01 ký tự viết thường, 01 chữ số và 01 ký tự đặc biệt.</i>
                        </div>
                    </div>
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
import { Employee, MLActionResult } from '@/models';
import Role from '@/models/Role';
import { employeeStore } from '@/stores/employeeStore';
import { mapActions, mapState } from 'pinia';

export default {
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
         handleShowDialog(record: Employee) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.imageUrl = this.$commonFunction.getImageUrl(record.ImageUrl);

                this.record = record;
                this.record.Roles = [];
                Object.assign(this.oldRecord, this.record);
                this.isShow = true;

                this.loadAllSubData();
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

            this.record.PhoneNumber = this.record.PhoneNumber?.replace(/\D/g, "") ?? '';

            if (this.record.UserLogin) {
                this.record.UserLogin.Username = this.record.EmployeeCode;
            }
            
            const actionResult:MLActionResult = await this.$service.EmployeeService.updateEmployee(this.record, this.imageFile);
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

        selectImage() {
            const imageSelector = (this.$refs.imageSelector as any);

            const input = imageSelector.$el.querySelector('input[type=file]');
            input.click();
        },

        getImageUrls(file:File|File[]) {
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.imageFile = file as File;
                    this.imageUrl = e.target?.result as string;
                };
                reader.readAsDataURL(file as File);
            }
        },

        loadAllSubData() {
            this.loading = true;

            var servicesCalled:number = 0;
            this.subData.forEach((subData:any) => {
                subData().then(() => {
                    servicesCalled++;
                    if (servicesCalled >= this.subData.length) {
                        this.loading = false;
                    }
                });
            });
        }
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            editMode: <EnumEditMode>EnumEditMode.Add,

            record: <Employee>{} as Employee,
            oldRecord: <Employee>{} as Employee,

            imageFile: <File|undefined>undefined,
            imageUrl: <string>'',

            allRoles: <Role[]>[]
        }
    },

    computed: {
        ...mapState(employeeStore, ['dataList', 'selectedIndex']),

        lstWorkStatus() {
            return [
                {
                    Text: 'Chính thức',
                    Value: this.$enumeration.EnumEmployeeWorkStatus.Active
                },
                {
                    Text: 'Thử việc',
                    Value: this.$enumeration.EnumEmployeeWorkStatus.Probation
                },
                {
                    Text: 'Nghỉ phép',
                    Value: this.$enumeration.EnumEmployeeWorkStatus.OnLeave
                },
                {
                    Text: 'Nghỉ việc',
                    Value: this.$enumeration.EnumEmployeeWorkStatus.Terminated
                }
            ]
        },

        subData() {
            return [
                async () => {
                    if (this.editMode === this.$enumeration.EnumEditMode.Edit) {
                        const detail = await this.$service.EmployeeService.getByID(this.record.EmployeeID);
                        if (detail) {
                            this.record.Roles = detail.Roles;
                        }
                    }
                },
                async () => {
                    const allRoles = await this.$service.RoleService.getAll();
                    this.allRoles = allRoles;
                }
            ]
        },
    }
}
</script>