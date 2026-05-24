import { useState } from 'react';
import { Upload, Button, message, Space, List, Typography } from 'antd';
import { UploadOutlined, DeleteOutlined, PaperClipOutlined } from '@ant-design/icons';
import type { UploadFile, UploadProps } from 'antd/es/upload/interface';
import { fileApi } from '../../api/file';
import { FileAttachment } from '../../types/models';

const { Text } = Typography;

export interface FileUploadProps {
  /** 业务类型 */
  businessType?: string;
  /** 业务ID */
  businessId?: number;
  /** 允许的文件类型（扩展名数组，如 ['.jpg', '.png']） */
  accept?: string;
  /** 最大文件大小（MB） */
  maxSize?: number;
  /** 是否支持多文件 */
  multiple?: boolean;
  /** 最大文件数量 */
  maxCount?: number;
  /** 已上传的文件列表 */
  value?: FileAttachment[];
  /** 文件列表变化回调 */
  onChange?: (files: FileAttachment[]) => void;
  /** 按钮文本 */
  buttonText?: string;
}

export default function FileUpload({
  businessType,
  businessId,
  accept,
  maxSize = 50,
  multiple = false,
  maxCount = 10,
  value = [],
  onChange,
  buttonText = '上传文件',
}: FileUploadProps) {
  const [uploading, setUploading] = useState(false);

  const handleUpload = async (info: any) => {
    const file = info.file as File;

    // 检查文件大小
    if (file.size > maxSize * 1024 * 1024) {
      message.error(`文件大小不能超过 ${maxSize}MB`);
      return;
    }

    // 检查文件数量
    if (value.length >= maxCount) {
      message.error(`最多上传 ${maxCount} 个文件`);
      return;
    }

    setUploading(true);
    try {
      const res = await fileApi.upload(file, businessType, businessId);
      if (res.success && res.data) {
        const newFiles = [...value, res.data];
        onChange?.(newFiles);
        message.success('上传成功');
      } else {
        message.error(res.message || '上传失败');
      }
    } catch (error) {
      message.error('上传失败');
    } finally {
      setUploading(false);
    }
  };

  const handleRemove = (fileId: number) => {
    const newFiles = value.filter((f) => f.id !== fileId);
    onChange?.(newFiles);
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  return (
    <div>
      <Upload
        customRequest={handleUpload}
        showUploadList={false}
        accept={accept}
        multiple={multiple}
      >
        <Button icon={<UploadOutlined />} loading={uploading} disabled={value.length >= maxCount}>
          {buttonText}
        </Button>
      </Upload>

      {value.length > 0 && (
        <List
          size="small"
          dataSource={value}
          style={{ marginTop: 8 }}
          renderItem={(file) => (
            <List.Item
              actions={[
                <Button
                  key="delete"
                  type="text"
                  size="small"
                  danger
                  icon={<DeleteOutlined />}
                  onClick={() => handleRemove(file.id)}
                />,
              ]}
            >
              <Space>
                <PaperClipOutlined />
                <Text ellipsis style={{ maxWidth: 200 }}>
                  {file.fileName}
                </Text>
                <Text type="secondary">{formatFileSize(file.fileSize)}</Text>
              </Space>
            </List.Item>
          )}
        />
      )}
    </div>
  );
}
