import { useState, useEffect } from 'react';
import {
  Table,
  Card,
  Button,
  Space,
  Tag,
  Upload,
  message,
  Popconfirm,
  Select,
  Modal,
} from 'antd';
import {
  UploadOutlined,
  DeleteOutlined,
  DownloadOutlined,
  FileImageOutlined,
  FileOutlined,
  MobileOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { FileAttachment } from '../../types/models';
import { fileApi, FileListParams } from '../../api/file';
import { PageResult } from '../../types/api';

const fileTypeLabels: Record<string, { color: string; label: string }> = {
  image: { color: 'blue', label: '图片' },
  document: { color: 'green', label: '文档' },
  apk: { color: 'orange', label: 'APK' },
  other: { color: 'default', label: '其他' },
};

export default function FilesPage() {
  const [files, setFiles] = useState<FileAttachment[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [pageIndex, setPageIndex] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [fileTypeFilter, setFileTypeFilter] = useState<string | undefined>();

  const fetchFiles = async (page = pageIndex, size = pageSize, fileType?: string) => {
    setLoading(true);
    try {
      const params: FileListParams = {
        pageIndex: page,
        pageSize: size,
        fileType,
      };
      const res = await fileApi.getList(params);
      if (res.success && res.data) {
        const pageResult = res.data as unknown as PageResult<FileAttachment>;
        setFiles(pageResult.items || []);
        setTotal(pageResult.total || 0);
      }
    } catch (error) {
      message.error('获取文件列表失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFiles();
  }, []);

  const handleUpload = async (info: any) => {
    if (info.file.status === 'uploading') {
      return;
    }
    if (info.file.status === 'done') {
      message.success('上传成功');
      fetchFiles();
    } else if (info.file.status === 'error') {
      message.error('上传失败');
    }
  };

  const handleDelete = async (id: number) => {
    try {
      const res = await fileApi.delete(id);
      if (res.success) {
        message.success('删除成功');
        fetchFiles();
      } else {
        message.error(res.message || '删除失败');
      }
    } catch (error) {
      message.error('删除失败');
    }
  };

  const handleDownload = (id: number) => {
    fileApi.download(id);
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  const columns: ColumnsType<FileAttachment> = [
    {
      title: '文件名',
      dataIndex: 'fileName',
      key: 'fileName',
      ellipsis: true,
      width: 250,
    },
    {
      title: '文件类型',
      dataIndex: 'fileType',
      key: 'fileType',
      width: 100,
      render: (type: string) => {
        const config = fileTypeLabels[type] || fileTypeLabels.other;
        return <Tag color={config.color}>{config.label}</Tag>;
      },
    },
    {
      title: '大小',
      dataIndex: 'fileSize',
      key: 'fileSize',
      width: 100,
      render: (size: number) => formatFileSize(size),
    },
    {
      title: '版本',
      key: 'version',
      width: 120,
      render: (_, record) => {
        if (record.fileType === 'apk' && record.version) {
          return (
            <Space>
              <Tag color="purple">v{record.version}</Tag>
              {record.versionCode && <span>({record.versionCode})</span>}
            </Space>
          );
        }
        return '-';
      },
    },
    {
      title: '业务类型',
      dataIndex: 'businessType',
      key: 'businessType',
      width: 100,
      render: (type?: string) => type || '-',
    },
    {
      title: '上传时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 180,
      render: (time: string) => new Date(time).toLocaleString('zh-CN'),
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      fixed: 'right',
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            size="small"
            icon={<DownloadOutlined />}
            onClick={() => handleDownload(record.id)}
          >
            下载
          </Button>
          <Popconfirm
            title="确定删除此文件？"
            onConfirm={() => handleDelete(record.id)}
            okText="确定"
            cancelText="取消"
          >
            <Button type="link" size="small" danger icon={<DeleteOutlined />}>
              删除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const uploadProps = {
    name: 'file',
    action: '/api/File/upload',
    headers: {
      Authorization: `Bearer ${localStorage.getItem('token')}`,
    },
    showUploadList: false,
    onChange: handleUpload,
  };

  return (
    <div style={{ padding: '24px' }}>
      <Card
        title="文件管理"
        extra={
          <Space>
            <Select
              placeholder="文件类型"
              allowClear
              style={{ width: 120 }}
              value={fileTypeFilter}
              onChange={(value) => {
                setFileTypeFilter(value);
                fetchFiles(1, pageSize, value);
              }}
              options={[
                { value: 'image', label: '图片' },
                { value: 'document', label: '文档' },
                { value: 'apk', label: 'APK' },
                { value: 'other', label: '其他' },
              ]}
            />
            <Upload {...uploadProps}>
              <Button type="primary" icon={<UploadOutlined />}>
                上传文件
              </Button>
            </Upload>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={files}
          rowKey="id"
          loading={loading}
          pagination={{
            current: pageIndex,
            pageSize: pageSize,
            total: total,
            showSizeChanger: true,
            showQuickJumper: true,
            showTotal: (total) => `共 ${total} 条`,
            onChange: (page, size) => {
              setPageIndex(page);
              setPageSize(size);
              fetchFiles(page, size, fileTypeFilter);
            },
          }}
          scroll={{ x: 1000 }}
        />
      </Card>
    </div>
  );
}
