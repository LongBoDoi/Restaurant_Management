<template>
    <VApp>
    <VAppBar class="bg-emerald-800">
        <VContainer style="display: flex; align-items: center;">
            <img style="width: 48px; height: 48px; border-radius: 8px; cursor: pointer; border-radius: 50%;" src="/src/resources/app-logo.jpeg" @click="handleLogoClick" />
            
            <h1 class="text-white font-semibold tracking-wide ml-3 hidden md:block text-2xl">{{ $commonFunction.getSettingValue('RestaurantName') }}</h1>

            <div class="hidden md:flex md:flex-1 md:justify-center md:space-x-8 text-white">
                <RouterLink
                    :to="{name: '//home'}"
                    active-class="after:w-full"
                    class="font-medium relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-emerald-400 hover:after:w-full after:transition-all after:duration-300"
                >
                    Trang chủ
                </RouterLink>
                <RouterLink
                    v-if="$commonFunction.getSettingValue('DisplayMenuScreenForCustomer')"
                    :to="{name: '//menu'}"
                    active-class="after:w-full"
                    class="font-medium relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-emerald-400 hover:after:w-full after:transition-all after:duration-300"
                >
                    Thực đơn
                </RouterLink>
                <RouterLink
                    v-if="$commonFunction.getSettingValue('DisplayBookingScreenForCustomer')"
                    :to="{name: '//reservation'}"
                    active-class="after:w-full"
                    class="font-medium relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-emerald-400 hover:after:w-full after:transition-all after:duration-300"
                >
                    Đặt bàn
                </RouterLink>
            </div>

            <VBtn rounded class="bg-emerald-700 text-white ml-auto" v-if="!isCustomerLoggedIn" :to="{name: '//login'}">Đăng nhập</VBtn>
            <v-menu v-else :persistent="accountMenuLoading" :close-on-content-click="!accountMenuLoading" offset-y>
                <template v-slot:activator="{ props }">
                <VBtn rounded class="bg-emerald-700 text-white" v-bind="props" prepend-icon="mdi-account-circle">
                    {{ userName }}
                </VBtn>
                </template>
                <VCard :disabled="accountMenuLoading" class="rounded-lg" max-width="300" min-width="200" >
                    <template v-slot:loader>
                        <VProgressLinear v-if="accountMenuLoading" color="primary" indeterminate />
                    </template>
                    <div class="p-4 border-b border-gray-100">
                        <div class="flex align-center space-x-3">
                            <div class="h-12 w-12 rounded-full bg-green-100 flex align-center justify-center text-green-600 overflow-hidden border border-green-500">
                                <img v-if="customerData.ImageUrl" :src="$commonFunction.getImageUrl(customerData.ImageUrl)" class="rounded-full border border-green-500">
                                <VIcon v-else icon="mdi-account-outline" />
                            </div>
                            <div style="flex-grow: 1;">
                                <div class="font-bold text-gray-800 text-center">{{ customerData.CustomerName }}</div>
                                <div class="text-sm text-gray-500 text-center" style="word-break: break-all;">{{ $commonFunction.formatPhoneNumber(customerData.PhoneNumber) ?? customerData.Email ?? '' }}</div>
                            </div>
                        </div>
                    </div>

                    
                    <v-list style="color: rgb(var(--v-theme-primary));">
                        <VListItem link @click="handleUpdatePersonalInfoClick" color="primary" class="px-5">
                            <VIcon class="mr-3 text-gray-500" icon="mdi-account-circle-outline" />
                            <span class="text-gray-500">Hồ sơ cá nhân</span>
                        </VListItem>
                        <VListItem link @click="handleChangePasswordClick" color="primary" class="px-5">
                            <VIcon class="mr-3 text-gray-500" icon="mdi-key-variant" />
                            <span class="text-gray-500">Đổi mật khẩu</span>
                        </VListItem>
                    </v-list>

                    <div class="border-t border-gray-100 pa-3">
                        <VBtn prepend-icon="mdi-logout" rounded variant="text" class="bg-red-50 text-red-600 w-full" @click="logOut">
                            Đăng xuất
                        </VBtn>
                    </div>
                </VCard>
            </v-menu>
        </VContainer>
    </VAppBar>

    <VMain style="padding: unset; height: calc(100% - 64px); top: 64px;" scrollable>
        <RouterView />
    </VMain>

    <ChatbotWindow />
    <FormChangePassword />
    <FormUpdatePersonalInfo />
    </VApp>
