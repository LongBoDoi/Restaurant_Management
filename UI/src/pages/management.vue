<!-- Trang chính Quản lý nhà hàng -->
<template>
    <VApp>
        <VAppBar app class="app-tool-bar bg-gradient-to-r from-teal-600 to-green-500 px-6 flex justify-between items-center" style="height: 64px;" >
            <img style="width: 40px; height: 40px; border-radius: 50%; cursor: pointer;" @click="handleLogoClick" src="/src/resources/app-logo.jpeg" />
            <h1 className="text-white text-2xl font-bold ml-3">{{ $commonFunction.getSettingValue('RestaurantName') }}</h1>
            <VSpacer />
            <v-menu :persistent="accountMenuLoading" :close-on-content-click="!accountMenuLoading" offset-y>
                <template v-slot:activator="{ props }">
                <VBtn rounded class="mr-8" v-bind="props" prepend-icon="mdi-account-outline" style="background-color: white; color: rgb(var(--v-theme-primary));">
                    {{ userName }}
                </VBtn>
                </template>
                <VCard :disabled="accountMenuLoading" >
                    <template v-slot:loader>
                        <VProgressLinear v-if="accountMenuLoading" color="primary" indeterminate />
                    </template>
                    <v-list style="color: rgb(var(--v-theme-primary));">
                        <VListItem link @click="logOut">
                            <VIcon class="mr-2" icon="mdi-logout" />
                            <span>Đăng xuất</span>
                        </VListItem>
                    </v-list>
                </VCard>
            </v-menu>
        </VAppBar>

        <VNavigationDrawer app permanent persistent class="bg-gradient-to-b from-gray-100 to-gray-50">
            <VList class="pa-4" style="color: rgb(55, 65, 81) !important;">
                <RouterLink
                    to="/management/order"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                    active-class="active-navigation"
                >
                    <span className="material-symbols-outlined mr-3">restaurant_menu</span>
                    <span>Order</span>
                </RouterLink>
                <RouterLink
                    to="/management/reservation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                    active-class="active-navigation"
                >
                    <span className="material-symbols-outlined mr-3">table_restaurant</span>
                    <span>Đặt bàn</span>
                </RouterLink>
                <RouterLink
                    to="/management/menu"
                    active-class="active-navigation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                >
                    <span className="material-symbols-outlined mr-3">menu_book</span>
                    <span>Thực đơn</span>
                </RouterLink>
                <RouterLink
                    to="/management/inventory"
                    active-class="active-navigation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                >
                    <span className="material-symbols-outlined mr-3">inventory</span>
                    <span>Nguyên liệu</span>
                </RouterLink>
                <RouterLink
                    to="/management/customer"
                    active-class="active-navigation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                >
                    <span className="material-symbols-outlined mr-3">groups</span>
                    <span>Khách hàng</span>
                </RouterLink>
                <RouterLink
                    to="/management/employee"
                    active-class="active-navigation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                >
                    <span className="material-symbols-outlined mr-3">badge</span>
                    <span>Nhân viên</span>
                </RouterLink>
                <RouterLink
                    to="/management/setting"
                    active-class="active-navigation"
                    class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                >
                    <VIcon icon="mdi-cog-outline" class="mr-3" style="opacity: 1;" />
                    <span>Thiết lập</span>
                </RouterLink>
            </VList>
        </VNavigationDrawer>

        <VMain>
            <VSheet class="main-app-sheet">
                <RouterView />
            </VSheet>
        </VMain>
    </VApp>
</template>

<script lang="ts">
import LocalStorageKey from '@/common/LocalStorageKey';
import { MLActionResult } from '@/models';

export default {
    created() {
        const currentRouteName = window.location.pathname;
        if (currentRouteName === '/management' || currentRouteName === '/management/') {
            this.$router.replace({name: '/management/order'});
            return;
        }
    },

    data() {
        return {
            accountMenuLoading: <boolean>false
        }
    },

    methods: {
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
                localStorage.removeItem(LocalStorageKey.AuthToken);
                window.location.reload();
            }
        },

        handleLogoClick() {
            window.location.pathname = '/';
        },

        isRouteActive(path: string) {
            return window.location.pathname.startsWith(path);
        }
    },

    computed: {
        userName() {
            return this.$session.UserName;
        }
    }
}
</script>

<style lang="scss" scoped>
.active-navigation {
    background-color: rgb(var(--v-theme-secondary)) !important;
    color: white !important;
    opacity: 1;
}

.main-app-sheet {
    height: calc(100vh - 64px);
    padding: 24px 40px 24px 24px;

    & > * {
        height: 100%;
    }
}

.from-gray-100 {
  --tw-gradient-from: #f3f4f6 var(--tw-gradient-from-position);
  --tw-gradient-to: rgba(243, 244, 246, 0) var(--tw-gradient-to-position);
  --tw-gradient-stops: var(--tw-gradient-from), var(--tw-gradient-to);
}
.to-gray-50 {
  --tw-gradient-to: #f9fafb var(--tw-gradient-to-position);
}
.hover\:bg-gray-200:hover {
  --tw-bg-opacity: 1;
  background-color: rgb(229 231 235 / var(--tw-bg-opacity));
}
</style>