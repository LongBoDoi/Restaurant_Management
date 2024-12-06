<template>
    <MLVbox style="width: 100%; height: 100%; max-height: 100vh;">
        <!-- Toolbar -->
        <VToolbar color="primary">
            <VContainer style="display: flex;">
                <VSpacer />

                <VBtn rounded variant="tonal" style="margin-right: 24px;" @click="handleLoginClick" v-if="!isCustomerLoggedIn">Đăng nhập</VBtn>
                <v-menu v-else :persistent="accountMenuLoading" :close-on-content-click="!accountMenuLoading" offset-y>
                    <template v-slot:activator="{ props }">
                    <VBtn rounded class="mr-6" v-bind="props" prepend-icon="mdi-account-circle">
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
import MLActionResult from '@/models/MLActionResult';

export default {
    data() {
        return {
            accountMenuLoading: false,
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
                localStorage.removeItem(this.$localStorageKey.AuthToken);
                window.location.reload();
            }
        }
    },

    computed: {
        isCustomerLoggedIn() {
            return this.$session.UserType === EnumUserType.Customer && this.$session.UserData !== undefined;
        },

        userName() {
            return this.$session.UserName;
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