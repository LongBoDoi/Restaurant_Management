<template>
  <div style="position: fixed; width: 100vw; height: 100vh;">
    <MLLoadingScreen v-if="isLoading" />
    <router-view v-else></router-view>
  </div>

  <MLToastMessage />

  <MLDialog />
</template>

<script lang="ts">
import { mapActions } from 'pinia';
import { userStore } from './stores/userStore';
import EventBus from './common/EventBus';
import { MLActionResult } from './models';
import { EnumUserType } from './common/Enumeration';

export default {
  async created() {
    //Lấy dữ liệu người dùng
    let result:MLActionResult|undefined = undefined;
    try {
      this.isLoading = true;

      result = await this.$service.UserLoginService.getUserData();
    } catch (e) {
      this.redirectToLogin();
    } finally {
      this.isLoading = false;
    }

    if (result?.Success) {
      Object.assign(this.$session, result.Data);

      if (result.Data.UserType === EnumUserType.Employee && window.location.pathname === '/') {
        this.$router.replace({name: '/management'});
      }
    } else {
      this.redirectToLogin();
    }

    EventBus.on(this.$eventName.RedirectToLogin, this.redirectToLogin);
  },

  beforeUnmount() {
    EventBus.off(this.$eventName.RedirectToLogin);
  },

  data() {
    return {
      isLoading: false
    }
  },

  methods: {
    ...mapActions(userStore, ['setUserID']),

    redirectToLogin() {
      if (window.location.pathname !== '/') {
        window.location.pathname = '/';
      }
    }
  }
}
</script>

<style lang="scss" scoped>
:deep() {
  .v-application {
    --primary-color-dark: #8c949a;

    --primary-color-darker: #717a82;

    --border-color: #C3C3C3;

    --normal-font-size: 16px;

    --big-font-size: 25px;

    --bigger-font-size: 40px;
  }

  .v-application.v-theme--dark {

    --primary-color-dark: #5a6166;

    --primary-color-darker: #727a81;

    color: rgba(255, 255, 255, 0.7);

    .v-card-title, .v-card-item {
      color: rgba(255, 255, 255, 0.7);
    }
  }

  .v-skeleton-loader {
    background-color: transparent;

    .v-skeleton-loader__ossein {
      background-color: rgb(var(--v-theme-primary));
    }
  }

  .v-label {
    cursor: inherit;
    opacity: 1;
  }

  .v-radio-group {
    .v-input__details {
      display: none;
    }
  }
}
</style>