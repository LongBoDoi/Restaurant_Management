<template>
  <div style="position: fixed; width: 100vw; height: 100vh;">
    <MLLoadingScreen v-if="isLoading" />
    <router-view v-else></router-view>
  </div>
</template>

<script lang="ts">
import { mapActions } from 'pinia';
import { MLActionResult } from './models';
import { userStore } from './stores/userStore';
import EventBus from './common/EventBus';

export default {
  async created() {
    // const token = localStorage.getItem('authToken');
    // if (!token) {
    //   this.redirectToLogin();
    //   return;
    // }

    // //Lấy dữ liệu người dùng
    // let result:MLActionResult|undefined = undefined;
    // try {
    //   this.isLoading = true;

    //   result = await this.$service.UserLoginService.getUserData();
    // } catch (e) {
    //   localStorage.removeItem('userID');
    //   this.redirectToLogin();
    // } finally {
    //   this.isLoading = false;
    // }

    // if (result?.Success) {
    //   // this.$session.UserID = userID;
    // } else {
    //   localStorage.removeItem('userID');
    //   this.redirectToLogin();
    // }

    // EventBus.on('RedirectToLogin', this.redirectToLogin);
  },

  beforeUnmount() {
    EventBus.off('RedirectToLogin');
  },

  data() {
    return {
      isLoading: false
    }
  },

  methods: {
    ...mapActions(userStore, ['setUserID']),

    redirectToLogin() {
      this.$router.clearRoutes();
      this.$router.push({name: '/login'});
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