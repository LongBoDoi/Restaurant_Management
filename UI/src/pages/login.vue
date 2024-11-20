<template>
    <div class="me-login-background">
        <MLVbox class='me-login-window' :stretch="true">
            <!-- Màn loading -->
            <MLLoadingScreen v-if="isLoading" />

            <MLVbox style="padding: 24px;" align="center" :stretch="true">
                <!-- Logo -->
                <img className='me-login-logo' src='/src/resources/home-logo.png' />

                <!-- Tiêu đề -->
                <b className='me-login-title'>Login</b>

                <!-- Tên đăng nhập + Mật khẩu -->
                <VForm ref="form" @submit="onLogin">
                    <MLVbox style="width: 100%; margin-top: 16px">
                        <MLTextField ref="txtUsername" placeholder="Username" :required="true" :autofocus="true" style="margin-bottom: 8px;" />
                        <MLTextField ref="txtPassword" type="password" placeholder="Password" :required="true" style="margin-bottom: 16px;" />

                        <VBtn @click="onLogin" color="#A7ADB2" text="LOG IN" :height="40" style="font-size: 16px; color: white" />
                    </MLVbox>
                </VForm>
            </MLVbox>
        </MLVbox>
    </div>
</template>

<script lang="ts">
import { UserLogin, MLActionResult } from '@/models';

export default {
    data() {
        return {
            isLoading: false
        }
    },

    methods: {
        async onLogin() {
            const form:any = this.$refs.form;
            if (form?.isValid) {
                this.isLoading = true;

                try {
                    const result:MLActionResult|undefined = await this.$service.UserLoginService.login({
                        UserName: (this.$refs.txtUsername as any).getValue(),
                        Password: (this.$refs.txtPassword as any).getValue()
                    } as UserLogin);
                    if (result?.Success) {
                        debugger
                        localStorage.setItem('authToken', result.Data);
                        this.$service.UserLoginService.configApi();
                        this.$router.replace({name: '/'});
                        return;
                    } else {
                        localStorage.removeItem('authToken');
                    }
                } catch (ex) {
                    localStorage.removeItem('authToken');
                } finally {
                    this.isLoading = false;
                }
            }
        }
    },
}
</script>

<style lang="scss" scoped>
.me-login-background {
    width: 100vw;
    height: 100vh;
    position: relative;
    
    background-color: rgb(var(--v-theme-primary));
    display: flex;
    align-items: center;
    justify-content: center;

    .me-login-window {
        width: 600px;
        height: 400px;
        background-color: #8B929A;
        border-radius: 8px;

        .me-login-logo {
            width: 200px;
            height: 50px;
        }

        .me-login-title {
            font-size: var(--big-font-size);
            color: white;
            margin-top: 16px;
            width: auto;
        }
    }

    .me-version-info {
        position: absolute;
        right: 0;
        bottom: 0;
        font-style: italic;
        margin-right: 4px;
        margin-bottom: 4px;
    }
}
</style>