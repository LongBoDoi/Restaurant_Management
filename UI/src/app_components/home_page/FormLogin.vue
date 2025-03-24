<template>
    <VContainer class="d-flex flex-column justify-center">
        <h2 class="text-2xl md:text-3xl font-bold text-emerald-600 text-center mb-6 mt-6 md:mb-8">
            Đăng nhập
        </h2>

        <div class="flex flex-col md:flex-row items-center justify-center gap-8">
            <div className="backdrop-blur-sm p-8 rounded-xl shadow-2xl w-full max-w-md transition-transform duration-300 hover:translate-y-[-5px]"
                style="background-color: rgba(var(--v-theme-primary), 0.9);"
            >
                <h2 className="text-white text-2xl font-bold text-center mb-6">Dành cho khách hàng</h2>

                <VTabs fixed-tabs style="color: white;" v-model:model-value="customerTab" :disabled="customerLoading">
                    <VTab :value="0">ĐĂNG NHẬP</VTab>
                    <VTab :value="1">ĐĂNG KÝ</VTab>
                </VTabs>

                <VProgressLinear v-if="customerLoading" color="primary" indeterminate />

                <VTabsWindow v-model:model-value="customerTab">
                    <VTabsWindowItem :value="0">
                        <VForm ref="formCustomerLogin" class="space-y-6 mt-4 sm:mt-6" :disabled="customerLoading" >
                            <VTextField 
                                :rules="[textFieldRequireRule]" 
                                variant="outlined" 
                                style="
                                    color: white;
                                "
                                label="Số điện thoại"
                                bg-color="rgba(52, 211, 153, 0.5)"
                                hide-details
                                v-model:model-value="customerUserLogin.Username"
                            />
                            <VTextField 
                                type="password"
                                variant="outlined"
                                style="color: white;"
                                bg-color="rgba(52, 211, 153, 0.5)"
                                label="Mật khẩu"
                                hide-details
                                :rules="[textFieldRequireRule]"
                                v-model:model-value="customerUserLogin.Password"
                            />
                            <VBtn 
                                style="
                                    background-color: rgb(var(--v-theme-background)); 
                                    color: rgb(var(--v-theme-primary));
                                    margin-left: auto;
                                    width: 100%;
                                "
                                rounded
                                prepend-icon="mdi-login"
                                :disabled="customerLoading"
                                @click="handleCustomerLogin"
                            >
                                Đăng nhập
                            </VBtn>
                        </VForm>
                    </VTabsWindowItem>
                    <VTabsWindowItem :value="1">
                        <VForm :disabled="customerLoading" ref="formCustomerRegister" class="mt-4 sm:mt-6">
                            <VTextField ref="txtCustomerNameSignup" class="mt-2" :rules="[textFieldRequireRule]" style="color: white;" label="Họ tên" variant="outlined" bg-color="rgba(52, 211, 153, 0.5)" />
                            <VTextField ref="txtCustomerPhoneSignup" bg-color="rgba(52, 211, 153, 0.5)" style="color: white;" :rules="[textFieldRequireRule]" :error="customerPhoneSignupError !== ''" :errorMessages="customerPhoneSignupError" label="Số điện thoại" variant="outlined" @update:modelValue="customerPhoneSignupError = ''" />
                            <VTextField ref="txtCustomerPasswordSignup" bg-color="rgba(52, 211, 153, 0.5)" :rules="[textFieldRequireRule]" style="color: white;" type="password" label="Mật khẩu" variant="outlined" />
                            <VTextField ref="txtCustomerConfirmPasswordSignup" bg-color="rgba(52, 211, 153, 0.5)" :rules="[textFieldRequireRule, confirmPasswordRule]" style="color: white;" type="password" label="Nhập lại mật khẩu" variant="outlined" />

                            <VCardActions>
                                <VBtn rounded :disabled="customerLoading" style="background-color: rgb(var(--v-theme-background)); color: rgb(var(--v-theme-primary)); margin-left: auto; width: 100%;" @click="handleCustomerSignup">Đăng ký</VBtn>
                            </VCardActions>
                        </VForm>
                    </VTabsWindowItem>
                </VTabsWindow>
                <!-- <div className="mt-4 text-center">
                    <a
                        href="https://webcrumbs.cloud/placeholder"
                        className="text-white/80 hover:text-white text-sm underline transition-colors"
                    >
                        Quên mật khẩu?
                    </a>
                </div> -->
            </div>

            <div className="backdrop-blur-sm p-8 rounded-xl shadow-2xl w-full max-w-md transition-transform duration-300 hover:translate-y-[-5px]"
                style="background-color: rgba(var(--v-theme-primary), 0.9);"
            >
                <h2 className="text-white text-2xl font-bold text-center mb-6">Dành cho nhân viên</h2>

                <VForm @keypress.enter="handleEmployeeLogin" :disabled="employeeLoading" ref="formLoginCustomer" style="width: 100%;">
                    <VTextField :rules="[textFieldRequireRule]" v-model="employeeLoginData.Username" label="Tên đăng nhập" variant="outlined" style="color: white;" bg-color="rgba(52, 211, 153, 0.5)" />
                    <VTextField :rules="[textFieldRequireRule]" v-model="employeeLoginData.Password" ref="txtPasswordEmployee" type="password" label="Mật khẩu" variant="outlined" style="color: white;" bg-color="rgba(52, 211, 153, 0.5)" />
                </VForm>

                <VCardActions style="width: 100%; margin-top: 8px;">
                    <VBtn prepend-icon="mdi-badge-account-horizontal-outline" :disabled="employeeLoading" style="background-color: white; color: rgb(var(--v-theme-primary)); margin-left: auto; width: 100%;" @click="handleEmployeeLogin" rounded>Đăng nhập</VBtn>
                </VCardActions>
                <!-- <div className="mt-4 text-center">
                    <a
                        href="https://webcrumbs.cloud/placeholder"
                        className="text-white/80 hover:text-white text-sm underline transition-colors"
                    >
                        Quên mật khẩu?
                    </a>
                </div> -->
            </div>
        </div>
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

            employeeLoginData: <UserLogin>{} as UserLogin
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
                if (!this.$config.UseCookies) {
                    localStorage.setItem(this.$localStorageKey.AuthToken, result.Data);
                }
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
                });
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
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
            this.employeeLoginData.Username = this.employeeLoginData.Username ?? '';
            this.employeeLoginData.Password = this.employeeLoginData.Password ?? '';

            const form:any = this.$refs.formLoginCustomer;
            const isFormValid:boolean = (await form.validate()).valid;

            if (!isFormValid) return;

            this.employeeLoading = true;
            
            let result:MLActionResult|undefined = undefined;
            try {
                result = await this.$service.UserLoginService.loginEmployee(this.employeeLoginData);
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.employeeLoading = false;
            }

            if (!result) return;
            if (result.Success) {
                if (!this.$config.UseCookies) {
                    localStorage.setItem(this.$localStorageKey.AuthToken, result.Data);
                }
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
                return str === undefined || str !== '';
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