<template>
    <VDialog width="480" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Đổi mật khẩu</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="isShow = false;" />
            </VCardTitle>

            <VForm ref="form">
            <div class="pa-6 space-y-4">
                <div>
                    <label class="block text-sm font-medium text-gray-700">
                        Mật khẩu hiện tại
                    </label>
                    <VTextField
                        hide-details
                        variant="outlined"
                        density="compact"
                        color="primary"

                        type="password"
                        class="w-full mt-1"
                        placeholder="Nhập mật khẩu hiện tại"
                        :rules="[$commonValue.textFieldRequireRule]"
                        v-model:model-value="data.OldPassword"
                    />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700">
                        Mật khẩu mới
                    </label>
                    <VTextField
                        hide-details
                        variant="outlined"
                        density="compact"
                        color="primary"

                        type="password"
                        class="w-full mt-1"
                        placeholder="Nhập mật khẩu mới"
                        :rules="[$commonValue.textFieldRequireRule, $commonValue.textPasswordRule]"
                        v-model:model-value="data.Password"
                    />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700">
                        Xác nhận mật khẩu
                    </label>
                    <VTextField
                        variant="outlined"
                        density="compact"
                        color="primary"

                        type="password"
                        class="w-full mt-1"
                        placeholder="Xác nhận mật khẩu mới"
                        :rules="[$commonValue.textFieldRequireRule, (value: string) => {
                            return value.trim() === data.Password.trim() || 'Mật khẩu không trùng khớp.'
                        }]"
                    />
                </div>

                <div class="bg-yellow-50 border border-yellow-100 rounded-lg pa-3 text-sm text-yellow-700">
                    <div class="flex">
                        <VIcon icon="mdi-information-variant-circle-outline" class="mr-2" style="margin-top: 2px;" />
                        <p>
                            Mật khẩu phải chứa ít nhất 08 ký tự, bao gồm ít nhất 01 ký tự viết hoa, 01 ký tự viết thường, 01 chữ số và 01 ký tự đặc biệt.
                        </p>
                    </div>
                </div>
            </div>
            </VForm>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="isShow = false">Đóng</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Cập nhật</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { Customer, Employee, UserLogin } from '@/models';

export default {
    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            data: <UserLogin>{
                Username: ''
            } as UserLogin
        }
    },

    created() {
        EventBus.on(this.$eventName.ShowFormChangePassword, () => {
            this.isShow = true;
        });
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormChangePassword);
    },

    methods: {
        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            this.data.CustomerID = (this.$session.UserData as Customer)?.CustomerID;
            this.data.EmployeeID = (this.$session.UserData as Employee)?.EmployeeID;

            const success = await this.$service.UserLoginService.changePassword(this.data);
            if (success) {
                this.$commonFunction.showToastMessage('Đổi mật khẩu thành công', 'success');
                window.location.pathname = '/login';
            }

            this.loading = false;
        }
    }
}
</script>