<template>
    <VDialog persistent v-model:model-value="isShow" max-width="600">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <!-- <VCardTitle class="d-flex align-center">
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="isShow = false" />
            </VCardTitle> -->
            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">
                    {{ dialogData.Title }}
                </h2>
                <VBtn class="hover:scale-110" variant="text" style="width: 40px; height: 40px; color: white; opacity: 1;" icon="mdi-close" @click="isShow = false" />
            </VCardTitle>
            <VCardText class="pa-6 text-gray-700" style="min-height: 100px;">
                <div v-html="content" />
            </VCardText>
            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VBtn class="border-gray-300 text-gray-700 hover:scale-110" variant="outlined" rounded @click="isShow = false">Huỷ</VBtn>
                <VBtn variant="tonal" class="bg-green-500 text-white ml-1 hover:scale-110 hover:bg-green-600" rounded @click="handleConfirmClick">Đồng ý</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import EventName from '@/common/EventName';
import DOMPurify from 'dompurify';

export default {
    created() {
        EventBus.on(EventName.ShowDialog, this.handleShowDialog);
    },

    beforeUnmount() {
        EventBus.off(EventName.ShowDialog);
    },

    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,
            dialogData: {
                Title: '',
                Message: '',
                ConfirmAction: async () => {}
            }
        }
    },
    
    methods: {
        handleShowDialog(dialogData: any) {
            this.dialogData = dialogData;
            this.isShow = true;
        },

        async handleConfirmClick() {
            this.loading = true;
            if (this.dialogData.ConfirmAction !== undefined) {
                await this.dialogData.ConfirmAction();
            }

            this.loading = false;
            this.isShow = false;
        }
    },

    computed: {
        content() {
            return DOMPurify.sanitize(this.dialogData.Message);
        }
    }
}
</script>