<template>
    <MLVbox style="width: 100%; height: 100%; max-height: 100vh;">
        <!-- Toolbar -->
        <VToolbar color="primary">
            <VContainer style="display: flex;">
                <VBtn rounded variant="tonal" style="margin-right: 24px; margin-left: auto;" @click="handleLoginClick" v-if="!isCustomerLoggedIn">Đăng nhập</VBtn>
            </VContainer>
        </VToolbar>

        <MLVbox class="main-background" style="background-color: rgba(var(--v-theme-primary), 0.2);">
            <!-- Form thông tin nhà hàng -->
            <FormRestaurantInfo v-on:viewRestaurantMenu="handleViewMenuClick" v-on:bookReservation="handleBookReservationClick" style="flex-shrink: 0; width: 100%; height: 100%;" />

            <!-- chatbot -->
            <ChatbotWindow />

            <!-- Form menu nhà hàng -->
            <div ref="frmRestaurantMenu" style="flex-shrink: 0; width: 100%; height: 100%;">
                <FormRestaurantMenu style="width: 100%; height: 100%;" />
            </div>  

            <div ref="frmBookReservation" style="flex-shrink: 0; width: 100%; height: 100%;">
                <FormBookReservation style="width: 100%; height: 100%;" />
            </div>

            <div v-if="!isCustomerLoggedIn" ref="frmLogin" style="flex-shrink: 0; width: 100%; height: 100%;">
                <FormLogin style="height: 100%;" />
            </div>
        </MLVbox>
    </MLVbox>
</template>

<script lang="ts">
import { EnumUserType } from '@/common/Enumeration';

export default {
    data() {
        return {
            showLoading: false,
        }
    },

    methods: {
        handleViewMenuClick() {
            (this.$refs.frmRestaurantMenu as any).scrollIntoView({
                behavior: 'smooth'
            });
        },

        handleBookReservationClick() {
            (this.$refs.frmBookReservation as any).scrollIntoView({
                behavior: 'smooth'
            });
        },

        handleLoginClick() {
            (this.$refs.frmLogin as any).scrollIntoView({
                behavior: 'smooth'
            });
        },
    },

    computed: {
        isCustomerLoggedIn() {
            return this.$session.UserType === EnumUserType.Customer && this.$session.UserData !== undefined;
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
</style>