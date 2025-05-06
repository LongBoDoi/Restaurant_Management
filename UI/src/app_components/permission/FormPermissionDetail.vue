<template>
    <VDialog width="500" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <!-- Header -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Vai trò</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="handleCloseDialog" />
            </VCardTitle>

            <!-- Body -->
            <VCardItem class="pa-6" style="overflow-y: auto; max-height: 75vh;">
                <VForm ref="form">
                    <!-- Mã vai trò -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Mã vai trò <span style="color: red;">*</span></label>

                        <VTextField
                            hide-details
                            variant="outlined"
                            density="compact"
                            class="mt-1"
                            color="primary"

                            v-model:model-value="record.RoleCode"
                            :disabled="editMode === $enumeration.EnumEditMode.Edit && oldRecord.RoleCode === 'AD'"
                            :rules="[$commonValue.textFieldRequireRule]"
                        />
                    </div>

                    <!-- Tên vai trò -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Tên vai trò <span style="color: red;">*</span></label>

                        <VTextField
                            hide-details
                            variant="outlined"
                            density="compact"
                            class="mt-1"
                            color="primary"

                            v-model:model-value="record.RoleName"
                            :disabled="editMode === $enumeration.EnumEditMode.Edit && oldRecord.RoleCode === 'AD'"
                            :rules="[$commonValue.textFieldRequireRule]"
                        />
                    </div>

                    <!-- Nhân viên -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Nhân viên</label>

                        <VSelect
                            hide-details
                            variant="outlined"
                            density="compact"
                            class="mt-1"
                            color="primary"
                            no-data-text="Không có dữ liệu"
                            multiple
                            chips
                            return-object

                            :items="allEmployees"
                            item-title="EmployeeName"
                            v-model:model-value="record.Employees"
                        />
                    </div>

                    <!-- Phân quyền -->
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-medium">Phân quyền</label>
                        <VCard class="border border-gray-300 rounded-lg divide-y overflow-hidden mt-1" style="box-shadow: unset !important;" 
                            :disabled="editMode === $enumeration.EnumEditMode.Edit && oldRecord.RoleCode === 'AD'"
                        >
                            <!-- Thực đơn -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all" style="border-top: unset;">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-amber-500 mr-2">
                                            menu_book
                                        </span>
                                        <span class="font-medium">Thực đơn</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageMenu" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Nguyên liệu -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-green-500 mr-2">
                                            inventory
                                        </span>
                                        <span class="font-medium">Nguyên liệu</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageInventory" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Bàn, đặt bàn -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-blue-500 mr-2">
                                            table_restaurant
                                        </span>
                                        <span class="font-medium">Bàn, đặt bàn</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageTable" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Order -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-purple-500 mr-2">
                                            receipt_long
                                        </span>
                                        <span class="font-medium">Order</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageOrder" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Khách hàng -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-indigo-500 mr-2">
                                            groups
                                        </span>
                                        <span class="font-medium">Khách hàng</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageCustomer" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Nhân viên -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-orange-500 mr-2">
                                            badge
                                        </span>
                                        <span class="font-medium">Nhân viên</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManageEmployee" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Phân quyền -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-teal-500 mr-2">
                                            security
                                        </span>
                                        <span class="font-medium">Phân quyền</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManagePermission" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Xem báo cáo -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-red-500 mr-2">
                                            bar_chart
                                        </span>
                                        <span class="font-medium">Xem báo cáo</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManagePermission" v-model="record.PermissionCodes" />
                                </div>
                            </div>

                            <!-- Xem báo cáo -->
                            <div class="px-3 py-1 hover:bg-gray-50 transition-all">
                                <div class="flex items-center justify-between">
                                    <div class="flex items-center">
                                        <span class="material-symbols-outlined text-gray-500 mr-2">
                                            settings
                                        </span>
                                        <span class="font-medium">Thiết lập</span>
                                    </div>
                                    
                                    <VSwitch color="primary" hide-details density="compact" class="mr-1" value="ManagePermission" v-model="record.PermissionCodes" />
                                </div>
                            </div>
                        </VCard>
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
import Permission from '@/models/Permission';
import Role from '@/models/Role';
import { roleStore } from '@/stores/roleStore';
import { mapActions, mapState } from 'pinia';

export default {
    created() {
        EventBus.on(this.$eventName.ShowFormRoleDetail, this.handleShowDialog as any);
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormRoleDetail);
    },

    methods: {
        ...mapActions(roleStore as any, ['removeSelectedRecord']),

        /**
         * Xử lý mở form
         */
        handleShowDialog(record: Role) {
            if (record) {
                if (record.EditMode !== undefined) {
                    this.editMode = record.EditMode;
                }

                this.record = record;
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

            this.record.Permissions = this.record.PermissionCodes.map(pc => this.allPermissions.find(p => p.PermissionCode === pc) ?? {} as Permission);
            
            const actionResult:MLActionResult = await this.$service.RoleService.saveChanges(this.record);
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

            record: <Role>{} as Role,
            oldRecord: <Role>{} as Role,

            allEmployees: <Employee[]>[],
            allPermissions: <Permission[]>[]
        }
    },

    computed: {
        ...mapState(roleStore, ['dataList', 'selectedIndex']),

        subData() {
            return [
                async () => {
                    if (this.editMode === this.$enumeration.EnumEditMode.Edit) {
                        const detail = await this.$service.RoleService.getByID(this.record.RoleID);
                        if (detail) {
                            this.record.Permissions = detail.Permissions;
                            this.record.PermissionCodes = detail.Permissions.map(p => p.PermissionCode);
                        }
                    }
                },
                async () => {
                    const allEmployees = await this.$service.EmployeeService.getAll();
                    this.allEmployees = allEmployees.filter(e => e.EmployeeCode !== 'admin');
                },
                async () => {
                    const allPermissions = await this.$service.PermissionService.getAll();
                    this.allPermissions = allPermissions;
                }
            ]
        },
    }
}
</script>