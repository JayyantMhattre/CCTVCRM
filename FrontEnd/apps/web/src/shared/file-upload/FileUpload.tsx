/**
 * Reusable file upload control — platform infrastructure, not a feature page.
 *
 * Usage:
 *   <FileUpload accept="image/*" maxBytes={5_000_000} onUploaded={(f) => ...} />
 */

import { useRef, useState } from 'react';
import { uploadFile } from './api';
import type { FileUploadProps } from './types';
import { useToast } from '@/shared/ui/toast';
import { useApiError } from '@/shared/hooks/useApiError';
import { Spinner } from '@/shared/components/Spinner';

export function FileUpload({
  onUploaded,
  accept,
  maxBytes = 10 * 1024 * 1024,
  uploadUrl,
  disabled = false,
  className = '',
}: FileUploadProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const toast = useToast();
  const { extractMessage } = useApiError();
  const [progress, setProgress] = useState<number | null>(null);
  const [busy, setBusy] = useState(false);

  async function handleChange(fileList: FileList | null) {
    const file = fileList?.[0];
    if (!file) return;

    if (file.size > maxBytes) {
      toast.error(`File exceeds maximum size (${Math.round(maxBytes / 1024 / 1024)} MB).`);
      return;
    }

    setBusy(true);
    setProgress(0);
    try {
      const result = await uploadFile(file, uploadUrl, setProgress);
      toast.success('File uploaded.');
      onUploaded?.(result);
    } catch (err) {
      toast.error(extractMessage(err));
    } finally {
      setBusy(false);
      setProgress(null);
      if (inputRef.current) inputRef.current.value = '';
    }
  }

  return (
    <div className={className}>
      <label className="btn btn-outline-primary btn-sm mb-0">
        {busy ? <Spinner size="sm" /> : 'Choose file'}
        <input
          ref={inputRef}
          type="file"
          className="d-none"
          accept={accept}
          disabled={disabled || busy}
          onChange={(e) => void handleChange(e.target.files)}
        />
      </label>
      {progress !== null && (
        <div className="progress mt-2" style={{ height: 6 }}>
          <div
            className="progress-bar"
            role="progressbar"
            style={{ width: `${progress}%` }}
            aria-valuenow={progress}
            aria-valuemin={0}
            aria-valuemax={100}
          />
        </div>
      )}
    </div>
  );
}
