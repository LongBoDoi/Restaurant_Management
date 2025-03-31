<template>
    <VSnackbar
      v-model="visible"
      :timeout="3000"
      :color="color"
      location="bottom"
      elevation="2"
      class="rounded-lg"
    >
      {{ message }}
      <template #actions>
        <VBtn style="width: 40px; height: 40px;" icon @click="visible = false">
          <VIcon icon="mdi-close" />
        </VBtn>
      </template>
    </VSnackbar>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';

export default {
    created() {
        EventBus.on('ShowToastMessage', this.handleShowToastMessage)
    },

    beforeUnmount() {
        EventBus.off('ShowToastMessage');
    },

    methods: {
        handleShowToastMessage(data:any) {
          this.visible = false;

          this.$nextTick().then(() => {
            this.message = data.Message;
            this.color = data.Type;
            this.visible = true;
          });
        }
    },

    data() {
        return {
            visible: <boolean>false,
            color: <string>'',
            message: <string>''
        }
    }
}
</script>

<style lang="scss" scoped>
:deep() {
  .v-snackbar__wrapper {
    border-radius: 24px;
  }
}
</style>