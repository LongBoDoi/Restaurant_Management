/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { createRouter, createWebHistory } from 'vue-router/auto'
import { setupLayouts } from 'virtual:generated-layouts'
import { routes } from 'vue-router/auto-routes';
import NotFoundPage from '@/app_components/NotFoundPage.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: setupLayouts(routes),
});

// Workaround for https://github.com/vitejs/vite/issues/11804
router.onError((err, to) => {
  if (err?.message?.includes?.('Failed to fetch dynamically imported module')) {
    if (!localStorage.getItem('vuetify:dynamic-reload')) {
      console.log('Reloading page to fix dynamic import error')
      localStorage.setItem('vuetify:dynamic-reload', 'true')
      location.assign(to.fullPath)
    } else {
      console.error('Dynamic import error, reloading page did not fix it', err)
    }
  } else {
    console.error(err)
  }
})

router.isReady().then(() => {
  localStorage.removeItem('vuetify:dynamic-reload')
});

const routerMapPermissions = {
  '/management/dashboard': 'ViewReport',
  '/management/order': 'ManageOrder',
  '/management/table': 'ManageTable',
  '/management/area': 'ManageTable',
  '/management/reservation': 'ManageTable',
  '/management/menu': 'ManageMenu',
  '/management/menu-category': 'ManageMenu',
  '/management/custom-menu-request': 'ManageMenu',
  '/management/inventory': 'ManageInventory',
  '/management/inventory-category': 'ManageInventory',
  '/management/employee': 'ManageEmployee',
  '/management/customer': 'ManageCustomer',
  '/management/permission': 'ManagePermission',
  '/management/setting': 'ManageSetting',
} as any;

router.addRoute({
  path: '/:pathMatch(.*)*',
  name: 'NotFoundPage',
  component: NotFoundPage
})

export { routerMapPermissions };

export default router;
