import { menuItemService } from '@/services/menuItemService';
import { createStoreOnBase } from './baseStore';
import { MenuItem } from '@/models';
 
export const menuItemStore = createStoreOnBase<MenuItem>('menuItemStore', menuItemService);