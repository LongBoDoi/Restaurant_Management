import { menuItemService } from '@/services/menuItemService';
import { createStoreOnBase } from './baseStore';
 
export const menuItemStore = createStoreOnBase('menuItemStore', menuItemService);