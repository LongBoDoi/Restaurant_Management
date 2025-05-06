<!-- Trang chính Quản lý nhà hàng -->
<template>
    <VApp>
        <VAppBar app class="app-tool-bar bg-gradient-to-r from-teal-600 to-green-500 px-6 flex justify-between items-center" style="height: 64px;" >
            <img style="width: 40px; height: 40px; border-radius: 50%; cursor: pointer;" @click="handleLogoClick" src="/src/resources/app-logo.jpeg" />
            <h1 class="text-white text-2xl font-bold ml-3">{{ $commonFunction.getSettingValue('RestaurantName') }}</h1>
            <VSpacer />
            <v-menu :persistent="accountMenuLoading" :close-on-content-click="false" offset-y>
                <template v-slot:activator="{ props }">
                <VBtn rounded class="mr-8" v-bind="props" prepend-icon="mdi-account-outline" style="background-color: white; color: rgb(var(--v-theme-primary));">
                    {{ employeeData.EmployeeCode }}
                </VBtn>
                </template>
                <VCard :disabled="accountMenuLoading" class="rounded-lg" max-width="300" min-width="200" >
                    <template v-slot:loader>
                        <VProgressLinear v-if="accountMenuLoading" color="primary" indeterminate />
                    </template>
                    <div class="p-4 border-b border-gray-100">
                        <div class="flex align-center space-x-3">
                            <img v-if="employeeData.ImageUrl" :src="$commonFunction.getImageUrl(employeeData.ImageUrl)" class="h-12 w-12 rounded-full border border-green-500">
                            <div v-else class="h-12 w-12 rounded-full bg-green-100 flex align-center justify-center text-green-600">
                                <VIcon icon="mdi-account-outline" />
                            </div>
                            <div style="flex-grow: 1;">
                                <div class="font-bold text-gray-800 text-center">{{ employeeData.EmployeeName }}</div>
                                <div class="text-sm text-gray-500 text-center" style="word-break: break-all;">{{ $commonFunction.formatPhoneNumber(employeeData.PhoneNumber) ?? employeeData.Email ?? '' }}</div>
                            </div>
                        </div>
                    </div>

                    
                    <v-list style="color: rgb(var(--v-theme-primary));">
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
        </VAppBar>

        <VNavigationDrawer app permanent persistent class="bg-gradient-to-b from-gray-100 to-gray-50">
            <VList class="pa-4" style="color: rgb(55, 65, 81) !important;">
                <div v-for="route in routes" :key="route.name">
                    <RouterLink v-if="!route.isGroup" :to="(route.name as string)" class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200" active-class="active-navigation">
                        <span class="material-symbols-outlined mr-3">{{ route.icon }}</span>
                        <span>{{ route.title }}</span>
                    </RouterLink>

                    <VListGroup v-if="route.isGroup">
                        <template v-slot:activator="{ props }">
                            <v-list-item v-bind="props"
                                class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 mb-1 transition-all duration-200"
                            >
                                <MLHbox class="align-center">
                                    <span class="material-symbols-outlined mr-3">{{ route.icon }}</span>
                                    <span>{{ route.title }}</span>
                                </MLHbox>
                            </v-list-item>
                        </template>

                        <template v-for="child in route.children" :key="child.name">
                            <RouterLink
                                :to="(child.name as string)"
                                active-class="active-navigation"
                                class="flex items-center text-gray-700 hover:bg-gray-200 rounded-lg px-4 py-3 pl-8 mb-1 transition-all duration-200"
                            >
                                <MLHbox class="align-center">
                                    <span class="material-symbols-outlined mr-3 text-sm" style="font-size: 20px;">{{ child.icon }}</span>
                                    <span>{{ child.title }}</span>
                                </MLHbox>
                            </RouterLink>
                        </template>
                    </VListGroup>
                </div>
            </VList>
        </VNavigationDrawer>

        <VMain>
            <VSheet class="main-app-sheet">
                <RouterView />
            </VSheet>
        </VMain>

        <FormChangePassword />
    </VApp>
