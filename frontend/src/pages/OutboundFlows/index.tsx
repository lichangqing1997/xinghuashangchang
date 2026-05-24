import { useState, useEffect, useCallback } from 'react';
import { Card, Table, Button, Input, Space, Statistic, Row, Col, message } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ColumnsType, TablePaginationConfig } from 'antd/es/table';
import dayjs from 'dayjs';
import { outboundOrderApi } from '../../api';
import type { OutboundFlow } from '../../types/models';
import './index.css';

export default function OutboundFlowsPage() {
  const [data, setData] = useState<OutboundFlow[]>([]);
  const [loading, setLoading] = useState(false);
  const [orderNo, setOrderNo] = useState('');
  const [keyword, setKeyword] = useState('');
  const [pagination, setPagination] = useState({ current: 1, pageSize: 20, total: 0 });
  const [totalQuantity, setTotalQuantity] = useState(0);
  const [totalAmount, setTotalAmount] = useState(0);

  const loadData = useCallback(async (page = 1, size = 20) => {
    setLoading(true);
    try {
      const res = await outboundOrderApi.getFlows({
        orderNo: orderNo || undefined,
        keyword: keyword || undefined,
        pageIndex: page,
        pageSize: size,
      });
      if (res.success && res.data) {
        setData(res.data.items);
        setPagination({ current: res.data.pageIndex, pageSize: res.data.pageSize, total: res.data.total });
        setTotalQuantity(res.data.items.reduce((s, i) => s + i.quantity, 0));
        setTotalAmount(res.data.items.reduce((s, i) => s + i.amount, 0));
      }
    } finally {
      setLoading(false);
    }
  }, [orderNo, keyword]);

  useEffect(() => { loadData(); }, []);

  const handleTableChange = (pag: TablePaginationConfig) => {
    loadData(pag.current, pag.pageSize);
  };

  const columns: ColumnsType<OutboundFlow> = [
    { title: '序号', width: 60, render: (_, __, i) => (pagination.current - 1) * pagination.pageSize + i + 1 },
    { title: '出库单号', dataIndex: 'orderNo', width: 150 },
    { title: '商品编码', dataIndex: 'productCode', width: 100, render: (v) => v || '-' },
    { title: '商品名', dataIndex: 'productName', width: 140, render: (v) => v || '-' },
    { title: '库位', dataIndex: 'locationCode', width: 100, render: (v) => v || '-' },
    { title: '数量', dataIndex: 'quantity', width: 80 },
    { title: '单价', dataIndex: 'unitPrice', width: 100, render: (v) => `¥${v.toFixed(2)}` },
    { title: '金额', dataIndex: 'amount', width: 100, render: (v) => `¥${v.toFixed(2)}` },
    { title: '批号', dataIndex: 'batchNo', width: 100, render: (v) => v || '-' },
    { title: '操作人', dataIndex: 'operator', width: 100, render: (v) => v || '-' },
    {
      title: '出库时间', dataIndex: 'flowTime', width: 170,
      render: (v) => v ? dayjs(v).format('YYYY-MM-DD HH:mm') : '-',
    },
  ];

  return (
    <div className="outbound-flows-page">
      <Row gutter={16} style={{ marginBottom: 16 }}>
        <Col span={12}>
          <Card>
            <Statistic title="本页出库数量" value={totalQuantity} suffix="件" />
          </Card>
        </Col>
        <Col span={12}>
          <Card>
            <Statistic title="本页出库金额" value={totalAmount} precision={2} prefix="¥" />
          </Card>
        </Col>
      </Row>

      <Card
        title="出库流水"
        extra={
          <Space wrap>
            <Input
              placeholder="出库单号"
              value={orderNo}
              onChange={(e) => setOrderNo(e.target.value)}
              onPressEnter={() => loadData()}
              style={{ width: 160 }}
              allowClear
            />
            <Input
              placeholder="商品编码/名称"
              prefix={<SearchOutlined />}
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
              onPressEnter={() => loadData()}
              style={{ width: 180 }}
              allowClear
            />
            <Button icon={<ReloadOutlined />} onClick={() => loadData()}>刷新</Button>
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
    </div>
  );
}
