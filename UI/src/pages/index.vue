<template>
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
                    :to="{name: '//menu'}"
                    active-class="after:w-full"
                    class="font-medium relative after:absolute after:bottom-0 after:left-0 after:h-0.5 after:w-0 after:bg-emerald-400 hover:after:w-full after:transition-all after:duration-300"
                >
                    Thực đơn
                </RouterLink>
                <RouterLink
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
                <VCard :disabled="accountMenuLoading">
                    <template v-slot:loader>
                        <VProgressLinear v-if="accountMenuLoading" color="primary" indeterminate />
                    </template>
                    <v-list>
                        <VListItem class="text-primary" link @click="logOut">
                            <VIcon class="mr-2" icon="mdi-logout" />
                            <span>Đăng xuất</span>
                        </VListItem>
                    </v-list>
                </VCard>
            </v-menu>
        </VContainer>
    </VAppBar>

    <VMain style="padding: unset; height: calc(100% - 64px); top: 64px;" scrollable>
        <RouterView />
    </VMain>
</template>

<script lang="ts">
import { EnumUserType } from '@/common/Enumeration';
import MLActionResult from '@/models/MLActionResult';

export default {
    data() {
        return {
            accountMenuLoading: false
        }
    },

    created() {
        const currentRouteName = window.location.pathname;
        if (currentRouteName === '/' || currentRouteName === '//') {
            this.$router.replace({name: '//home'});
            return;
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