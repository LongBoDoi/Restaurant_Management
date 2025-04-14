import { createStoreOnBase } from './baseStore';
import { Table } from '@/models';
import { tableService } from '@/services/tableService';

export const tableStore = createStoreOnBase<Table>('tableStore', tableService);