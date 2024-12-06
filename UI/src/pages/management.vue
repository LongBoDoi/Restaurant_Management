<!-- Trang chính Quản lý nhà hàng -->

<template>
    <VApp>
        <VAppBar app color="primary" >
            <VSpacer />
            <v-menu :persistent="accountMenuLoading" :close-on-content-click="!accountMenuLoading" offset-y>
                <template v-slot:activator="{ props }">
                <VBtn rounded class="mr-8" v-bind="props" prepend-icon="mdi-account-circle">
                    {{ userName }}
                </VBtn>
                </template>
                <VCard :disabled="accountMenuLoading">
                    <template v-slot:loader>
                        <VProgressLinear v-if="accountMenuLoading" color="primary" indeterminate />
                    </template>
                    <v-list>
                        <VListItem link @click="logOut">
                            <VIcon class="mr-2" icon="mdi-logout" />
                            <span>Đăng xuất</span>
                        </VListItem>
                    </v-list>
                </VCard>
            </v-menu>
        </VAppBar>

        <VNavigationDrawer app permanent persistent color="secondary">
            <VList active-class="active-navigation">
                <VListItem link :to="{name: '/management/order'}" prepend-icon="mdi-silverware-fork-knife" title="Order" />
                <VListItem link :to="{name: '/management/reservation'}" prepend-icon="mdi-notebook-edit" title="Đặt bàn" />
                <VListItem link :to="{name: '/management/menu'}" prepend-icon="mdi-food" title="Thực đơn" />
                <VListItem link :to="{name: '/management/inventory'}" prepend-icon="mdi-food-variant" title="Nguyên liệu" />
                <VListItem link :to="{name: '/management/customer'}" prepend-icon="mdi-account-group" title="Khách hàng" />
                <VListItem link :to="{name: '/management/employee'}" prepend-icon="mdi-account-file-text" title="Nhân viên" />
            </VList>
        </VNavigationDrawer>

        <VMain>
            <RouterView />
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
    background-color: #FFB300;

    :deep() {
        .v-list-item-title {
            font-weight: bold;
            color: rgb(var(--v-theme-on-primary));
            opacity: 1;
        }

        .v-list-item__prepend {
            color: rgb(var(--v-theme-on-primary));
        }

        .v-list-item__overlay {
            opacity: 0.25;
        }
    }
}
</style>