</template>

<script lang="ts">
import { EnumUserType } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import LocalStorageKey from '@/common/LocalStorageKey';
import { session } from '@/common/Session';
import { Customer, Employee } from '@/models';
import MLActionResult from '@/models/MLActionResult';
import { configTokenForService } from '@/services';
import { userLoginService } from '@/services/userLoginService';

export default {
    data() {
        return {
            accountMenuLoading: false
        }
    },

    async beforeRouteEnter(to, from, next) {
        //Lấy dữ liệu người dùng
        const userID = localStorage.getItem(LocalStorageKey.UserID) ?? '';

        let result:MLActionResult|undefined = undefined;
        try {
            result = await userLoginService.getUserData(userID);
        } catch (e) {
        } finally {
        }

        if (result?.Data) {
            Object.assign(session, result.Data);

            session.Permissions = new Set((session.UserData as Employee)?.Roles?.map(r => r.Permissions.map(p => p.PermissionCode)).flat());

            configTokenForService(result.Data.Token);
        }

        if (session.UserType === EnumUserType.Employee) {
            next({name: '/management'});
            return;
        }

        // if (to.name === '/' && !from.name?.startsWith('/management')) {
        //     console.log('from', from);
        //     next('/home');
        // } else {
        //     next();
        // }
        
        next();
    },

    created() {
        if (window.location.pathname === '/') {
            this.$router.replace({name: '//home'});
        }
    },

    methods: {
        handleLogoClick() {
            window.location.pathname = '/';
        },

        async logOut() {
            this.accountMenuLoading = true;

            let result:MLActionResult|undefined = undefined;
            try {
                result = await this.$service.UserLoginService.logOut();
            } catch (e) {
                this.$commonFunction.handleException(e);
            } finally {
                this.accountMenuLoading = false;
            }

            if (result?.Success) {
                localStorage.removeItem(this.$localStorageKey.UserID);
                window.location.reload();
            }
        },

        handleChangePasswordClick() {
            EventBus.emit(this.$eventName.ShowFormChangePassword);
        },

        handleUpdatePersonalInfoClick() {
            EventBus.emit(this.$eventName.ShowFormUpdatePersonalInfo);
        },
    },

    computed: {
        isCustomerLoggedIn() {
            return this.$session.UserType === EnumUserType.Customer && this.$session.UserData !== undefined;
        },

        userName() {
            return this.$session.UserName;
        },

        customerData() {
            return this.$session.UserData as Customer;
        }
    }
}
</script>

<style lang="scss" scoped>
.main-background {
    flex-grow: 1;
    overflow-y: auto;
    background-size: 100% 100%;
}

.hidden {
    display: none;
}

@media (min-width: 768px) {
    .md\:block {
        display: block;
    }
}

.main-app-sheet {
    height: calc(100vh - 64px);

    & > * {
        height: 100%;
    }
}

.after\:absolute:after {
  content: var(--tw-content);
  position: absolute;
}
.after\:bottom-0:after {
  bottom: 0;
  content: var(--tw-content);
}
.after\:left-0:after {
  content: var(--tw-content);
  left: 0;
}
.after\:h-0\.5:after {
  content: var(--tw-content);
  height: 2px;
}
.after\:w-0:after {
  content: var(--tw-content);
  width: 0;
}
.after\:bg-emerald-400:after {
  content: var(--tw-content);
  --tw-bg-opacity: 1;
  background-color: rgb(52 211 153 / var(--tw-bg-opacity));
}
.after\:transition-all:after {
  content: var(--tw-content);
  transition-duration: 0.15s;
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}
.after\:duration-300:after {
  content: var(--tw-content);
  transition-duration: 0.3s;
}
.hover\:after\:w-full:hover:after,
.after\:w-full:after {
  content: var(--tw-content);
  width: 100%;
}
</style>