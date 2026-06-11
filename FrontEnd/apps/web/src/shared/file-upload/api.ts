import { apiClient } from '@/core/api/client';
import type { UploadedFileDto } from './types';

const DEFAULT_URL = `${import.meta.env.VITE_API_VERSION ? `/api/${import.meta.env.VITE_API_VERSION}` : '/api/v1'}/files`;

export async function uploadFile(
  file: File,
  uploadUrl: string = DEFAULT_URL,
  onProgress?: (percent: number) => void,
): Promise<UploadedFileDto> {
  const form = new FormData();
  form.append('file', file);

  const res = await apiClient.post<UploadedFileDto>(uploadUrl, form, {
    headers: { 'Content-Type': 'multipart/form-data' },
    onUploadProgress: (event) => {
      if (!onProgress || !event.total) return;
      onProgress(Math.round((event.loaded * 100) / event.total));
    },
  });

  return res.data;
}