</template>

<script lang="ts">
import { EnumUserType } from '@/common/Enumeration';
import EventBus from '@/common/EventBus';
import LocalStorageKey from '@/common/LocalStorageKey';
import { session } from '@/common/Session';
import { Employee, MLActionResult } from '@/models';
import { routerMapPermissions } from '@/router';
import { configTokenForService } from '@/services';
import { userLoginService } from '@/services/userLoginService';

export default {
    async beforeRouteEnter(to, from, next) {
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
            const userPermissions = session.Permissions;
            if (userPermissions === undefined) {
                next();
                return;
            }

            const permission = routerMapPermissions[to.path];
            const isAllowed = !permission || userPermissions.has(routerMapPermissions[to.path]);
            if (!isAllowed) {
                next('/page-not-found')  // or redirect somewhere else
            } else {
                next()
            }
        } else {
            next('/home');
        }
    },

    created() {
        if (window.location.pathname === '/management' && this.routes.length > 0) {
            this.$router.replace(this.routes[0].name ?? '/home');
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
                localStorage.removeItem(LocalStorageKey.UserID);
                window.location.pathname = '/';
            }
        },

        handleLogoClick() {
            window.location.pathname = '/';
        },

        handleChangePasswordClick() {
            EventBus.emit(this.$eventName.ShowFormChangePassword);
        },

        isRouteActive(path: string) {
            return window.location.pathname.startsWith(path);
        },

        hasPermission(permissionCode: string):boolean {
            return this.permissions.has(permissionCode);
        }
    },

    computed: {
        employeeData() {
            return this.$session.UserData as Employee;
        },

        permissions() {
            return this.$session.Permissions as Set<string>;
        },

        routes() {
            const routes = [];
            if (this.hasPermission('ViewReport')) {
                routes.push({ name: '/management/dashboard', icon: 'bar_chart', title: 'Báo cáo' });
            }
            if (this.hasPermission('ManageOrder')) {
                routes.push({ name: '/management/order', icon: 'receipt_long', title: 'Order' });
            }
            if (this.hasPermission('ManageTable')) {
                routes.push({ isGroup: true, icon: 'table_restaurant', title: 'Bàn', children: [
                    { name: '/management/table', icon: 'table_restaurant', title: 'Bàn' },
                    { name: '/management/area', icon: 'map', title: 'Khu vực' },
                    { name: '/management/reservation', icon: 'event_available', title: 'Đặt bàn' }
                ] });
            }
            if (this.hasPermission('ManageMenu')) {
                routes.push({ isGroup: true, icon: 'menu_book', title: 'Thực đơn', children: [
                    { name: '/management/menu', icon: 'restaurant_menu', title: 'Món ăn' },
                    { name: '/management/menu-category', icon: 'category', title: 'Nhóm thực đơn' },
                    { name: '/management/custom-menu-request', icon: 'restaurant', title: 'Sushi tự chọn' }
                ] });
            }
            if (this.hasPermission('ManageInventory')) {
                routes.push({ isGroup: true, icon: 'inventory', title: 'Nguyên liệu', children: [
                    { name: '/management/inventory', icon: 'rice_bowl', title: 'Nguyên liệu' },
                    { name: '/management/inventory-category', icon: 'category', title: 'Nhóm nguyên liệu' }
                ] });
            }
            if (this.hasPermission('ManageCustomer')) {
                routes.push({ name: '/management/customer', icon: 'groups', title: 'Khách hàng' });
            }
            if (this.hasPermission('ManageEmployee')) {
                routes.push({ name: '/management/employee', icon: 'badge', title: 'Nhân viên' });
            }
            if (this.hasPermission('ManagePermission')) {
                routes.push({ name: '/management/permission', icon: 'security', title: 'Phân quyền' });
            }
            if (this.hasPermission('ManageSetting')) {
                routes.push({ name: '/management/setting', icon: 'settings', title: 'Thiết lập' });
            }
            return routes;
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