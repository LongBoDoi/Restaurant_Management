<template>
    <div class="bg-emerald-50" style="min-height: 100%;">
    <VContainer class="h-full d-flex align-center justify-center bg-emerald-50">
        <div class="py-12 md:py-24 px-4 md:px-8">
        <div class="flex justify-center items-center mb-12">
        <div class="h-0.5 bg-emerald-200 w-24"></div>
        <h2 class="text-3xl font-bold px-6 text-emerald-800 text-center">Đăng nhập</h2>
        <div class="h-0.5 bg-emerald-200 w-24"></div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-6xl mx-auto">
            <!-- Customer Login Form -->
            <VCard class="group relative w-full overflow-hidden rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 bg-emerald-200 transform hover:-translate-y-1" :disabled="customerLoading">
                <template #loader>
                    <VProgressLinear indeterminate color="primary" v-if="customerLoading" />
                </template>
                <div class="p-8">
                    <VTabsWindow :model-value="customerTab">
                        <VTabsWindowItem :value="0">
                            <h3 class="text-2xl font-bold text-emerald-800 mb-6 flex items-center">
                                <VIcon icon="mdi-account-circle" class="mr-2" />
                                Dành cho khách hàng
                            </h3>
                            <VForm class="space-y-4" ref="formCustomerLogin" @keypress.enter="handleCustomerLogin">
                                <div>
                                    <label class="block text-emerald-700 font-medium">Số điện thoại/Email</label>
                                    <VTextField
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="Nhập SĐT/Email của bạn..."
                                        v-model:model-value="customerLoginData.Username"
                                        :rules="[$commonValue.textFieldRequireRule]"
                                    />
                                </div>
                                <div>
                                    <label class="block text-emerald-700 font-medium">Mật khẩu</label>
                                    <VTextField 
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        type="password" 
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="••••••••"
                                        v-model:model-value="customerLoginData.Password"
                                        :rules="[$commonValue.textFieldRequireRule]"
                                    />
                                </div>
                                <div class="flex items-center justify-between" v-if="false">
                                    <div class="flex items-center">
                                        <input 
                                        id="customer-remember" 
                                        type="checkbox" 
                                        class="w-4 h-4 text-emerald-600 border-emerald-300 rounded focus:ring-emerald-500"
                                        />
                                        <label htmlFor="customer-remember" class="ml-2 text-emerald-700">Remember me</label>
                                    </div>
                                    <a href="https://webcrumbs.cloud/placeholder" class="text-emerald-600 hover:text-emerald-800 hover:underline transition-all">Forgot password?</a>
                                </div>
                                <VBtn
                                    @click="handleCustomerLogin"
                                    style="height: 48px !important;"
                                    class="w-full bg-emerald-700 hover:bg-emerald-600 text-white px-4 py-3 rounded-lg font-semibold transition-all duration-300 transform hover:-translate-y-1 active:translate-y-0 shadow-md hover:shadow-lg"
                                >
                                    Đăng nhập
                                </VBtn>
                                
                                <div class="mt-6 text-center">
                                    <p class="text-emerald-700">Bạn chưa có tài khoản?
                                    <span @click="customerTab = 1" style="text-decoration: underline; cursor: pointer;" class="text-emerald-600 font-medium hover:text-emerald-800 hover:underline ml-1 transition-all">
                                        Đăng ký ngay
                                    </span>
                                    </p>
                                </div>
                            </VForm>
                        </VTabsWindowItem>

                        <VTabsWindowItem :value="1">
                            <h3 class="text-2xl font-bold text-emerald-800 mb-6 flex items-center">
                                <VIcon icon="mdi-account-circle" class="mr-2" />
                                Dành cho khách hàng
                            </h3>
                            <VForm class="space-y-4" ref="formCustomerRegister" @keypress.enter="handleCustomerSignup">
                                <div>
                                    <label class="block text-emerald-700 font-medium">Họ tên</label>
                                    <VTextField
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="Nhập họ tên của bạn..."
                                        :rules="[$commonValue.textFieldRequireRule]"
                                        v-model:model-value="customerSignupData.CustomerName"
                                    />
                                </div>
                                <div>
                                    <label class="block text-emerald-700 font-medium">Số điện thoại</label>
                                    <VTextField
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="Nhập SĐT của bạn..."
                                        :rules="[$commonValue.textFieldRequireRule]"
                                        v-mask="'0### ### ###'"
                                        v-on:update:model-value="(v:string|undefined) => {
                                            customerSignupData.PhoneNumber = $commonFunction.getRealPhoneNumberValue(v);
                                        }"
                                    />
                                </div>
                                <div>
                                    <label class="block text-emerald-700 font-medium">Email</label>
                                    <VTextField
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="Nhập Email của bạn..."
                                        v-model:model-value="customerSignupData.Email"
                                    />
                                </div>
                                <div v-if="customerSignupData.UserLogin !== undefined">
                                    <label class="block text-emerald-700 font-medium">Mật khẩu</label>
                                    <VTextField 
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        type="password" 
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="••••••••"
                                        :rules="[$commonValue.textFieldRequireRule, $commonValue.textPasswordRule]"
                                        v-model:model-value="customerSignupData.UserLogin.Password"
                                    />
                                </div>
                                <div v-if="customerSignupData.UserLogin !== undefined">
                                    <label class="block text-emerald-700 font-medium">Xác nhận mật khẩu</label>
                                    <VTextField 
                                        hide-details
                                        variant="outlined"
                                        density="comfortable"
                                        type="password" 
                                        class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                                        placeholder="••••••••"
                                        :rules="[$commonValue.textFieldRequireRule, (v:string|undefined) => {
                                            return v === customerSignupData.UserLogin?.Password;
                                        }]"
                                    />
                                </div>
                                <div class="text-xs text-gray-500 mt-1">
                                    <i>Mật khẩu phải chứa ít nhất 08 ký tự, bao gồm ít nhất 01 ký tự viết hoa, 01 ký tự viết thường, 01 chữ số và 01 ký tự đặc biệt.</i>
                                </div>
                                <div class="flex items-center justify-between" v-if="false">
                                    <div class="flex items-center">
                                        <input 
                                        id="customer-remember" 
                                        type="checkbox" 
                                        class="w-4 h-4 text-emerald-600 border-emerald-300 rounded focus:ring-emerald-500"
                                        />
                                        <label htmlFor="customer-remember" class="ml-2 text-emerald-700">Remember me</label>
                                    </div>
                                    <a href="https://webcrumbs.cloud/placeholder" class="text-emerald-600 hover:text-emerald-800 hover:underline transition-all">Forgot password?</a>
                                </div>
                                <VBtn
                                    @click="handleCustomerSignup"
                                    style="height: 48px !important;"
                                    class="w-full bg-emerald-700 hover:bg-emerald-600 text-white px-4 py-3 rounded-lg font-semibold transition-all duration-300 transform hover:-translate-y-1 active:translate-y-0 shadow-md hover:shadow-lg"
                                >
                                    Đăng ký
                                </VBtn>
                            </VForm>
                            <div class="mt-6 text-center">
                                <p class="text-emerald-700">Đã có tài khoản?
                                <a @click="customerTab = 0" style="text-decoration: underline; cursor: pointer;" class="text-emerald-600 font-medium hover:text-emerald-800 hover:underline ml-1 transition-all">
                                    Đăng nhập
                                </a>
                                </p>
                            </div>
                        </VTabsWindowItem>
                    </VTabsWindow>
                </div>
            </VCard>

            <!-- Staff Login Form -->
            <VCard class="group relative w-full overflow-hidden rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 bg-emerald-200 transform hover:-translate-y-1" :disabled="employeeLoading">
                <template #loader>
                    <VProgressLinear indeterminate color="primary" v-if="employeeLoading" />
                </template>
                <div class="p-8">
                <h3 class="text-2xl font-bold text-emerald-800 mb-6 flex items-center">
                    <VIcon icon="mdi-badge-account-horizontal" class="mr-3" />
                    Dành cho nhân viên
                </h3>
                <VForm class="space-y-4" ref="formLoginCustomer" @keypress.enter="handleEmployeeLogin">
                    <div>
                        <label class="block text-emerald-700 font-medium" htmlFor="staff-id">Mã nhân viên</label>
                        <VTextField 
                            hide-details
                            variant="outlined"
                            density="comfortable"
                            class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                            placeholder="Nhập mã nhân viên..."
                            v-model:model-value="employeeLoginData.Username"
                        />
                    </div>
                    <div>
                        <label class="block text-emerald-700 mb-2 font-medium" htmlFor="staff-password">Mật khẩu</label>
                        <VTextField 
                            hide-details
                            variant="outlined"
                            density="comfortable" 
                            type="password" 
                            class="w-full mt-2 rounded-lg border-emerald-200 focus:border-emerald-500 focus:ring-2 focus:ring-emerald-200 outline-none transition-all" 
                            placeholder="••••••••"
                            v-model:model-value="employeeLoginData.Password"
                        />
                    </div>
                    <div class="flex items-center justify-between" v-if="false">
                        <div class="flex items-center">
                            <input 
                            id="staff-remember" 
                            type="checkbox" 
                            class="w-4 h-4 text-emerald-600 border-emerald-300 rounded focus:ring-emerald-500"
                            />
                            <label htmlFor="staff-remember" class="ml-2 text-emerald-700">Remember me</label>
                        </div>
                        <a href="https://webcrumbs.cloud/placeholder" class="text-emerald-600 hover:text-emerald-800 hover:underline transition-all">Need help?</a>
                    </div>
                    <VBtn
                        style="height: 48px !important;"
                        @click="handleEmployeeLogin"
                        class="w-full bg-emerald-700 hover:bg-emerald-600 text-white px-4 py-3 rounded-lg font-semibold transition-all duration-300 transform hover:-translate-y-1 active:translate-y-0 shadow-md hover:shadow-lg"
                    >
                        Đăng nhập
                    </VBtn>
                </VForm>
                <div class="mt-6 text-center">
                    <p class="text-emerald-700">Liên hệ quản lý nhà hàng để truy cập
                    <a v-if="false" href="https://webcrumbs.cloud/placeholder" style="text-decoration: underline;" class="text-emerald-600 font-medium hover:text-emerald-800 hover:underline ml-1 transition-all">
                        đặt lại mật khẩu
                    </a>
                    </p>
                </div>
                </div>
            </VCard>
        </div>
        
        <div class="mt-12 text-center">
        <RouterLink :to="{name: '//home'}" style="cursor: pointer;" class="inline-flex items-center text-emerald-700 hover:text-emerald-600 transition-all">
            <VIcon icon="mdi-arrow-left" />
            <span class="ml-2" style="text-decoration: underline">Quay lại trang chủ</span>
        </RouterLink>
        </div>
    </div>
    </VContainer>
    </div>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { session } from '@/common/Session';
