export interface UploadedFileDto {
  id: string;
  fileName: string;
  contentType: string;
  size: number;
  uploadedOnUtc: string;
}

export interface FileUploadProps {
  /** Called after a successful upload with server metadata. */
  onUploaded?: (file: UploadedFileDto) => void;
  /** MIME types, e.g. `image/png,image/jpeg`. */
  accept?: string;
  /** Max bytes (client-side guard; server enforces too). */
  maxBytes?: number;
  /** Override API path (default `/api/v1/files`). */
  uploadUrl?: string;
  disabled?: boolean;
  className?: string;
}
