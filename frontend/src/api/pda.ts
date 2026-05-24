import { api } from './client';
import { ScanRecord, PdaScanInInput, PdaScanOutInput, PdaShelfInput, MobileReceiveInput } from '../types/models';
import { PageResult, PageParams } from '../types/api';

export const pdaApi = {
  scanIn: (data: PdaScanInInput) => api.post<string>('/api/Pda/scan-in', data),

  scanOut: (data: PdaScanOutInput) => api.post<string>('/api/Pda/scan-out', data),

  shelf: (data: PdaShelfInput) => api.post<string>('/api/Pda/shelf', data),

  mobileReceive: (data: MobileReceiveInput) => api.post<string>('/api/Pda/mobile-receive', data),

  getRecords: (params?: { barcode?: string; operationType?: string } & PageParams) =>
    api.get<PageResult<ScanRecord>>('/api/Pda/records', params),
};
