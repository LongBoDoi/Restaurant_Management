<template>
    <VDialog persistent v-model:model-value="isShow" max-width="600">
        <VCard :disabled="loading">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>
            <VCardTitle class="d-flex align-center">
                {{ dialogData.Title }}
                <VBtn variant="plain" class="ml-auto" icon="mdi-close" @click="isShow = false" />
            </VCardTitle>
            <VCardText>
                <div v-html="content" />
            </VCardText>
            <VCardActions>
                <VBtn @click="isShow = false">Huỷ</VBtn>
                <VBtn style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary));" @click="handleConfirmClick">Đồng ý</VBtn>
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