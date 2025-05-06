import Role from '@/models/Role';
import { createStoreOnBase } from './baseStore';
import { roleService } from '@/services/roleService';

export const roleStore = createStoreOnBase<Role>('roleStore', roleService);