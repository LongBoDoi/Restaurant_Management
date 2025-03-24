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

    if (result?.Data) {
      Object.assign(this.$session, result.Data);
    }

    if (result?.Success) {
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
</style>