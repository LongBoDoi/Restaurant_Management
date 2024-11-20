<template>
    <MLVbox style="width: 100%; height: 100%; max-height: 100vh;">
        <!-- Toolbar -->
        <VToolbar color="primary">
            <VContainer style="display: flex;">
                <VBtn rounded style="margin-right: 24px; margin-left: auto;">Đăng nhập</VBtn>
            </VContainer>
        </VToolbar>

        <MLVbox class="main-background">
            <VContainer style="flex-shrink: 0; width: 100%; height: 100%; padding: unset; display: flex; align-items: center;">
                <div style="width: 50%; height: 100%; background-image: url(https://images.unsplash.com/photo-1712630514718-3830cc6c0d0a?w=400&auto=format&fit=crop&q=60&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8cmVzdGF1cmFudCUyMGJhY2tncm91bmR8ZW58MHx8MHx8fDA%3D); background-size: 100% 100%;" />
                <MLVbox style="width: 50%; height: 100%; padding-left: 24px; align-items: center; justify-content: center;">
                    <MLVbox style="align-items: center; max-width: 80%;">
                        <!-- Tên nhà hàng -->
                        <VLabel style="font-size: 3rem; font-weight: bold; color: rgb(var(--v-theme-primary))">Nhà Hàng 123</VLabel>

                        <!-- Địa chỉ nhà hàng -->
                        <MLHbox style="width: 100%;">
                            <VIcon color="primary" size="36" icon="mdi-map-marker" />
                            <VLabel style="font-size: 1.5rem; word-break: break-word; white-space: normal; margin-left: 8px;">Số 12 đường Giải Phóng, Xuân Đỉnh, Bắc Từ Liêm, Hà Nội, Việt Nam</VLabel>
                        </MLHbox>

                        <!-- SĐT nhà hàng -->
                        <MLHbox style="width: 100%;">
                            <VIcon color="primary" size="36" icon="mdi-phone" />
                            <VLabel style="font-size: 1.5rem; word-break: break-word; white-space: normal; margin-left: 8px;">0123456789</VLabel>
                        </MLHbox>

                        <!-- Mô tả nhà hàng -->
                        <VLabel style="width: 100%; margin-top: 12px;">Chúng tôi phục vụ những món ăn ngon nhất với giá cả hợp lý</VLabel>

                        <VSpacer style="height: 48px;" />
                        
                        <MLHbox style="width: 100%;">
                            <VBtn color="primary">Xem thực đơn</VBtn>
                            <VBtn style="margin-left: 16px;">Đặt bàn</VBtn>
                        </MLHbox>
                    </MLVbox>
                </MLVbox>
            </VContainer>

            <!-- chatbot -->
            <ChatbotWindow />
        </MLVbox>


        <!-- <MLHbox style="flex-grow: 1; overflow: hidden;" :stretch="true">
            <MLVbox class="navigation-bar">
                <router-link v-for="x in pageList" class="ml-hbox navigation-button" active-class="selected" :to="x.Url">
                    <img class="nav-button-icon" :src="x.Icon">
                    <span class="nav-button-text">{{x.Text}}</span>
                </router-link>
            </MLVbox>

            <div class="main-view-container">
                <router-view />
            </div>
        </MLHbox>

        <VDialog v-model="dialogData.Visible" max-width="500" persistent>
            <VCard min-height="200">
                <VCardTitle style="text-align: center;">{{ dialogData.Title }}</VCardTitle>
                <VCardText v-html="dialogData.Content">
                </VCardText>
                <VCardActions>
                    <VSpacer />
                    <VBtn @click="dialogData.Visible = false;">Cancel</VBtn>
                    <VBtn variant="tonal" @click="dialogData.ConfirmAction();">{{dialogData.ConfirmText}}</VBtn>
                </VCardActions>
            </VCard>
        </VDialog>

        <MLLoadingScreen v-if="showLoading" /> -->
    </MLVbox>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';

export default {
    created() {
        EventBus.on('ShowDialog', this.handleShowDialog);
        EventBus.on('CloseDialog', this.handleCloseDialog);
        EventBus.on('ToggleLoading', this.handleToggleLoading);
    },

    beforeUnmount() {
        EventBus.off('ShowDialog');
        EventBus.off('CloseDialog');
        EventBus.off('ToggleLoading');
    },

    data() {
        return {
            pageList: [{
                Url: '/tests',
                Text: 'Tests',
                Icon: '/src/resources/document-24.png'
            },
            {
                Url: '/manage-test',
                Text: 'Manage test',
                Icon: '/src/resources/wrench-24.png'
            }],

            dialogData: {
                Visible: false,
                Title: 'Popup Title',
                Content: 'Popup Content',
                ConfirmText: 'Save',
                ConfirmAction: () => {}
            },

            showLoading: false,
        }
    },

    methods: {
        handleHomeClick() {
            this.$router.push({name: '/'});
        },

        handleShowDialog(dialogData:any) {
            if (dialogData?.Title) {
                this.dialogData.Title = dialogData.Title;
            }
            if (dialogData?.Content) {
                this.dialogData.Content = dialogData.Content;
            }
            if (dialogData?.ConfirmText) {
                this.dialogData.ConfirmText = dialogData.ConfirmText;
            }
            if (dialogData?.ConfirmAction) {
                this.dialogData.ConfirmAction = dialogData.ConfirmAction;
            }
            this.dialogData.Visible = true;
        },

        handleCloseDialog() {
            this.dialogData.Visible = false;
        },

        handleToggleLoading(visible:any) {
            this.showLoading = visible;
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