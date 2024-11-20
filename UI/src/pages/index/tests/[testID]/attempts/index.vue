<template>
    <VContainer style="height: 100%; display: flex; flex-direction: column; align-items: center; overflow-y: auto; padding: unset;">
        <VContainer style="max-width: 800px; padding: unset;">
            <template v-for="att, index in attempts">
                <VSpacer v-if="index > 0" style="height: 16px;" />
                <VCard link @click="handleAttemptClick(att.AttemptID)">
                    <VCardItem>
                        <MLHbox align="center">
                            <VIcon icon="mdi-account" />
                            <VLabel style="font-size: 1.5rem; margin-left: 8px;">{{ `${att.User?.FirstName} ${att.User?.LastName}` }}</VLabel>
                        </MLHbox>
                        <MLHbox align="center">
                            <VIcon icon="mdi-update" />
                            <VLabel style="margin-left: 8px;">{{ commonFunction.formatDate(att.CreatedDate) }}</VLabel>
                        </MLHbox>

                        <VLabel :style="{'margin-top': '16px', 'color': testStatusColor(att.IsFinished)}">{{ testStatusName(att.IsFinished) }}</VLabel>
                    </VCardItem>
                </VCard>
            </template>
        </VContainer>
    </VContainer>
</template>

<script lang="ts">
import { Attempt } from '@/models';

export default {
    async created() {
        this.attempts = await this.$service.AttemptService.getAttemptsFromTest(this.testID);
    },

    data() {
        return {
            attempts: <Attempt[]>[]
        }
    },

    methods: {
        testStatusName(isFinished:boolean) {
            return isFinished ? 'Finished' : 'Working';
        },

        testStatusColor(isFinished:boolean) {
            return isFinished ? 'rgb(var(--v-theme-error))' : 'rgb(var(--v-theme-warning))';
        },

        handleAttemptClick(attemptID:string) {
            this.$router.push({name: '//tests/[testID]/attempts/[attemptID]', params: {
                testID: this.testID,
                attemptID: attemptID
            }})
        }
    },

    computed: {
        testID() {
            return (this.$route.params as any).testID;
        },

        commonFunction() {
            return this.$commonFunction;
        }
    }
}
</script>