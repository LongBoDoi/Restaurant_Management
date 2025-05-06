<template>
    <VDialog width="480" persistent :model-value="isShow">
        <VCard :disabled="loading" style="border-radius: 36px;">
            <template v-slot:loader>
                <VProgressLinear v-if="loading" indeterminate color="primary" />
            </template>

            <VCardTitle class="bg-gradient-to-r from-teal-600 to-green-500 px-6 py-4 d-flex justify-between items-center">
                <h2 class="text-white text-xl font-bold">Hồ sơ cá nhân</h2>
                <VBtn variant="plain" style="color: white; opacity: 1; width: 40px; height: 40px;" class="ml-auto" icon="mdi-close" @click="isShow = false;" />
            </VCardTitle>

            <VForm ref="form">
            <div class="pa-6 space-y-4">
                <div className="flex items-center mb-6 space-x-4">
                    <div class="relative">
                        <div style="width: 96px; height: 96px; border: 4px solid rgb(220 252 231);" class="rounded-full bg-gray-200 flex items-center justify-center overflow-hidden border-4 border-green-100">
                            <img v-if="imageUrl" :src="imageUrl" >
                            <VIcon v-else class="text-gray-400" icon="mdi-account-outline" style="font-size: 48px;" />
                        </div>

                        <VBtn 
                            style="width: fit-content !important; height: fit-content !important; padding: 8px !important; font-size: 10px !important;"
                            class="absolute bottom-0 right-0 bg-green-500 text-white hover:bg-green-600 transition-all duration-200 transform hover:scale-105"
                            icon="mdi-pencil-outline" 
                            @click="selectImage"
                        />

                        <VFileInput
                            ref="imageSelector"
                            v-show="false"
                            accept="image/*"
                            v-model:model-value="imageFile"
                            v-on:update:model-value="getImageUrls"
                        />
                    </div>
                    <div>
                        <h4 className="text-lg font-bold text-gray-800">{{ data.CustomerName }}</h4>
                        <p className="text-gray-500">{{ $commonFunction.formatPhoneNumber(data.PhoneNumber) ?? data.Email ?? '' }}</p>
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700">
                        Họ tên <span style="color: red;">*</span>
                    </label>
                    <VTextField
                        hide-details
                        variant="outlined"
                        density="compact"
                        color="primary"

                        class="w-full mt-1"
                        placeholder="Nhập họ tên của bạn"
                        :rules="[$commonValue.textFieldRequireRule]"
                        v-model:model-value="data.CustomerName"
                    />
                </div>

                <div class="flex space-x-4">
                    <div style="flex-grow: 0.25;">
                        <label class="block text-sm font-medium text-gray-700">
                            Số điện thoại <span style="color: red;">*</span>
                        </label>
                        <VTextField
                            hide-details
                            variant="outlined"
                            density="compact"
                            color="primary"

                            class="w-full mt-1"
                            placeholder="Nhập SĐT"
                            v-mask="'0### ### ###'"
                            :rules="[$commonValue.textFieldRequireRule]"
                            v-model:model-value="data.PhoneNumber"
                        />
                    </div>

                    <div style="flex-grow: 0.75;">
                        <label class="block text-sm font-medium text-gray-700">
                            Email
                        </label>
                        <VTextField
                            hide-details
                            variant="outlined"
                            density="compact"
                            color="primary"

                            class="w-full mt-1"
                            placeholder="Nhập email"
                            :rules="[$commonValue.textEmailRule]"
                            v-model:model-value="data.Email"
                        />
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700">
                        Địa chỉ
                    </label>
                    <VTextarea
                        variant="outlined"
                        color="primary"
                        no-resize
                        hide-details

                        class="w-full mt-1"
                        placeholder="Nhập địa chỉ"
                        v-model:model-value="data.Address"
                    />
                </div>
            </div>
            </VForm>

            <VCardActions class="px-6 py-4 bg-gray-50 border-t flex justify-end space-x-3">
                <VSpacer />

                <VBtn class="border-gray-300 text-gray-700" variant="outlined" rounded @click="isShow = false">Đóng</VBtn>
                <VBtn variant="tonal" class="bg-green-500 hover:bg-green-600 text-white ml-1" rounded @click="handleSaveClick">Cập nhật</VBtn>
            </VCardActions>
        </VCard>
    </VDialog>
</template>

<script lang="ts">
import EventBus from '@/common/EventBus';
import { Customer } from '@/models';

export default {
    data() {
        return {
            isShow: <boolean>false,
            loading: <boolean>false,

            data: <Customer>Object.assign({}, this.$session.UserData as Customer),
            imageUrl: <string>'',
            imageFile: <File|undefined>undefined,
        }
    },

    created() {
        EventBus.on(this.$eventName.ShowFormUpdatePersonalInfo, () => {
            this.isShow = true;
            this.imageUrl = this.$commonFunction.getImageUrl(this.data.ImageUrl);
        });
    },

    beforeUnmount() {
        EventBus.off(this.$eventName.ShowFormUpdatePersonalInfo);
    },

    methods: {
        async handleSaveClick() {
            const form = this.$refs.form as any;
            const formValid:boolean = (await form.validate()).valid;

            if (!formValid) return;

            this.loading = true;

            this.data.EditMode = this.$enumeration.EnumEditMode.Edit;
            const success = await this.$service.CustomerService.updatePersonalInfo(this.data, this.imageFile);
            if (success) {
                Object.assign(this.$session.UserData, this.data);
                this.$commonFunction.showToastMessage('Cập nhật thông tin cá nhân thành công', 'success');
                this.isShow = false;
            }

            this.loading = false;
        },

        selectImage() {
            const imageSelector = (this.$refs.imageSelector as any);

            const input = imageSelector.$el.querySelector('input[type=file]');
            input.click();
        },

        getImageUrls(file:File|File[]) {
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.imageFile = file as File;
                    this.imageUrl = e.target?.result as string;
                };
                reader.readAsDataURL(file as File);
            }
        },
    }
}
</script>