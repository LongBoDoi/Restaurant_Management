<template>
    <MLVbox align="center">
        <template v-if="loading">
            <MLLoadingScreen :contained="true" />
        </template>
        <template v-else>
            <VLabel style="font-size: 2rem; font-weight: bold; flex-shrink: 0;">Test {{ testData?.TestNumber }}</VLabel>

            <VTabs style="width: 100%; flex-shrink: 0;" fixed-tabs>
                <VTab :value="0" :to="{name: '//tests/[testID]/questions', params: {testID: testID}}">Questions</VTab>
                <VTab :to="{name: '//tests/[testID]/attempts/', params: {testID: testID}}">Attempts</VTab>
            </VTabs>

            <VSpacer style="height: 24px; flex-shrink: 0; flex-grow: 0;" />

            <VContainer style="position: relative; flex-grow: 1; padding: unset; overflow: hidden; max-width: unset;">
                <RouterView />
            </VContainer>
        </template>
    </MLVbox>
</template>

<script lang="ts">
import { Test } from '@/models';
import { testStore } from '@/stores/testStore';
import { mapActions } from 'pinia';

export default {
    async created() {
        if (this.$route.name === '//tests/[testID]') {
            this.$router.replace({name: '//tests/[testID]/questions', params: {testID: this.testID}});
        }

        this.loading = true;
        const testData = await this.$service.TestService.getTestInfo(this.testID);
        if (testData) {
            this.testData = testData;
            this.loading = false;
        }
    },

    beforeUnmount() {
        this.resetTestData();
    },

    data() {
        return {
            loading: <boolean>false,

            testData: <Test|undefined>undefined
        }
    },

    computed: {
        testID() {
            return (this.$route.params as any).testID;
        }
    },

    methods: {
        ...mapActions(testStore, ['resetTestData'])
    }
}
</script>

<style lang="scss" scoped>
.test-detail-title {
    width: fit-content;
    font-weight: bold;
    font-size: 2rem;
    flex-shrink: 0;
}

.ml-exercise-quick-view-item {
    width: 72px;
    background-color: rgb(var(--v-theme-surface));
}
</style>