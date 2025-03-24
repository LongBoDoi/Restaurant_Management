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
import { session } from './common/Session';
import ServiceInterface from './services';
import EnumerationInterface from './common/Enumeration';
import CommonFunction from './common/CommonFunction';
import VueTheMask from 'vue-the-mask';
import EventName from './common/EventName';
import money from 'v-money';
import LocalStorageKey from './common/LocalStorageKey';
import Config from './common/Config';
import CommonValue from './common/CommonValue';

const app = createApp(App);

app.use(vuetify)
    .use(router)
    .use(pinia)
    .use(VueTheMask)
    .use(money, {precision: 4});

app.config.globalProperties.$service = new ServiceInterface();
app.config.globalProperties.$session = session;
app.config.globalProperties.$enumeration = new EnumerationInterface();
app.config.globalProperties.$commonFunction = CommonFunction;
app.config.globalProperties.$localStorageKey = LocalStorageKey;
app.config.globalProperties.$eventName = EventName;
app.config.globalProperties.$config = Config;
app.config.globalProperties.$commonValue = CommonValue;

app.mount('#app')
