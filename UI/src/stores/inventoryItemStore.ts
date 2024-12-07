import { inventoryItemService } from '@/services/inventoryItemService';
import { createStoreOnBase } from './baseStore';
import { InventoryItem } from '@/models';

export const inventoryItemStore = createStoreOnBase<InventoryItem>('inventoryItemStore', inventoryItemService);