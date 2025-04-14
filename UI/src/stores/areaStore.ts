import { createStoreOnBase } from './baseStore';
import { Area } from '@/models';
import { areaService } from '@/services/areaService';

export const areaStore = createStoreOnBase<Area>('areaStore', areaService);