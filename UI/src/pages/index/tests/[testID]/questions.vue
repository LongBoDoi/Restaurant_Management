<template>
    <VContainer style="display: flex; flex-direction: column; align-items: center; max-height: 100%; max-width: unset;">
        <MLLoadingScreen contained v-if="loading" />

        <!-- Màn thông tin bắt đầu -->
        <template v-if="testData && !testData?.UserAttempt">
            <VCard style="width: 500px; min-height: 200px; display: flex; flex-direction: column;">
                <VCardText style="flex-grow: 1;">
                    <!-- Thời gian -->
                    <MLHbox>
                        <VLabel style="font-weight: bold;">Time Limit</VLabel>
                        <VLabel style="margin-left: auto;">{{ timeLimit }}</VLabel>
                    </MLHbox>

                    <!-- Số lượng bài tập -->
                    <MLHbox>
                        <VLabel style="font-weight: bold;">Number of exercises</VLabel>
                        <VLabel style="margin-left: auto;">{{ testData.Exercises.length }}</VLabel>
                    </MLHbox>

                    <!-- Chi tiết số lượng bài tập -->
                    <MLVbox style="margin-left: 24px;">
                        <!-- Bài nghe -->
                        <MLHbox v-if="listeningExercises.length">
                            <VLabel style="font-weight: bold; font-size: 0.9rem;">Listening</VLabel>
                            <VLabel style="margin-left: auto; font-size: 0.9rem;">{{ listeningExercises.length }}</VLabel>
                        </MLHbox>

                        <!-- Bài ngữ pháp -->
                        <MLHbox v-if="listeningExercises.length">
                            <VLabel style="font-weight: bold; font-size: 0.9rem;">Grammar</VLabel>
                            <VLabel style="margin-left: auto; font-size: 0.9rem;">{{ grammarExercises.length }}</VLabel>
                        </MLHbox>
                    </MLVbox>
                </VCardText>

                <VCardActions>
                    <VBtn variant="tonal" style="width: 100%;" @click="handleStartTestClick">START TEST</VBtn>
                </VCardActions>
            </VCard>
        </template>
        <template v-if="testData && testData.UserAttempt">
            <FormTestDetail style="width: 100%;" />
        </template>
    </VContainer>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { testStore } from '@/stores/testStore';
import { mapActions, mapState } from 'pinia';

export default {
    async created() {
        this.loading = true;

        await this.getTestDetail(this.testID);
        this.loading = false;
    },

    data() {
        return {
            loading: <boolean>false
        }
    },

    computed: {
        ...mapState(testStore, ['testData', 'listeningExercises', 'grammarExercises']),

        testID() {
            return (this.$route.params as any).testID;
        },

        /**
         * Giới hạn thời gian
         */
        timeLimit() {
            return this.testData?.TimeLimit ? this.$commonFunction.formatTimeBySecond(this.testData.TimeLimit) : 'Unset';
        }
    },

    methods: {
        ...mapActions(testStore, ['getTestDetail', 'makeAnAttempt']),

        /**
         * Nhấn nút Bắt đầu làm bài test
         */
        handleStartTestClick() {
            if (this.testData) {
                EventBus.emit('ShowDialog', {
                    Title: 'Start Test',
                    Content: `Do you want to start Test ${this.testData.TestNumber}?`,
                    ConfirmText: 'Start',
                    ConfirmAction: async () => {
                        EventBus.emit('ToggleLoading', true);

                        await this.makeAnAttempt(this.testID);
                        EventBus.emit('ToggleLoading', false);
                        EventBus.emit('CloseDialog');
                    }
                });
            }
        },
    }
}
</script>