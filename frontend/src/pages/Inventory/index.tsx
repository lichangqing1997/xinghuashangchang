import { useState } from 'react';
import { Table, Card, Input, Space, Tag, Button, message } from 'antd';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { Inventory, Product, Location } from '../../types/models';
import './index.css';

const mockProducts: Product[] = [
  {
    id: 1,
    productCode: 'SP001',
    productName: '白色T恤',
    barcode: '6901234567890',
    SKU: 'TSHIRT-W-XL',
    category: '衣服',
    color: '白色',
    size: 'XL',
    purchasePrice: 50,
    salePrice: 99,
    unit: '件',
    supplierId: 1,
    status: 1,
    createdAt: '2024-01-15T14:30:00',
  },
  {
    id: 2,
    productCode: 'SP002',
    productName: '黑色牛仔裤',
    barcode: '6901234567891',
    SKU: 'JEANS-B-L',
    category: '裤子',
    color: '黑色',
    size: 'L',
    purchasePrice: 80,
    salePrice: 169,
    unit: '件',
    supplierId: 1,
    status: 1,
    createdAt: '2024-01-15T14:25:00',
  },
];

const mockLocations: Location[] = [
  {
    id: 1,
    locationCode: 'A-01-01',
    locationName: 'A区1架1层',
    warehouse: '主仓库',
    shelf: 'A-01',
    floor: 1,
    position: 1,
    status: '占用',
    createdAt: '2024-01-15T14:30:00',
  },
  {
    id: 2,
    locationCode: 'A-01-02',
    locationName: 'A区1架2层',
    warehouse: '主仓库',
    shelf: 'A-01',
    floor: 2,
    position: 1,
    status: '占用',
    createdAt: '2024-01-15T14:25:00',
  },
  {
    id: 3,
    locationCode: 'B-01-01',
    locationName: 'B区1架1层',
    warehouse: '主仓库',
    shelf: 'B-01',
    floor: 1,
    position: 1,
    status: '空闲',
    createdAt: '2024-01-15T14:20:00',
  },
];

const mockInventory: Inventory[] = [
  {
    id: 1,
    productId: 1,
    locationId: 1,
    quantity: 500,
    lockedQuantity: 50,
    availableQuantity: 450,
    createdAt: '2024-01-15T14:30:00',
  },
  {
    id: 2,
    productId: 2,
    locationId: 2,
    quantity: 300,
    lockedQuantity: 0,
    availableQuantity: 300,
    createdAt: '2024-01-15T14:25:00',
  },
];

interface InventoryDisplay {
  key: number;
  productName: string;
  productCode: string;
  barcode: string;
  locationCode: string;
  locationName: string;
  quantity: number;
  lockedQuantity: number;
  availableQuantity: number;
  status: string;
}

export default function InventoryPage() {
  const [searchText, setSearchText] = useState('');

  const inventoryData: InventoryDisplay[] = mockInventory.map((inv) => {
    const product = mockProducts.find((p) => p.id === inv.productId);
    const location = mockLocations.find((l) => l.id === inv.locationId);
    return {
      key: inv.id,
      productName: product?.productName || '',
      productCode: product?.productCode || '',
      barcode: product?.barcode || '',
      locationCode: location?.locationCode || '',
      locationName: location?.locationName || '',
      quantity: inv.quantity,
      lockedQuantity: inv.lockedQuantity,
      availableQuantity: inv.availableQuantity,
      status: location?.status || '',
    };
  });

  const filteredData = inventoryData.filter(
    (item) =>
      item.productName.includes(searchText) ||
      item.productCode.includes(searchText) ||
      item.barcode.includes(searchText) ||
      item.locationCode.includes(searchText)
  );

  const handleRefresh = () => {
    message.success('数据已刷新');
  };

  const columns: ColumnsType<InventoryDisplay> = [
    {
      title: '序号',
      key: 'index',
      width: 60,
      render: (_, __, index) => index + 1,
    },
    {
      title: '商品编码',
      dataIndex: 'productCode',
      key: 'productCode',
      width: 100,
    },
    {
      title: '商品名称',
      dataIndex: 'productName',
      key: 'productName',
      width: 150,
    },
    {
      title: '条形码',
      dataIndex: 'barcode',
      key: 'barcode',
      width: 150,
    },
    {
      title: '库位编码',
      dataIndex: 'locationCode',
      key: 'locationCode',
      width: 120,
    },
    {
      title: '库位名称',
      dataIndex: 'locationName',
      key: 'locationName',
      width: 150,
    },
    {
      title: '总数量',
      dataIndex: 'quantity',
      key: 'quantity',
      width: 100,
      sorter: (a, b) => a.quantity - b.quantity,
    },
    {
      title: '锁定数量',
      dataIndex: 'lockedQuantity',
      key: 'lockedQuantity',
      width: 100,
      render: (val: number) => (
        <Tag color={val > 0 ? 'orange' : 'default'}>{val}</Tag>
      ),
    },
    {
      title: '可用数量',
      dataIndex: 'availableQuantity',
      key: 'availableQuantity',
      width: 100,
      render: (val: number) => (
        <Tag color={val > 0 ? 'green' : 'red'}>{val}</Tag>
      ),
    },
    {
      title: '库位状态',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (val: string) => (
        <Tag color={val === '空闲' ? 'green' : 'blue'}>{val}</Tag>
      ),
    },
  ];

  const totalQuantity = filteredData.reduce((sum, item) => sum + item.quantity, 0);
  const totalAvailable = filteredData.reduce(
    (sum, item) => sum + item.availableQuantity,
    0
  );
  const totalLocked = filteredData.reduce(
    (sum, item) => sum + item.lockedQuantity,
    0
  );

  return (
    <div className="inventory-page">
      <Card
        title="库存管理"
        extra={
          <Space>
            <Input
              placeholder="搜索商品名称/编码/条形码/库位"
              prefix={<SearchOutlined />}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              style={{ width: 300 }}
            />
            <Button icon={<ReloadOutlined />} onClick={handleRefresh}>
              刷新
            </Button>
          </Space>
        }
      >
        <div className="inventory-stats">
          <Space size="large">
            <div className="stat-item">
              <span className="stat-label">总库存：</span>
              <span className="stat-value">{totalQuantity}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">可用库存：</span>
              <span className="stat-value stat-available">{totalAvailable}</span>
            </div>
            <div className="stat-item">
              <span className="stat-label">锁定库存：</span>
              <span className="stat-value stat-locked">{totalLocked}</span>
            </div>
          </Space>
        </div>

        <Table
          columns={columns}
          dataSource={filteredData}
          pagination={{
            total: filteredData.length,
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
          }}
        />
      </Card>
    </div>
  );
}
