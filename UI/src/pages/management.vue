<!-- Trang chính Quản lý nhà hàng -->

<template>
    <VApp>
        <VAppBar app color="primary" />

        <VNavigationDrawer app permanent persistent color="secondary">
            <VList active-class="active-navigation">
                <VListItem link :to="{name: '/management/order'}" prepend-icon="mdi-silverware-fork-knife" title="Order" />
                <VListItem link :to="{name: '/management/reservation'}" prepend-icon="mdi-notebook-edit" title="Đặt bàn" />
                <VListItem link :to="{name: '/management/menu'}" prepend-icon="mdi-food" title="Thực đơn" />
                <VListItem link :to="{name: '/management/customer'}" prepend-icon="mdi-account-multiple" title="Khách hàng" />
            </VList>
        </VNavigationDrawer>

        <VMain>
            <RouterView />
        </VMain>
    </VApp>
</template>

<script lang="ts">
export default {
    created() {
        const currentRouteName = window.location.pathname;
        if (currentRouteName === '/management' || currentRouteName === '/management/') {
            this.$router.replace({name: '/management/order'});
            return;
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