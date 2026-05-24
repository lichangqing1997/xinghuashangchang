import { useState } from 'react';
import {
  Table,
  Card,
  Button,
  Input,
  Space,
  Tag,
  Modal,
  Form,
  InputNumber,
  Select,
  message,
  Popconfirm,
} from 'antd';
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { Product } from '../../types/models';
import './index.css';

const { Option } = Select;

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
  {
    id: 3,
    productCode: 'SP003',
    productName: '红色连衣裙',
    barcode: '6901234567892',
    SKU: 'DRESS-R-M',
    category: '裙子',
    color: '红色',
    size: 'M',
    purchasePrice: 120,
    salePrice: 259,
    unit: '件',
    supplierId: 2,
    status: 1,
    createdAt: '2024-01-15T14:20:00',
  },
  {
    id: 4,
    productCode: 'SP004',
    productName: '运动鞋',
    barcode: '6901234567893',
    SKU: 'SHOE-42',
    category: '鞋子',
    color: '白色',
    size: '42',
    purchasePrice: 150,
    salePrice: 299,
    unit: '双',
    supplierId: 3,
    status: 1,
    createdAt: '2024-01-15T14:15:00',
  },
];

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>(mockProducts);
  const [searchText, setSearchText] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [form] = Form.useForm();

  const filteredProducts = products.filter(
    (p) =>
      p.productName.includes(searchText) ||
      p.barcode.includes(searchText) ||
      p.productCode.includes(searchText)
  );

  const showModal = (product?: Product) => {
    setEditingProduct(product || null);
    if (product) {
      form.setFieldsValue(product);
    } else {
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingProduct(null);
    form.resetFields();
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingProduct) {
        setProducts((prev) =>
          prev.map((p) =>
            p.id === editingProduct.id ? { ...p, ...values } : p
          )
        );
        message.success('更新成功');
      } else {
        const newProduct: Product = {
          ...values,
          id: Math.max(...products.map((p) => p.id)) + 1,
          createdAt: new Date().toISOString(),
        };
        setProducts((prev) => [...prev, newProduct]);
        message.success('新增成功');
      }
      handleCancel();
    } catch {
      // validation failed
    }
  };

  const handleDelete = (id: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
    message.success('删除成功');
  };

  const columns: ColumnsType<Product> = [
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
      title: '类别',
      dataIndex: 'category',
      key: 'category',
      width: 80,
      render: (val: string) => <Tag>{val}</Tag>,
    },
    {
      title: '颜色',
      dataIndex: 'color',
      key: 'color',
      width: 80,
    },
    {
      title: '尺码',
      dataIndex: 'size',
      key: 'size',
      width: 80,
    },
    {
      title: '进货价',
      dataIndex: 'purchasePrice',
      key: 'purchasePrice',
      width: 100,
      render: (price: number) => `¥${price.toFixed(2)}`,
    },
    {
      title: '销售价',
      dataIndex: 'salePrice',
      key: 'salePrice',
      width: 100,
      render: (price: number) => `¥${price.toFixed(2)}`,
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
      title: '操作',
      key: 'action',
      width: 150,
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => showModal(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定要删除吗？"
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
    <div className="products-page">
      <Card
        title="商品管理"
        extra={
          <Space>
            <Input
              placeholder="搜索商品名称/条形码/编码"
              prefix={<SearchOutlined />}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              style={{ width: 300 }}
            />
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => showModal()}
            >
              新增商品
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={filteredProducts}
          rowKey="id"
          pagination={{
            total: filteredProducts.length,
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
          }}
        />
      </Card>

      <Modal
        title={editingProduct ? '编辑商品' : '新增商品'}
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={handleCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: 1, unit: '件' }}
        >
          <Form.Item
            name="productCode"
            label="商品编码"
            rules={[{ required: true, message: '请输入商品编码' }]}
          >
            <Input placeholder="请输入商品编码" />
          </Form.Item>

          <Form.Item
            name="productName"
            label="商品名称"
            rules={[{ required: true, message: '请输入商品名称' }]}
          >
            <Input placeholder="请输入商品名称" />
          </Form.Item>

          <Form.Item
            name="barcode"
            label="条形码"
            rules={[{ required: true, message: '请输入条形码' }]}
          >
            <Input placeholder="请输入条形码" />
          </Form.Item>

          <Form.Item name="SKU" label="SKU">
            <Input placeholder="请输入SKU" />
          </Form.Item>

          <Form.Item
            name="category"
            label="类别"
            rules={[{ required: true, message: '请选择类别' }]}
          >
            <Select placeholder="请选择类别">
              <Option value="衣服">衣服</Option>
              <Option value="裤子">裤子</Option>
              <Option value="裙子">裙子</Option>
              <Option value="鞋子">鞋子</Option>
              <Option value="配饰">配饰</Option>
            </Select>
          </Form.Item>

          <Form.Item name="color" label="颜色">
            <Input placeholder="请输入颜色" />
          </Form.Item>

          <Form.Item name="size" label="尺码">
            <Input placeholder="请输入尺码" />
          </Form.Item>

          <Form.Item
            name="purchasePrice"
            label="进货价"
            rules={[{ required: true, message: '请输入进货价' }]}
          >
            <InputNumber
              min={0}
              precision={2}
              prefix="¥"
              style={{ width: '100%' }}
            />
          </Form.Item>

          <Form.Item
            name="salePrice"
            label="销售价"
            rules={[{ required: true, message: '请输入销售价' }]}
          >
            <InputNumber
              min={0}
              precision={2}
              prefix="¥"
              style={{ width: '100%' }}
            />
          </Form.Item>

          <Form.Item name="unit" label="单位">
            <Select>
              <Option value="件">件</Option>
              <Option value="双">双</Option>
              <Option value="个">个</Option>
              <Option value="套">套</Option>
            </Select>
          </Form.Item>

          <Form.Item name="status" label="状态">
            <Select>
              <Option value={1}>启用</Option>
              <Option value={0}>禁用</Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
