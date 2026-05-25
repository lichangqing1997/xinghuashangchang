import { useState, useEffect, useCallback } from 'react';
import { Card, Table, Button, Input, Space, Tag, Popconfirm, message, Select, DatePicker } from 'antd';
import { PlusOutlined, ReloadOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import dayjs from 'dayjs';
import { inboundOrderApi } from '../../api';
import type { InboundOrder } from '../../types/models';
import InboundOrderForm from './components/InboundOrderForm';
import InboundOrderDetail from './components/InboundOrderDetail';
import AuditModal from './components/AuditModal';
import './index.css';

const { RangePicker } = DatePicker;

const statusOptions = [
  { label: '全部', value: '' },
  { label: '未处理', value: '未处理' },
  { label: '正在处理', value: '正在处理' },
  { label: '已完成', value: '已完成' },
  { label: '手动关闭', value: '手动关闭' },
];

const auditStatusOptions = [
  { label: '全部', value: '' },
  { label: '待审核', value: '待审核' },
  { label: '已通过', value: '已通过' },
  { label: '已驳回', value: '已驳回' },
];

const statusColorMap: Record<string, string> = {
  '未处理': 'default',
  '正在处理': 'processing',
  '已完成': 'success',
  '手动关闭': 'error',
};

const auditStatusColorMap: Record<string, string> = {
  '待审核': 'warning',
  '已通过': 'success',
  '已驳回': 'error',
};

export default function InboundOrdersPage() {
  const [data, setData] = useState<InboundOrder[]>([]);
  const [loading, setLoading] = useState(false);
  const [keyword, setKeyword] = useState('');
  const [status, setStatus] = useState('');
  const [auditStatus, setAuditStatus] = useState('');
  const [dateRange, setDateRange] = useState<[dayjs.Dayjs | null, dayjs.Dayjs | null]>([null, null]);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 20, total: 0 });
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailOpen, setDetailOpen] = useState(false);
  const [detailId, setDetailId] = useState<number | null>(null);
  const [auditOpen, setAuditOpen] = useState(false);
  const [auditId, setAuditId] = useState<number | null>(null);

  const loadData = useCallback(async (page = 1, size = 20) => {
    setLoading(true);
    try {
      const res = await inboundOrderApi.getList({
        keyword: keyword || undefined,
        status: status || undefined,
        auditStatus: auditStatus || undefined,
        startDate: dateRange[0]?.toISOString(),
        endDate: dateRange[1]?.toISOString(),
        pageIndex: page,
        pageSize: size,
      });
      if (res.success && res.data) {
        setData(res.data.items);
        setPagination({ current: res.data.pageIndex, pageSize: res.data.pageSize, total: res.data.total });
      }
    } finally {
      setLoading(false);
    }
  }, [keyword, status, auditStatus, dateRange]);

  useEffect(() => { loadData(); }, []);

  const handleTableChange = (pag: TablePaginationConfig) => {
    loadData(pag.current, pag.pageSize);
  };

  const handleDelete = async (id: number) => {
    const res = await inboundOrderApi.delete(id);
    if (res.success) {
      message.success('删除成功');
      loadData(pagination.current, pagination.pageSize);
    } else {
      message.error(res.message || '删除失败');
    }
  };

  const handleChangeStatus = async (id: number, newStatus: string) => {
    const res = await inboundOrderApi.changeStatus(id, newStatus);
    if (res.success) {
      message.success('状态变更成功');
      loadData(pagination.current, pagination.pageSize);
    } else {
      message.error(res.message || '状态变更失败');
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

  const handleView = (id: number) => {
    setDetailId(id);
    setDetailOpen(true);
  };

  const handleAudit = (id: number) => {
    setAuditId(id);
    setAuditOpen(true);
  };

  const handleFormClose = (refresh?: boolean) => {
    setFormOpen(false);
    setEditingId(null);
    if (refresh) loadData(pagination.current, pagination.pageSize);
  };

  const handleAuditClose = (refresh?: boolean) => {
    setAuditOpen(false);
    setAuditId(null);
    if (refresh) loadData(pagination.current, pagination.pageSize);
  };

  const columns: ColumnsType<InboundOrder> = [
    { title: '序号', width: 60, render: (_, __, i) => (pagination.current - 1) * pagination.pageSize + i + 1 },
    { title: '入库单编号', dataIndex: 'orderNo', width: 160 },
    { title: '供应商名称', dataIndex: 'supplierName', width: 180, render: (v) => v || '-' },
    { title: '来源', dataIndex: 'source', width: 120, render: (v) => v || '-' },
    { title: '入库单类型', dataIndex: 'orderType', width: 120, render: (v) => v || '-' },
    {
      title: '状态', dataIndex: 'status', width: 100,
      render: (v) => <Tag color={statusColorMap[v] || 'default'}>{v}</Tag>,
    },
    {
      title: '审核状态', dataIndex: 'auditStatus', width: 100,
      render: (v) => <Tag color={auditStatusColorMap[v] || 'default'}>{v}</Tag>,
    },
    { title: '创建人', dataIndex: 'creator', width: 100, render: (v) => v || '-' },
    {
      title: '创建时间', dataIndex: 'createdAt', width: 170,
      render: (v) => v ? dayjs(v).format('YYYY-MM-DD HH:mm') : '-',
    },
    {
      title: '操作', width: 320, fixed: 'right',
      render: (_, record) => (
        <Space size="small">
          <Button type="link" size="small" onClick={() => handleView(record.id)}>查看</Button>
          {record.status === '未处理' && (
            <>
              <Button type="link" size="small" onClick={() => handleEdit(record.id)}>编辑</Button>
              <Popconfirm title="确认开始处理？" onConfirm={() => handleChangeStatus(record.id, '正在处理')}>
                <Button type="link" size="small" style={{ color: '#1890ff' }}>开始处理</Button>
              </Popconfirm>
              <Popconfirm title="确认删除此入库单？" onConfirm={() => handleDelete(record.id)}>
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </>
          )}
          {record.status === '正在处理' && (
            <>
              <Popconfirm title="确认完成入库？" onConfirm={() => handleChangeStatus(record.id, '已完成')}>
                <Button type="link" size="small" style={{ color: '#52c41a' }}>完成入库</Button>
              </Popconfirm>
              <Popconfirm title="确认手动关闭？" onConfirm={() => handleChangeStatus(record.id, '手动关闭')}>
                <Button type="link" size="small" danger>手动关闭</Button>
              </Popconfirm>
            </>
          )}
          {record.auditStatus === '待审核' && (
            <Button type="link" size="small" onClick={() => handleAudit(record.id)}>审核</Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div className="inbound-orders-page">
      <Card
        title="入库单管理"
        extra={
          <Space wrap>
            <Input
              placeholder="搜索单号/供应商/来源"
              prefix={<SearchOutlined />}
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              onPressEnter={() => loadData()}
              style={{ width: 200 }}
              allowClear
            />
            <Select
              value={status}
              onChange={(v) => { setStatus(v); }}
              options={statusOptions}
              style={{ width: 120 }}
              placeholder="入库单状态"
            />
            <Select
              value={auditStatus}
              onChange={(v) => { setAuditStatus(v); }}
              options={auditStatusOptions}
              style={{ width: 120 }}
              placeholder="审核状态"
            />
            <RangePicker
              value={dateRange as any}
              onChange={(dates) => setDateRange(dates as any)}
              placeholder={['开始日期', '结束日期']}
            />
            <Button icon={<ReloadOutlined />} onClick={() => loadData()}>刷新</Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>新增入库单</Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={data}
          rowKey="id"
          loading={loading}
          pagination={{ ...pagination, showSizeChanger: true, showTotal: (t) => `共 ${t} 条` }}
          onChange={handleTableChange}
          scroll={{ x: 1500 }}
        />
      </Card>

      {formOpen && (
        <InboundOrderForm
          open={formOpen}
          orderId={editingId}
          onClose={handleFormClose}
        />
      )}

      {detailOpen && detailId && (
        <InboundOrderDetail
          open={detailOpen}
          orderId={detailId}
          onClose={() => { setDetailOpen(false); setDetailId(null); }}
        />
      )}

      {auditOpen && auditId && (
        <AuditModal
          open={auditOpen}
          orderId={auditId}
          onClose={handleAuditClose}
        />
      )}
    </div>
  );
}
