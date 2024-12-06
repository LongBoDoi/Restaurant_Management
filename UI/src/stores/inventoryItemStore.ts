import { inventoryItemService } from '@/services/inventoryItemService';
import { createStoreOnBase } from './baseStore';

export const inventoryItemStore = createStoreOnBase('inventoryItemStore', inventoryItemService);