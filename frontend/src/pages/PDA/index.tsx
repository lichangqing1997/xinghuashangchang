import { useState } from 'react';
import {
  Card,
  Form,
  Input,
  InputNumber,
  Button,
  Tabs,
  Table,
  Tag,
  message,
  Row,
  Col,
  Statistic,
} from 'antd';
import {
  ScanOutlined,
  ImportOutlined,
  ExportOutlined,
  HistoryOutlined,
  CheckCircleOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { ScanRecord } from '../../types/models';
import './index.css';

const { TabPane } = Tabs;

const mockRecords: ScanRecord[] = [
  {
    id: 1,
    barcode: '6901234567890',
    productId: 1,
    operationType: 'In',
    locationId: 1,
    quantity: 100,
    referenceNo: 'JH20240115001',
    operator: 'admin',
    scanTime: '2024-01-15T14:30:00',
  },
  {
    id: 2,
    barcode: '6901234567891',
    productId: 2,
    operationType: 'Out',
    locationId: 2,
    quantity: 50,
    referenceNo: 'XS20240115001',
    operator: 'admin',
    scanTime: '2024-01-15T14:25:00',
  },
];

export default function PDAPage() {
  const [scanInForm] = Form.useForm();
  const [scanOutForm] = Form.useForm();
  const [records, setRecords] = useState<ScanRecord[]>(mockRecords);
  const [loading, setLoading] = useState(false);

  const handleScanIn = async () => {
    try {
      const values = await scanInForm.validateFields();
      setLoading(true);
      await new Promise((resolve) => setTimeout(resolve, 1000));

      const newRecord: ScanRecord = {
        id: records.length + 1,
        barcode: values.barcode,
        productId: 1,
        operationType: 'In',
        locationId: 1,
        quantity: values.quantity,
        referenceNo: values.purchaseOrderNo,
        operator: values.operator,
        scanTime: new Date().toISOString(),
      };

      setRecords((prev) => [newRecord, ...prev]);
      scanInForm.resetFields();
      message.success('入库成功');
    } catch {
      // validation failed
    } finally {
      setLoading(false);
    }
  };

  const handleScanOut = async () => {
    try {
      const values = await scanOutForm.validateFields();
      setLoading(true);
      await new Promise((resolve) => setTimeout(resolve, 1000));

      const newRecord: ScanRecord = {
        id: records.length + 1,
        barcode: values.barcode,
        productId: 2,
        operationType: 'Out',
        locationId: 2,
        quantity: values.quantity,
        referenceNo: values.salesOrderNo,
        operator: values.operator,
        scanTime: new Date().toISOString(),
      };

      setRecords((prev) => [newRecord, ...prev]);
      scanOutForm.resetFields();
      message.success('出库成功');
    } catch {
      // validation failed
    } finally {
      setLoading(false);
    }
  };

  const columns: ColumnsType<ScanRecord> = [
    {
      title: '条形码',
      dataIndex: 'barcode',
      key: 'barcode',
      width: 150,
    },
    {
      title: '操作类型',
      dataIndex: 'operationType',
      key: 'operationType',
      width: 100,
      render: (type: string) => (
        <Tag color={type === 'In' ? 'green' : 'blue'}>
          {type === 'In' ? '入库' : '出库'}
        </Tag>
      ),
    },
    {
      title: '数量',
      dataIndex: 'quantity',
      key: 'quantity',
      width: 80,
    },
    {
      title: '关联单号',
      dataIndex: 'referenceNo',
      key: 'referenceNo',
      width: 150,
    },
    {
      title: '操作人',
      dataIndex: 'operator',
      key: 'operator',
      width: 100,
    },
    {
      title: '操作时间',
      dataIndex: 'scanTime',
      key: 'scanTime',
      width: 180,
      render: (time: string) => new Date(time).toLocaleString(),
    },
  ];

  const todayInCount = records.filter((r) => r.operationType === 'In').length;
  const todayOutCount = records.filter((r) => r.operationType === 'Out').length;
  const todayInTotal = records
    .filter((r) => r.operationType === 'In')
    .reduce((sum, r) => sum + r.quantity, 0);
  const todayOutTotal = records
    .filter((r) => r.operationType === 'Out')
    .reduce((sum, r) => sum + r.quantity, 0);

  return (
    <div className="pda-page">
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日入库笔数"
              value={todayInCount}
              prefix={<ImportOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日入库数量"
              value={todayInTotal}
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日出库笔数"
              value={todayOutCount}
              prefix={<ExportOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日出库数量"
              value={todayOutTotal}
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
      </Row>

      <Card title="PDA扫码操作" style={{ marginBottom: 24 }}>
        <Tabs defaultActiveKey="scan-in">
          <TabPane
            tab={
              <span>
                <ImportOutlined /> 扫码入库
              </span>
            }
            key="scan-in"
          >
            <Form
              form={scanInForm}
              layout="vertical"
              style={{ maxWidth: 600 }}
            >
              <Form.Item
                name="barcode"
                label="条形码"
                rules={[{ required: true, message: '请输入或扫描条形码' }]}
              >
                <Input
                  placeholder="请输入或扫描条形码"
                  prefix={<ScanOutlined />}
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="locationCode"
                label="库位编码"
                rules={[{ required: true, message: '请输入库位编码' }]}
              >
                <Input placeholder="请输入库位编码" size="large" />
              </Form.Item>

              <Form.Item
                name="quantity"
                label="数量"
                rules={[{ required: true, message: '请输入数量' }]}
                initialValue={1}
              >
                <InputNumber min={1} style={{ width: '100%' }} size="large" />
              </Form.Item>

              <Form.Item name="purchaseOrderNo" label="关联进货单号">
                <Input placeholder="可选" size="large" />
              </Form.Item>

              <Form.Item name="operator" label="操作人">
                <Input placeholder="可选" size="large" />
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  size="large"
                  block
                  loading={loading}
                  onClick={handleScanIn}
                >
                  确认入库
                </Button>
              </Form.Item>
            </Form>
          </TabPane>

          <TabPane
            tab={
              <span>
                <ExportOutlined /> 扫码出库
              </span>
            }
            key="scan-out"
          >
            <Form
              form={scanOutForm}
              layout="vertical"
              style={{ maxWidth: 600 }}
            >
              <Form.Item
                name="barcode"
                label="条形码"
                rules={[{ required: true, message: '请输入或扫描条形码' }]}
              >
                <Input
                  placeholder="请输入或扫描条形码"
                  prefix={<ScanOutlined />}
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="quantity"
                label="数量"
                rules={[{ required: true, message: '请输入数量' }]}
                initialValue={1}
              >
                <InputNumber min={1} style={{ width: '100%' }} size="large" />
              </Form.Item>

              <Form.Item name="salesOrderNo" label="关联销售单号">
                <Input placeholder="可选" size="large" />
              </Form.Item>

              <Form.Item name="operator" label="操作人">
                <Input placeholder="可选" size="large" />
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  size="large"
                  block
                  loading={loading}
                  onClick={handleScanOut}
                >
                  确认出库
                </Button>
              </Form.Item>
            </Form>
          </TabPane>
        </Tabs>
      </Card>

      <Card
        title={
          <span>
            <HistoryOutlined /> 扫码记录
          </span>
        }
      >
        <Table
          columns={columns}
          dataSource={records}
          rowKey="id"
          pagination={{
            total: records.length,
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
          }}
        />
      </Card>
    </div>
  );
}
