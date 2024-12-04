<template>
    <VSnackbar
      v-model="visible"
      :timeout="3000"
      :color="color"
      location="bottom"
      elevation="2"
    >
      {{ message }}
      <template #actions>
        <VBtn icon @click="visible = false">
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
            this.message = data.Message;
            this.color = data.Type;
            this.visible = true;
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