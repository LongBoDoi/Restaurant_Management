<template>
    <VContainer ref="mainContainer" style="display: flex; align-items: center; justify-content: center;" >
        <MLVbox style="width: 100%; height: 75%; overflow: hidden;" align="center">
            <VLabel style="font-size: 3rem; font-weight: bold; flex-shrink: 0; color: rgb(var(--v-theme-primary))">Thực đơn</VLabel>

            <VSpacer style="height: 24px; flex-grow: 0; flex-shrink: 0;" />

            <VCarousel color="primary" style="background-color: rgba(var(--v-theme-primary), 0.7); border-radius: 8px;" show-arrows="hover" hide-delimiter-background cycle height="100%">
                <VCarouselItem src="https://nhahangamthucviet.com/img/01/menu%20%E1%BA%A9m%20th%E1%BB%B1c%20vi%E1%BB%87t%201-2.jpg" />
                <VCarouselItem src="https://nhahangamthucviet.com/img/01/menu%20%E1%BA%A9m%20th%E1%BB%B1c%20vi%E1%BB%87t%201-3.jpg" />
                <VCarouselItem src="https://nhahangamthucviet.com/img/01/menu%20%E1%BA%A9m%20th%E1%BB%B1c%20vi%E1%BB%87t%201-4.jpg" />
                <VCarouselItem src="https://nhahangamthucviet.com/img/01/menu%20%E1%BA%A9m%20th%E1%BB%B1c%20vi%E1%BB%87t%201-5.jpg" />
            </VCarousel>
        </MLVbox>
    </VContainer>
</template>

<script lang="ts">
export default {
}
</script>