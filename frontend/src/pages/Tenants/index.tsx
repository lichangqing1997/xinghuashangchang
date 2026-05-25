import { useState, useEffect, useCallback } from 'react';
import { Card, Table, Button, Input, Space, Tag, Popconfirm, message } from 'antd';
import { PlusOutlined, ReloadOutlined, SearchOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import { tenantApi } from '../../api';
import type { Tenant } from '../../types/models';
import TenantForm from './components/TenantForm';
import './index.css';

export default function TenantsPage() {
  const [data, setData] = useState<Tenant[]>([]);
  const [loading, setLoading] = useState(false);
  const [keyword, setKeyword] = useState('');
  const [pagination, setPagination] = useState({ current: 1, pageSize: 20, total: 0 });
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  const loadData = useCallback(async (page = 1, size = 20) => {
    setLoading(true);
    try {
      const res = await tenantApi.getList(keyword || undefined, page, size);
      if (res.success && res.data) {
        setData(res.data.items);
        setPagination({ current: res.data.pageIndex, pageSize: res.data.pageSize, total: res.data.total });
      }
    } finally {
      setLoading(false);
    }
  }, [keyword]);

  useEffect(() => { loadData(); }, []);

  const handleTableChange = (pag: TablePaginationConfig) => {
    loadData(pag.current, pag.pageSize);
  };

  const handleDelete = async (id: number) => {
    const res = await tenantApi.delete(id);
    if (res.success) {
      message.success('删除成功');
      loadData(pagination.current, pagination.pageSize);
    } else {
      message.error(res.message || '删除失败');
    }
  };

  const handleCreate = () => {
    setEditingId(null);
    setFormOpen(true);
  };

  const handleEdit = (id: number) => {
    setEditingId(id);
    setFormOpen(true);
  };

  const handleFormClose = (refresh?: boolean) => {
    setFormOpen(false);
    setEditingId(null);
    if (refresh) {
      loadData(pagination.current, pagination.pageSize);
    }
  };

  const columns: ColumnsType<Tenant> = [
    {
      title: '序号',
      key: 'index',
      width: 60,
      render: (_, __, index) => (pagination.current - 1) * pagination.pageSize + index + 1,
    },
    {
      title: '租户编码',
      dataIndex: 'tenantCode',
      key: 'tenantCode',
      width: 120,
    },
    {
      title: '租户名称',
      dataIndex: 'tenantName',
      key: 'tenantName',
      width: 200,
    },
    {
      title: '联系人',
      dataIndex: 'contactPerson',
      key: 'contactPerson',
      width: 100,
    },
    {
      title: '联系电话',
      dataIndex: 'contactPhone',
      key: 'contactPhone',
      width: 130,
    },
    {
      title: '最大用户数',
      dataIndex: 'maxUsers',
      key: 'maxUsers',
      width: 100,
    },
    {
      title: '过期时间',
      dataIndex: 'expireAt',
      key: 'expireAt',
      width: 170,
      render: (val: string) => val ? new Date(val).toLocaleString('zh-CN') : '永不过期',
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (val: number) => (
        <Tag color={val === 1 ? 'green' : 'red'}>
          {val === 1 ? '启用' : '禁用'}
        </Tag>
      ),
    },
    {
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 170,
      render: (val: string) => val ? new Date(val).toLocaleString('zh-CN') : '-',
    },
    {
      title: '操作',
      key: 'action',
      width: 150,
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record.id)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定要删除该租户吗？"
            onConfirm={() => handleDelete(record.id)}
          >
            <Button type="link" danger icon={<DeleteOutlined />}>
              删除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div className="tenants-page">
      <Card
        title="租户管理"
        extra={
          <Space>
            <Input
              placeholder="搜索租户名称/编码"
              prefix={<SearchOutlined />}
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              onPressEnter={() => loadData()}
              style={{ width: 300 }}
            />
            <Button
              icon={<ReloadOutlined />}
              onClick={() => loadData()}
            >
              刷新
            </Button>
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={handleCreate}
            >
              新增租户
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={data}
          rowKey="id"
          loading={loading}
          pagination={{
            ...pagination,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
          }}
          onChange={handleTableChange}
          scroll={{ x: 1400 }}
        />
      </Card>

      {formOpen && (
        <TenantForm
          id={editingId}
          onClose={handleFormClose}
        />
      )}
    </div>
  );
}
