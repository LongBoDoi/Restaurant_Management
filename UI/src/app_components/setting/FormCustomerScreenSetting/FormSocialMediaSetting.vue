<template>
    <VCard class="pa-6 ml-1 mr-1 bg-gray-50" style="border-radius: 24px;">
        <b style="font-size: 18px;">Liên kết mạng xã hội</b>

        <MLHbox align="center" class="mb-2 mt-4">
            <VCheckbox color="primary" hide-details style="width: 150px;" v-model:model-value="lstSocialLinks" :value="facebookData">
                <template #label>
                    <i class="fa-brands fa-facebook mr-2" style="font-size: 16px; color: #1877F2;" />
                    Facebook
                </template>
            </VCheckbox>

            <VTextField v-if="lstSocialLinks.includes(facebookData)" color="primary" placeholder="Nhập URL Facebook" density="compact" variant="outlined" class="ml-4" hide-details v-model:model-value="facebookData.Link" />
        </MLHbox>

        <MLHbox align="center" class="mb-2">
            <VCheckbox color="primary" hide-details style="width: 150px;" v-model:model-value="lstSocialLinks" :value="instagramData">
                <template #label>
                    <i class="fa-brands fa-instagram mr-2" style="font-size: 16px; color: #C13584;" />
                    Instagram
                </template>
            </VCheckbox>

            <VTextField v-if="lstSocialLinks.includes(instagramData)" color="primary" placeholder="Nhập URL Instagram" density="compact" variant="outlined" class="ml-4" hide-details v-model:model-value="instagramData.Link" />
        </MLHbox>

        <MLHbox align="center" class="mb-2">
            <VCheckbox color="primary" hide-details style="width: 150px; flex-shrink: 0;" v-model:model-value="lstSocialLinks" :value="youtubeData">
                <template #label>
                    <i class="fa-brands fa-youtube mr-2" style="font-size: 16px; color: red;" />
                    Youtube
                </template>
            </VCheckbox>

            <VTextField v-if="lstSocialLinks.includes(youtubeData)" color="primary" placeholder="Nhập URL Youtube" density="compact" variant="outlined" class="ml-4 flex-1" hide-details v-model:model-value="youtubeData.Link" />
        </MLHbox>

        <MLHbox align="center">
            <VCheckbox color="primary" hide-details style="width: 150px;" v-model:model-value="lstSocialLinks" :value="tiktokData">
                <template #label>
                    <i class="fa-brands fa-tiktok mr-2" style="font-size: 16px; color: black;" />
                    TikTok
                </template>
            </VCheckbox>

            <VTextField v-if="lstSocialLinks.includes(tiktokData)" color="primary" placeholder="Nhập URL TikTok" density="compact" variant="outlined" class="ml-4" hide-details v-model:model-value="tiktokData.Link" />
        </MLHbox>
    </VCard>
</template>

<script lang="ts">
import { Setting } from '@/models';
import { PropType } from 'vue';

interface SocialLink {
    Name: string,
    Link: string
}

export default {
    props: {
        setting: {
            type: Object as PropType<Setting>
        }
    },

    data() {
        return {
            lstSocialLinks: <SocialLink[]>[]
        }
    },

    methods: {
        prepareData() {
            if (this.setting) {
                this.setting.SettingValue = JSON.stringify(this.lstSocialLinks);
            }
        }
    },

    watch: {
        setting() {
            if (this.setting && this.setting.Value) {
                this.lstSocialLinks = JSON.parse(this.setting.Value);
            }
        }
    },

    computed: {
        facebookData() {
            var data = this.lstSocialLinks.find(sl => sl.Name === 'facebook');
            if (!data) {
                data = {
                    Name: 'facebook',
                    Link: ''
                } as SocialLink;
            }
            
            return data;
        },

        instagramData() {
            var data = this.lstSocialLinks.find(sl => sl.Name === 'instagram');
            if (!data) {
                data = {
                    Name: 'instagram',
                    Link: ''
                } as SocialLink;
            }
            
            return data;
        },

        youtubeData() {
            var data = this.lstSocialLinks.find(sl => sl.Name === 'youtube');
            if (!data) {
                data = {
                    Name: 'youtube',
                    Link: ''
                } as SocialLink;
            }
            
            return data;
        },

        tiktokData() {
            var data = this.lstSocialLinks.find(sl => sl.Name === 'tiktok');
            if (!data) {
                data = {
                    Name: 'tiktok',
                    Link: ''
                } as SocialLink;
            }
            
            return data;
        }
    }
}
</script>

<style lang="scss" scoped>
@import url(https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css);
</style>