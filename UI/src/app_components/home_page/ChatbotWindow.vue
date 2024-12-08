<template>
    <VMenu :close-on-content-click="false" persistent v-model:model-value="showWindow">
        <template v-slot:activator="{ props }">
            <VBtn v-bind="props" icon="mdi-chat" color="primary" style="position: fixed; right: 24px; bottom: 24px;" />
        </template>

        <VCard style="position: absolute; z-index: 10; right: 100%; width: 328px; height: 455px; margin-right: 16px; bottom: 0; display: flex; flex-direction: column;">
            <VCardTitle style="height: 48px; background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary)); display: flex; align-items: center;">
                <VIcon icon="mdi-robot" style="margin-bottom: 4px; margin-right: 12px;" />
                <span>Chatbot</span>
                <VIcon icon="mdi-close" style="margin-left: auto; cursor: pointer;" @click="showWindow = false;" />
            </VCardTitle>

            <VDivider />

            <!-- Nội dung tin nhắn -->
            <MLVbox ref="messages" style="flex-grow: 1; padding: 0.625rem 0.5rem; overflow-y: auto;">
                <template v-for="detail, index in conversation?.ChatbotConversationDetails">
                    <VSpacer v-if="index > 0" style="height: 8px; flex-grow: 0; flex-shrink: 0;" />

                    <span v-if="index === 0 || formatTime(detail.Timestamp) !== formatTime(conversation?.ChatbotConversationDetails[index - 1].Timestamp)" 
                        style="text-align: center; font-size: 0.85rem;"
                    >
                        {{ formatTime(detail.Timestamp) }}
                    </span>
                    <MLHbox :class="[
                        'chat-message-container',
                        {'sent-by-user' : detail.Sender === EnumChatbotSender.User}
                    ]">
                        <div class="chat-participant-avatar"
                            :style="{
                                'background-color': detail.Sender === EnumChatbotSender.Bot ? 'rgb(var(--v-theme-primary))' : '',
                                'border': detail.Sender === EnumChatbotSender.User ? '1px solid rgb(var(--v-theme-primary))' : ''
                            }"
                        >
                            <VIcon size="16" :icon="detail.Sender === EnumChatbotSender.Bot ? 'mdi-robot' : 'mdi-account'"
                                :style="{'color': `rgb(var(--v-theme${detail.Sender === EnumChatbotSender.Bot ? '-on' : ''}-primary))`}"
                            />
                        </div>

                        <VSpacer style="width: 4px; flex-shrink: 0; flex-grow: 0;" />

                        <VCard rounded :color="detail.Sender === $enumeration.EnumChatbotSender.User ? 'primary' : undefined" style="padding: 0.3rem 0.7rem;">{{ detail.Message }}</VCard>
                    </MLHbox>
                </template>

                <!-- Skeleton khi chờ phản hồi của bot -->
                <template v-if="gettingNewResponse">
                    <VSpacer style="height: 8px; flex-grow: 0; flex-shrink: 0;" />
                    <MLHbox class="chat-message-container">
                        <div class="chat-participant-avatar"
                            :style="{
                                'background-color': 'rgb(var(--v-theme-primary))',
                                'border': '1px solid rgb(var(--v-theme-primary))'
                            }"
                        >
                            <VIcon size="16" icon="mdi-robot"
                                :style="{'color': `rgb(var(--v-theme-on-primary))`}"
                            />
                        </div>

                        <div style="position: relative; display: flex; align-items: center; justify-content: center;">
                            <VSkeletonLoader color="onPrimary" style="margin-left: 4px;" width="50" height="100%" >
                            </VSkeletonLoader>
                            <VIcon style="position: absolute; color: rgb(var(--v-theme-on-primary))" icon="mdi-dots-horizontal" />
                        </div>
                    </MLHbox>
                </template>
            </MLVbox>

        <VDivider />

        <VCardActions style="flex-shrink: 0;">
            <VTextField ref="txtMessage" :disabled="waitingRequest" v-model:model-value="messageInput" @keypress.enter="sendNewMessage" density="compact" variant="outlined" hide-details rounded placeholder="Nhập tin nhắn..." />
            <VIcon :disabled="waitingRequest" @click="sendNewMessage" icon="mdi-send" color="primary" />
        </VCardActions>
    </VCard>
    </VMenu>
</template>

<script lang="ts">
import { ChatbotConversation, ChatbotConversationDetail } from '@/models';

export default {
    async created() {
        const chatbotConversationID = sessionStorage.getItem('chatbotConversationID');
        if (chatbotConversationID) {
            this.conversation = await this.$service.ChatbotService.getChatbotConversation(chatbotConversationID);

            //Nếu chưa có đoạn hội thoại trước đó thì tạo đoạn hội thoại mới
            if (!this.conversation) {
                this.conversation = await this.$service.ChatbotService.createNewConversation();
                if (!this.conversation) return;

                sessionStorage.setItem('chatbotConversationID', this.conversation.ConversationID);
            }

            this.$nextTick(() => {
                this.scrollToBottom();
            });
        }
    },

    data() {
        return {
            showWindow: <boolean>false,
            conversation: <ChatbotConversation|undefined>undefined,

            messageInput: <string>'',
            waitingRequest: <boolean>false,
            gettingNewResponse: <boolean>false
        }
    },

    methods: {
        /**
         * Đóng/Mở cửa sổ chat bot
         */
        async toggleChatbotWindow() {
            this.showWindow = !this.showWindow;
            if (this.showWindow) {
                this.$nextTick(() => {
                    (this.$refs.txtMessage as any).focus();
                });
            }
        },

        /**
         * Gửi tin nhắn mới
         */
        async sendNewMessage() {
            debugger
            if (this.messageInput && this.conversation) {
                this.waitingRequest = true;

                try {
                    const detail:ChatbotConversationDetail|undefined = await this.$service.ChatbotService.sendNewMessage(this.conversation.ConversationID, this.messageInput);
                    if (detail) {
                        this.conversation.ChatbotConversationDetails.push(detail);
                        this.$nextTick(() => {
                            this.scrollToBottom();
                        });
                        const oldMessage = this.messageInput;
                        this.messageInput = '';

                        this.gettingNewResponse = true;
                        const responseDetail:ChatbotConversationDetail|undefined = await this.$service.ChatbotService.getNewResponse(this.conversation.ConversationID, oldMessage);
                        if (responseDetail) {
                            this.conversation.ChatbotConversationDetails.push(responseDetail);
                            this.$nextTick(() => {
                                this.scrollToBottom();
                                (this.$refs.txtMessage as any).focus();
                            });
                        }
                    }
                } catch (e) {
                } finally {
                    this.gettingNewResponse = false;
                    this.waitingRequest = false;
                }
            }
        },

        /**
         * Cuộn xuống cuối đoạn hội thoại
         */
        scrollToBottom() {
            const div = (this.$refs.messages as any)?.getDiv();
            if (div) {
                div.scrollTop = div.scrollHeight;
            }
        },

        formatTime(date:Date|undefined) : string {
            if (date) {
                return this.$commonFunction.formatTime(date);
            }
            return '';
        }
    },

    computed: {
        EnumChatbotSender() {
            return this.$enumeration.EnumChatbotSender;
        }
    }
}
</script>

<style lang="scss" scoped>
.chat-message-container {
    max-width: 80%;

    &.sent-by-user {
        flex-direction: row-reverse;
        align-self: flex-end;
    }
}

.chat-participant-avatar {
    width: 24px;
    height: 24px;
    flex-shrink: 0;
    border-radius: 50%;
    margin-top: 4px;
    display: flex;
    align-items: center;
    justify-content: center;
}
</style>