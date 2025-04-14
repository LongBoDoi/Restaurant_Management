import { createStoreOnBase } from './baseStore';
import InventoryItemCategory from '@/models/InventoryItemCategory';
import { inventoryItemCategoryService } from '@/services/inventoryItemCategoryService';

export const inventoryItemCategoryStore = createStoreOnBase<InventoryItemCategory>('inventoryItemCategoryStore', inventoryItemCategoryService);