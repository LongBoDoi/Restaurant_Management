<template>
    <VContainer class="d-flex" style="padding: unset">
        <MLVbox style="width: 50%; height: 100%; background-color: rgb(var(--v-theme-primary)); align-items: center; justify-content: center;">
            <VSheet color="primary" style="display: flex; align-items: center; flex-direction: column;">
                <VLabel style="font-size: 2rem; color: rgb(var(--v-theme-on-primary)); font-weight: bold;">Dành cho khách hàng</VLabel>

                <VSpacer style="height: 24px; flex-grow: 0; flex-shrink: 0;" />

                <VTabs :disabled="customerLoading" style="width: 100%;" fixedTabs v-model:modelValue="customerTab">
                    <VTab :value="0">Đăng nhập</VTab>
                    <VTab :value="1">Đăng ký</VTab>
                </VTabs>

                <VSpacer style="height: 16px; flex-grow: 0; flex-shrink: 0;" />

                <VTabsWindow :modelValue="customerTab" style="width: 400px;">
                    <VTabsWindowItem :value="0">
                        <VSheet color="primary">
                            <VForm :disabled="customerLoading" ref="formCustomerLogin">
                                <VTextField :rules="[textFieldRequireRule]" label="Số điện thoại" variant="underlined" v-model:model-value="customerUserLogin.Username" />
                                <VTextField :rules="[textFieldRequireRule]" type="password" label="Mật khẩu" variant="underlined" v-model:model-value="customerUserLogin.Password" />
                            </VForm>

                            <VLabel class="text-error" v-if="customerLoginError">{{ customerLoginError }}</VLabel>

                            <VCardActions>
                                <VBtn style="background-color: rgb(var(--v-theme-background)); color: rgb(var(--v-theme-primary)); font-weight: bold; margin-left: auto;" @click="handleCustomerLogin">Đăng nhập</VBtn>
                            </VCardActions>
                            <VProgressLinear v-if="customerLoading" indeterminate />
                        </VSheet>
                    </VTabsWindowItem>
                    <VTabsWindowItem :value="1">
                        <VSheet color="primary">
                            <VForm :disabled="customerLoading" ref="formCustomerRegister">
                                <VTextField ref="txtCustomerNameSignup" :rules="[textFieldRequireRule]" label="Họ tên" variant="underlined" />
                                <VTextField ref="txtCustomerPhoneSignup" :rules="[textFieldRequireRule]" :error="customerPhoneSignupError !== ''" :errorMessages="customerPhoneSignupError" label="Số điện thoại" variant="underlined" @update:modelValue="customerPhoneSignupError = ''" />
                                <VTextField ref="txtCustomerPasswordSignup" :rules="[textFieldRequireRule]" type="password" label="Mật khẩu" variant="underlined" />
                                <VTextField ref="txtCustomerConfirmPasswordSignup" :rules="[textFieldRequireRule, confirmPasswordRule]" type="password" label="Nhập lại mật khẩu" variant="underlined" />

                                <VCardActions>
                                    <VBtn :disabled="customerLoading" style="background-color: rgb(var(--v-theme-background)); color: rgb(var(--v-theme-primary)); font-weight: bold; margin-left: auto;" @click="handleCustomerSignup">Đăng ký</VBtn>
                                </VCardActions>
                            </VForm>
                            <VProgressLinear v-if="customerLoading" indeterminate />
                        </VSheet>
                    </VTabsWindowItem>
                </VTabsWindow>
            </VSheet>
        </MLVbox>
        <MLVbox style="width: 50%; height: 100%; align-items: center; justify-content: center;">
            <VSheet style="width: 400px; min-height: 328px; background-color: transparent; display: flex; align-items: center; flex-direction: column;">
                <VLabel style="font-size: 2rem; color: rgb(var(--v-theme-primary)); font-weight: bold;">Dành cho nhân viên</VLabel>

                <VSpacer style="height: 80px; flex-grow: 0; flex-shrink: 0;" />

                <VForm :disabled="employeeLoading" ref="formLoginCustomer" style="width: 100%;">
                    <VTextField ref="txtUsernameEmployee" label="Tên đăng nhập" variant="underlined" />
                    <VTextField @keypress.enter="handleEmployeeLogin" ref="txtPasswordEmployee" type="password" label="Mật khẩu" variant="underlined" />
                </VForm>

                <VLabel v-if="employeeRequestMessage" :class="`text-${employeeRequestStatus}`">{{ employeeRequestMessage }}</VLabel>

                <VCardActions style="width: 100%; margin-top: 8px;">
                    <VBtn :disabled="employeeLoading" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary)); font-weight: bold; margin-left: auto;" @click="handleEmployeeLogin">Đăng nhập</VBtn>
                </VCardActions>

                <VProgressLinear v-if="employeeLoading" color="primary" indeterminate />
            </VSheet>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
