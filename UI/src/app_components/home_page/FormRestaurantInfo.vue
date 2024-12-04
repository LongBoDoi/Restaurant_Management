<template>
    <VContainer style="padding: unset; display: flex; align-items: center;">
        <div style="width: 50%; height: 100%; background-image: url(https://images.unsplash.com/photo-1712630514718-3830cc6c0d0a?w=400&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8cmVzdGF1cmFudCUyMGJhY2tncm91bmR8ZW58MHx8MHx8fDA%3D); background-size: 100% 100%;" />
        <MLVbox style="width: 50%; height: 100%; padding-left: 24px; align-items: center; justify-content: center;">
            <MLVbox style="align-items: center; max-width: 80%;">
                <!-- Tên nhà hàng -->
                <VLabel style="font-size: 3rem; font-weight: bold; color: rgb(var(--v-theme-primary))">Nhà Hàng 123</VLabel>

                <!-- Địa chỉ nhà hàng -->
                <MLHbox style="width: 100%;">
                    <VIcon color="primary" size="36" icon="mdi-map-marker" />
                    <VLabel style="font-size: 1.5rem; word-break: break-word; white-space: normal; margin-left: 8px;">Số 12 đường Giải Phóng, Xuân Đỉnh, Bắc Từ Liêm, Hà Nội, Việt Nam</VLabel>
                </MLHbox>

                <!-- SĐT nhà hàng -->
                <MLHbox style="width: 100%;">
                    <VIcon color="primary" size="36" icon="mdi-phone" />
                    <VLabel style="font-size: 1.5rem; word-break: break-word; white-space: normal; margin-left: 8px;">0123456789</VLabel>
                </MLHbox>

                <!-- Mô tả nhà hàng -->
                <VLabel style="width: 100%; margin-top: 12px;">Chúng tôi phục vụ những món ăn ngon nhất với giá cả hợp lý</VLabel>

                <VSpacer style="height: 48px;" />
                
                <MLHbox style="width: 100%;">
                    <VBtn color="primary" @click="onViewRestaurantMenu">Xem thực đơn</VBtn>
                    <VBtn style="margin-left: 16px;" @click="onBookReservation">Đặt bàn</VBtn>
                </MLHbox>
            </MLVbox>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
export default {
    props: {
        onViewRestaurantMenu: {
            type: Function
        },

        onBookReservation: {
            type: Function
        },
    },
}
</script>