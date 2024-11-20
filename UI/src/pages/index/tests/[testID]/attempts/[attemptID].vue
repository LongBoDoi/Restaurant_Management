<template>
    <MLLoadingScreen contained v-if="loading" />

    <FormTestDetail />
</template>

<script lang="ts">
import { testStore } from '@/stores/testStore';
import { mapActions } from 'pinia';

export default {
    async created() {
        this.loading = true;

        const testID = (this.$route.params as any).testID;
        const attemptID = (this.$route.params as any).attemptID;
        await this.getTestDetail(testID, attemptID);
        this.loading = false;
    },

    data() {
        return {
            loading: <boolean>false
        }
    },

    methods: {
        ...mapActions(testStore, ['getTestDetail'])
    }
}
</script>