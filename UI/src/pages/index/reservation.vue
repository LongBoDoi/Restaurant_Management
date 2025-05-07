<template>
    <FormBookReservation />
</template>

<script lang="ts">
export default {
    created() {
        if (!this.$commonFunction.getSettingValue('DisplayBookingScreenForCustomer')) {
            this.$router.replace('/page-not-found');
        }
    }
}
</script>