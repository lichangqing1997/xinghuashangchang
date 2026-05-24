import { useState, useEffect, useCallback } from 'react';
import { Card, Table, Button, Input, Space, Tag, Popconfirm, message, Select, DatePicker } from 'antd';
import { PlusOutlined, ReloadOutlined, SearchOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import dayjs from 'dayjs';
import { outboundOrderApi } from '../../api';
import type { OutboundOrder } from '../../types/models';
import OutboundOrderForm from './components/OutboundOrderForm';
import OutboundOrderDetail from './components/OutboundOrderDetail';
import './index.css';

const { RangePicker } = DatePicker;

const statusOptions = [
  { label: '全部', value: '' },
  { label: '待出库', value: '待出库' },
  { label: '已出库', value: '已出库' },
  { label: '已取消', value: '已取消' },
];

const statusColorMap: Record<string, string> = {
  '待出库': 'blue',
  '已出库': 'green',
  '已取消': 'red',
};

export default function OutboundOrdersPage() {
  const [data, setData] = useState<OutboundOrder[]>([]);
  const [loading, setLoading] = useState(false);
  const [keyword, setKeyword] = useState('');
  const [status, setStatus] = useState('');
  const [dateRange, setDateRange] = useState<[dayjs.Dayjs | null, dayjs.Dayjs | null]>([null, null]);
  const [pagination, setPagination] = useState({ current: 1, pageSize: 20, total: 0 });
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailOpen, setDetailOpen] = useState(false);
  const [detailId, setDetailId] = useState<number | null>(null);

  const loadData = useCallback(async (page = 1, size = 20) => {
    setLoading(true);
    try {
      const res = await outboundOrderApi.getList({
        keyword: keyword || undefined,
        status: status || undefined,
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
  }, [keyword, status, dateRange]);

  useEffect(() => { loadData(); }, []);

  const handleTableChange = (pag: TablePaginationConfig) => {
    loadData(pag.current, pag.pageSize);
  };

  const handleDelete = async (id: number) => {
    const res = await outboundOrderApi.delete(id);
    if (res.success) {
      message.success('取消成功');
      loadData(pagination.current, pagination.pageSize);
    } else {
      message.error(res.message || '取消失败');
    }
  };

  const handleConfirm = async (id: number) => {
    const res = await outboundOrderApi.confirmOutbound(id);
    if (res.success) {
      message.success('出库确认成功');
      loadData(pagination.current, pagination.pageSize);
    } else {
      message.error(res.message || '出库确认失败');
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

  const handleFormClose = (refresh?: boolean) => {
    setFormOpen(false);
    setEditingId(null);
    if (refresh) loadData(pagination.current, pagination.pageSize);
  };

  const columns: ColumnsType<OutboundOrder> = [
    { title: '序号', width: 60, render: (_, __, i) => (pagination.current - 1) * pagination.pageSize + i + 1 },
    { title: '出库单号', dataIndex: 'orderNo', width: 160 },
    { title: '收货公司', dataIndex: 'companyName', width: 180, render: (v) => v || '-' },
    { title: '总金额', dataIndex: 'totalAmount', width: 120, render: (v) => `¥${v.toFixed(2)}` },
    {
      title: '状态', dataIndex: 'status', width: 100,
      render: (v) => <Tag color={statusColorMap[v] || 'default'}>{v}</Tag>,
    },
    { title: '制单人', dataIndex: 'operator', width: 100, render: (v) => v || '-' },
    {
      title: '创建时间', dataIndex: 'createdAt', width: 170,
      render: (v) => v ? dayjs(v).format('YYYY-MM-DD HH:mm') : '-',
    },
    {
      title: '操作', width: 260, fixed: 'right',
      render: (_, record) => (
        <Space size="small">
          <Button type="link" size="small" onClick={() => handleView(record.id)}>查看</Button>
          {record.status === '待出库' && (
            <>
              <Button type="link" size="small" onClick={() => handleEdit(record.id)}>编辑</Button>
              <Popconfirm title="确认出库？" onConfirm={() => handleConfirm(record.id)}>
                <Button type="link" size="small" style={{ color: '#52c41a' }}>出库确认</Button>
              </Popconfirm>
              <Popconfirm title="确认取消此出库单？" onConfirm={() => handleDelete(record.id)}>
                <Button type="link" size="small" danger>取消</Button>
              </Popconfirm>
            </>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div className="outbound-orders-page">
      <Card
        title="出库单管理"
        extra={
          <Space wrap>
            <Input
              placeholder="搜索单号/公司名"
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
            />
            <RangePicker
              value={dateRange as any}
              onChange={(dates) => setDateRange(dates as any)}
              placeholder={['开始日期', '结束日期']}
            />
            <Button icon={<ReloadOutlined />} onClick={() => loadData()}>刷新</Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>新增出库单</Button>
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
          scroll={{ x: 1300 }}
        />
      </Card>

      {formOpen && (
        <OutboundOrderForm
          open={formOpen}
          orderId={editingId}
          onClose={handleFormClose}
        />
      )}

      {detailOpen && detailId && (
        <OutboundOrderDetail
          open={detailOpen}
          orderId={detailId}
          onClose={() => { setDetailOpen(false); setDetailId(null); }}
        />
      )}
    </div>
  );
}