import { EnumApplicationErrorCode } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import { Customer, MLActionResult, UserLogin } from '@/models';

export default {
    data() {
        return {
            customerLoading: <boolean>false,
            employeeLoading: <boolean>false,

            employeeRequestStatus: <''|'success'|'error'>'',
            employeeRequestMessage: <string>'',

            customerTab: <number>0,

            customerPhoneSignupError: <string>'',

            customerUserLogin: <UserLogin>{} as UserLogin,
            customerLoginError: <string>'',
        }
    },

    methods: {
        async handleCustomerLogin() {
            const form:any = this.$refs.formCustomerLogin;
            let isValid:boolean = false;

            isValid = (await form.validate()).valid;
            if (!isValid) return;

            this.customerLoading = true;

            let result:MLActionResult|undefined = undefined;
            try {
                result = await this.$service.UserLoginService.loginCustomer(this.customerUserLogin);
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.customerLoading = false;
            }

            if (!result) return;

            if (result.Success) {
                EventBus.emit('ShowToastMessage', {
                    Message: 'Đăng nhập thành công',
                    Type: 'success'
                });
                window.location.reload();
            } else {
                EventBus.emit('ShowToastMessage', {
                    Message: result.ErrorMsg,
                    Type: 'error'
                });
            }
        },

        /**
         * Xử lý đăng ký khách hàng
         */
        async handleCustomerSignup() {
            const form:any = this.$refs.formCustomerRegister;
            let isValid:boolean = false;

            isValid = (await form.validate()).valid;
            if (!isValid) return;

            this.customerLoading = true;

            const customer = {
                CustomerName: (this.$refs.txtCustomerNameSignup as any).value,
                PhoneNumber: (this.$refs.txtCustomerPhoneSignup as any).value,
                UserLogin: {
                    Username: (this.$refs.txtCustomerPhoneSignup as any).value,
                    Password: (this.$refs.txtCustomerPasswordSignup as any).value
                } as UserLogin
            } as Customer;

            let result:MLActionResult|undefined = undefined;
            try {
                result = await this.$service.UserLoginService.registerNewCustomer(customer);
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.customerLoading = false;
            }

            if (!result) return;

            if (result.Success) {
                EventBus.emit('ShowToastMessage', {
                    Message: 'Đăng ký thành công',
                    Type: 'success'
                })
            } else {
                switch (result.ErrorCode) {
                    case EnumApplicationErrorCode.DuplicateLoginInfo:
                        this.customerPhoneSignupError = 'Số điện thoại đã được đăng ký.';
                        break;
                }
            }
        },

        /**
         * Xử lý đăng nhập nhân viên
         */
        async handleEmployeeLogin() {
            const form:any = this.$refs.formLoginCustomer;
            const isFormValid:boolean = (await form.validate()).valid;

            if (!isFormValid) return;

            this.employeeLoading = true;

            const userLogin = {
                Username: (this.$refs.txtUsernameEmployee as any).value,
                Password: (this.$refs.txtPasswordEmployee as any).value
            } as UserLogin;
            
            let result:MLActionResult|undefined = undefined;
            try {
                result = await this.$service.UserLoginService.loginEmployee(userLogin);
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.employeeLoading = false;
            }

            if (!result) return;
            if (result.Success) {
                this.employeeRequestMessage = 'Đăng nhập thành công';
                this.employeeRequestStatus = 'success';

                window.location.reload();
            } else {
                this.employeeRequestMessage = result.ErrorMsg ?? '';
                this.employeeRequestStatus = 'error';
            }
        }
    },

    computed: {
        textFieldRequireRule() {
            return (str:string|undefined) => {
                return str !== undefined && str !== '';
            }
        },

        confirmPasswordRule() {
            return (str:string) => {
                const txtPassword = this.$refs.txtCustomerPasswordSignup as any
                return str === txtPassword.value || 'Mật khẩu không trùng khớp.';
            }
        },
    }
}
</script>