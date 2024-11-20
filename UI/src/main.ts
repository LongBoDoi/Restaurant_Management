/**
 * main.ts
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import vuetify from './plugins/vuetify'
import pinia from './stores'
import router from './router'

// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue';
import SessionInterface from './common/Session';
import ServiceInterface from './services';
import EnumerationInterface from './common/Enumeration';
import CommonFunction from './common/CommonFunction';

const app = createApp(App);

app.use(vuetify)
    .use(router)
    .use(pinia);

app.config.globalProperties.$service = new ServiceInterface();
app.config.globalProperties.$session = {} as SessionInterface;
app.config.globalProperties.$enumeration = new EnumerationInterface();
app.config.globalProperties.$commonFunction = new CommonFunction();

app.mount('#app')
