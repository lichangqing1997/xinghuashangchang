import { api, apiClient } from './client';
import { FileAttachment } from '../types/models';
import { PageResult, PageParams } from '../types/api';
import { Result } from '../types/api';

export interface FileListParams extends PageParams {
  fileType?: string;
  businessType?: string;
}

export const fileApi = {
  /**
   * 上传文件
   */
  upload: async (file: File, businessType?: string, businessId?: number) => {
    const formData = new FormData();
    formData.append('file', file);
    if (businessType) formData.append('businessType', businessType);
    if (businessId) formData.append('businessId', businessId.toString());

    const response = await apiClient.post<Result<FileAttachment>>('/api/File/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  /**
   * 获取文件列表
   */
  getList: (params?: FileListParams) =>
    api.get<PageResult<FileAttachment>>('/api/File/list', params),

  /**
   * 获取文件详情
   */
  getById: (id: number) => api.get<FileAttachment>(`/api/File/${id}`),

  /**
   * 下载文件
   */
  download: (id: number) => {
    const token = localStorage.getItem('token');
    window.open(`/api/File/download/${id}?token=${token}`, '_blank');
  },

  /**
   * 删除文件
   */
  delete: (id: number) => api.delete<boolean>(`/api/File/${id}`),

  /**
   * 获取最新APK信息
   */
  getLatestApk: () => api.get<FileAttachment>('/api/File/latest-apk'),
};
