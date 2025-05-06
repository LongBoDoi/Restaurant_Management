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

export default {
  async created() {
    

    // if (result?.Success) {
    //   if (result.Data.UserType === EnumUserType.Employee && !window.location.pathname.startsWith('/management')) {
    //     this.$router.replace({name: '/management'});
    //   }
    // } else {
    //   this.redirectToLogin();
    // }

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
      if (window.location.pathname.startsWith('/management')) {
        window.location.pathname = '/login';
      }
    }
  }
}
</script>

<style lang="scss" scoped>
</style>