<template>
    <VMenu :close-on-content-click="false" persistent v-model:model-value="showWindow">
        <template v-slot:activator="{ props }">
            <VBtn v-bind="props" icon="mdi-chat" color="primary" style="position: fixed; right: 24px; bottom: 24px;" />
        </template>

        <VCard class="rounded-lg" style="position: absolute; z-index: 10; right: 100%; width: 328px; height: 455px; margin-right: 16px; bottom: 0; display: flex; flex-direction: column;">
            <VCardTitle class="px-4 py-2" style="background-color: rgb(var(--v-theme-primary)); color: rgb(var(--v-theme-on-primary)); display: flex; align-items: center;">
                <VIcon icon="mdi-robot" style="margin-bottom: 4px; margin-right: 12px;" />
                <span>Chatbot</span>
                <VIcon icon="mdi-close" style="margin-left: auto; cursor: pointer;" @click="showWindow = false;" />
            </VCardTitle>

            <VDivider />

            <!-- Nội dung tin nhắn -->
            <MLVbox ref="messages" style="flex-grow: 1; padding: 0.625rem 0.5rem; overflow-y: auto;">
                <template v-for="detail, index in displayMessages" :key="index">
                    <VSpacer v-if="index > 0" style="height: 8px; flex-grow: 0; flex-shrink: 0;" />

                    <span v-if="index === 0 || formatTime(detail.timestamp) !== formatTime(conversationDetails[index - 1].timestamp)" 
                        style="text-align: center; font-size: 0.85rem;"
                    >
                        {{ formatTime(detail.timestamp) }}
                    </span>
                    <MLHbox :class="[
                        'chat-message-container',
                        {'sent-by-user' : detail.role === 'user'}
                    ]">
                        <div class="chat-participant-avatar"
                            :style="{
                                'background-color': detail.role === 'assistant' ? 'rgb(var(--v-theme-primary))' : '',
                                'border': detail.role === 'user' ? '1px solid rgb(var(--v-theme-primary))' : ''
                            }"
                        >
                            <VIcon size="16" :icon="detail.role === 'assistant' ? 'mdi-robot' : 'mdi-account'"
                                :style="{'color': `rgb(var(--v-theme${detail.role === 'assistant' ? '-on' : ''}-primary))`}"
                            />
                        </div>

                        <VSpacer style="width: 4px; flex-shrink: 0; flex-grow: 0;" />

                        <!-- Tin nhắn -->
                        <VCard 
                            class="rounded-lg px-4"
                            rounded 
                            :color="detail.role === 'user' ? 'primary' : undefined"
                            style="padding: 0.3rem 0.7rem; white-space: pre-line;"
                        >
                            <span v-html="formatMessage(detail.content)" />
                        </VCard>
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
                            <VSkeletonLoader color="onPrimary" style="margin-left: 4px;" width="50" height="100%" class="rounded-lg">
                            </VSkeletonLoader>
                            <VIcon style="position: absolute; color: rgb(var(--v-theme-on-primary))" icon="mdi-dots-horizontal" />
                        </div>
                    </MLHbox>
                </template>
            </MLVbox>

        <VDivider />

        <VCardActions style="flex-shrink: 0;" class="px-4 py-2 bg-gray-50">
            <VTextField autofocus color="primary" ref="txtMessage" :disabled="waitingRequest" v-model:model-value="messageInput" @keyup.enter="sendNewMessage" density="compact" variant="outlined" hide-details rounded placeholder="Nhập tin nhắn..." />
            <VIcon :disabled="waitingRequest" @click="sendNewMessage" icon="mdi-send" color="primary" />
        </VCardActions>
    </VCard>
    </VMenu>
</template>

<script lang="ts">
import { ChatbotConversationDetail } from '@/models';

export default {
    async created() {
        const conversationDetails = sessionStorage.getItem('chatbotConversation');
        if (conversationDetails) {
            this.conversationDetails = JSON.parse(conversationDetails) as ChatbotConversationDetail[];
        } else {
            this.conversationDetails = await this.$service.ChatbotService.createNewConversation();
        }
        // if (chatbotConversationID) {
        //     this.conversation = await this.$service.ChatbotService.getChatbotConversation(chatbotConversationID);
        // }

        // //Nếu chưa có đoạn hội thoại trước đó thì tạo đoạn hội thoại mới
        // if (!this.conversation) {
        //     this.conversation = await this.$service.ChatbotService.createNewConversation();
        //     if (!this.conversation) return;

        //     sessionStorage.setItem('chatbotConversationID', this.conversation.ConversationID);
        // }

        // this.$nextTick(() => {
        //     this.scrollToBottom();
        // });
    },
    
    watch: {
        showWindow(newValue: boolean) {
            if (newValue) {
                this.$nextTick(() => {
                    this.scrollToBottom();
                });
            }
        }
    },

    data() {
        return {
            showWindow: <boolean>false,
            conversationDetails: <ChatbotConversationDetail[]>[],

            messageInput: <string>'',
            waitingRequest: <boolean>false,
            gettingNewResponse: <boolean>false,

            userID: <string>this.$commonValue.NewGuid()
        }
    },

    methods: {
        /**
         * Gửi tin nhắn mới
         */
        async sendNewMessage() {
            if (this.messageInput) {
                this.waitingRequest = true;

                this.conversationDetails.push({
                    role: 'user',
                    content: this.messageInput
                } as ChatbotConversationDetail);
                // sessionStorage.setItem('chatbotConversation', JSON.stringify(this.conversationDetails));

                await this.$nextTick();
                this.scrollToBottom();
                this.waitingRequest = false;

                this.gettingNewResponse = true;
                await this.$nextTick();
                this.scrollToBottom();
                const message = this.messageInput;
                this.messageInput = '';
                const responseDetail = await this.$service.ChatbotService.getNewResponse(this.conversationDetails, message);
                if (responseDetail) {
                    this.conversationDetails.push(responseDetail)
                    // sessionStorage.setItem('chatbotConversation', JSON.stringify(this.conversationDetails));

                    await this.$nextTick();
                    this.scrollToBottom();
                }

                this.gettingNewResponse = false;

                this.$nextTick(() => {
                    (this.$refs.txtMessage as any).focus();
                });
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
        },

        formatMessage(input: string) {
            const formattedInput = input.replace(/\\n/g, '<br>').replace(/\*\*(.*?)\*\*/g, "<b>$1</b>").replace(/'/g, '').replace(/\\/g, '');
            return formattedInput;
        }
    },

    computed: {
        EnumChatbotSender() {
            return this.$enumeration.EnumChatbotSender;
        },

        displayMessages() {
            return this.conversationDetails.filter(x => x.role !== 'system');
        }
    },
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