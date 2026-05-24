import { Row, Col, Card, Statistic, Table, Tag, Typography } from 'antd';
import {
  ShoppingOutlined,
  InboxOutlined,
  ScanOutlined,
  TeamOutlined,
  ArrowUpOutlined,
  ArrowDownOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import './index.css';

const { Title } = Typography;

interface RecentRecord {
  key: string;
  barcode: string;
  productName: string;
  operationType: string;
  quantity: number;
  scanTime: string;
}

const recentRecords: RecentRecord[] = [
  {
    key: '1',
    barcode: '6901234567890',
    productName: '白色T恤 XL',
    operationType: 'In',
    quantity: 100,
    scanTime: '2024-01-15 14:30:00',
  },
  {
    key: '2',
    barcode: '6901234567891',
    productName: '黑色牛仔裤 L',
    operationType: 'Out',
    quantity: 50,
    scanTime: '2024-01-15 14:25:00',
  },
  {
    key: '3',
    barcode: '6901234567892',
    productName: '红色连衣裙 M',
    operationType: 'In',
    quantity: 80,
    scanTime: '2024-01-15 14:20:00',
  },
  {
    key: '4',
    barcode: '6901234567893',
    productName: '蓝色衬衫 S',
    operationType: 'Out',
    quantity: 30,
    scanTime: '2024-01-15 14:15:00',
  },
  {
    key: '5',
    barcode: '6901234567894',
    productName: '运动鞋 42码',
    operationType: 'In',
    quantity: 60,
    scanTime: '2024-01-15 14:10:00',
  },
];

const columns: ColumnsType<RecentRecord> = [
  {
    title: '条形码',
    dataIndex: 'barcode',
    key: 'barcode',
  },
  {
    title: '商品名称',
    dataIndex: 'productName',
    key: 'productName',
  },
  {
    title: '操作类型',
    dataIndex: 'operationType',
    key: 'operationType',
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
  },
  {
    title: '操作时间',
    dataIndex: 'scanTime',
    key: 'scanTime',
  },
];

export default function DashboardPage() {
  return (
    <div className="dashboard">
      <Title level={4}>首页仪表盘</Title>

      <Row gutter={[16, 16]} className="stat-cards">
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="商品总数"
              value={1128}
              prefix={<ShoppingOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="库存总量"
              value={25680}
              prefix={<InboxOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日入库"
              value={1580}
              prefix={<ArrowDownOutlined />}
              valueStyle={{ color: '#52c41a' }}
              suffix={
                <span style={{ fontSize: 14, color: '#52c41a' }}>
                  +12.5%
                </span>
              }
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="今日出库"
              value={980}
              prefix={<ArrowUpOutlined />}
              valueStyle={{ color: '#faad14' }}
              suffix={
                <span style={{ fontSize: 14, color: '#faad14' }}>
                  +8.2%
                </span>
              }
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 24 }}>
        <Col span={24}>
          <Card
            title={
              <span>
                <ScanOutlined /> 最近扫码记录
              </span>
            }
          >
            <Table
              columns={columns}
              dataSource={recentRecords}
              pagination={false}
              size="middle"
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[16, 16]} style={{ marginTop: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            className="quick-action-card"
            onClick={() => window.location.href = '/pda'}
          >
            <div className="quick-action">
              <ScanOutlined className="quick-action-icon" />
              <div>
                <div className="quick-action-title">PDA扫码</div>
                <div className="quick-action-desc">快速入库/出库操作</div>
              </div>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            className="quick-action-card"
            onClick={() => window.location.href = '/products'}
          >
            <div className="quick-action">
              <ShoppingOutlined className="quick-action-icon" />
              <div>
                <div className="quick-action-title">商品管理</div>
                <div className="quick-action-desc">查看和管理商品信息</div>
              </div>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            className="quick-action-card"
            onClick={() => window.location.href = '/inventory'}
          >
            <div className="quick-action">
              <InboxOutlined className="quick-action-icon" />
              <div>
                <div className="quick-action-title">库存查询</div>
                <div className="quick-action-desc">查看库存详情</div>
              </div>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            className="quick-action-card"
            onClick={() => window.location.href = '/suppliers'}
          >
            <div className="quick-action">
              <TeamOutlined className="quick-action-icon" />
              <div>
                <div className="quick-action-title">供应商管理</div>
                <div className="quick-action-desc">管理供应商信息</div>
              </div>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
}