import { Customer, MLActionResult, UserLogin } from '@/models';

export default {
    beforeRouteEnter(to, from, next) {
        if (session.UserType !== undefined && session.UserData) {
            next('/home');
            return;
        }

        next();
    },

    data() {
        return {
            customerLoading: <boolean>false,
            employeeLoading: <boolean>false,

            customerTab: <number>0,

            customerLoginData: <UserLogin>{} as UserLogin,
            customerSignupData: <Customer>{
                UserLogin: {} as UserLogin
            } as Customer,

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
                result = await this.$service.UserLoginService.loginCustomer(this.customerLoginData);
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.customerLoading = false;
            }

            if (!result) return;

            if (result.Success && result.Data) {
                localStorage.setItem(this.$localStorageKey.UserID, result.Data.UserID);
                EventBus.emit('ShowToastMessage', {
                    Message: 'Đăng nhập thành công',
                    Type: 'success'
                });
                window.location.pathname = '/';
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

            let result:MLActionResult|undefined = undefined;
            try {
                if (this.customerSignupData.UserLogin) {
                    this.customerSignupData.UserLogin.Username = this.customerSignupData.PhoneNumber;
                }

                result = await this.$service.UserLoginService.registerNewCustomer(this.customerSignupData);
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
            if (result.Success && result.Data) {
                localStorage.setItem(this.$localStorageKey.UserID, result.Data.UserID);

                window.location.reload();
            }
        }
    },
}
</script>

<style lang="scss" scoped>
.text-disabled {
  opacity: 0.5;
  pointer-events: none;
}
</style>