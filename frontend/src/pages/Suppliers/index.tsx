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
import { Supplier } from '../../types/models';
import './index.css';

const { Option } = Select;

const mockSuppliers: Supplier[] = [
  {
    id: 1,
    supplierCode: 'GYS001',
    supplierName: '优衣库供应商',
    contactPerson: '张三',
    contactPhone: '13800138001',
    address: '上海市浦东新区',
    remark: '主要供应T恤、衬衫',
    status: 1,
    createdAt: '2024-01-15T14:30:00',
  },
  {
    id: 2,
    supplierCode: 'GYS002',
    supplierName: '耐克供应商',
    contactPerson: '李四',
    contactPhone: '13800138002',
    address: '北京市朝阳区',
    remark: '运动服饰',
    status: 1,
    createdAt: '2024-01-15T14:25:00',
  },
  {
    id: 3,
    supplierCode: 'GYS003',
    supplierName: '阿迪达斯供应商',
    contactPerson: '王五',
    contactPhone: '13800138003',
    address: '广州市天河区',
    remark: '运动鞋服',
    status: 1,
    createdAt: '2024-01-15T14:20:00',
  },
];

export default function SuppliersPage() {
  const [suppliers, setSuppliers] = useState<Supplier[]>(mockSuppliers);
  const [searchText, setSearchText] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingSupplier, setEditingSupplier] = useState<Supplier | null>(null);
  const [form] = Form.useForm();

  const filteredSuppliers = suppliers.filter(
    (s) =>
      s.supplierName.includes(searchText) ||
      s.supplierCode.includes(searchText)
  );

  const showModal = (supplier?: Supplier) => {
    setEditingSupplier(supplier || null);
    if (supplier) {
      form.setFieldsValue(supplier);
    } else {
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingSupplier(null);
    form.resetFields();
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingSupplier) {
        setSuppliers((prev) =>
          prev.map((s) =>
            s.id === editingSupplier.id ? { ...s, ...values } : s
          )
        );
        message.success('更新成功');
      } else {
        const newSupplier: Supplier = {
          ...values,
          id: Math.max(...suppliers.map((s) => s.id)) + 1,
          createdAt: new Date().toISOString(),
        };
        setSuppliers((prev) => [...prev, newSupplier]);
        message.success('新增成功');
      }
      handleCancel();
    } catch {
      // validation failed
    }
  };

  const handleDelete = (id: number) => {
    setSuppliers((prev) => prev.filter((s) => s.id !== id));
    message.success('删除成功');
  };

  const columns: ColumnsType<Supplier> = [
    {
      title: '序号',
      key: 'index',
      width: 60,
      render: (_, __, index) => index + 1,
    },
    {
      title: '供应商编码',
      dataIndex: 'supplierCode',
      key: 'supplierCode',
      width: 120,
    },
    {
      title: '供应商名称',
      dataIndex: 'supplierName',
      key: 'supplierName',
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
      width: 150,
    },
    {
      title: '地址',
      dataIndex: 'address',
      key: 'address',
      width: 200,
      ellipsis: true,
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
    <div className="suppliers-page">
      <Card
        title="供应商管理"
        extra={
          <Space>
            <Input
              placeholder="搜索供应商名称/编码"
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
              新增供应商
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={filteredSuppliers}
          rowKey="id"
          pagination={{
            total: filteredSuppliers.length,
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
          }}
        />
      </Card>

      <Modal
        title={editingSupplier ? '编辑供应商' : '新增供应商'}
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={handleCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: 1 }}
        >
          <Form.Item
            name="supplierCode"
            label="供应商编码"
            rules={[{ required: true, message: '请输入供应商编码' }]}
          >
            <Input placeholder="请输入供应商编码" />
          </Form.Item>

          <Form.Item
            name="supplierName"
            label="供应商名称"
            rules={[{ required: true, message: '请输入供应商名称' }]}
          >
            <Input placeholder="请输入供应商名称" />
          </Form.Item>

          <Form.Item name="contactPerson" label="联系人">
            <Input placeholder="请输入联系人" />
          </Form.Item>

          <Form.Item name="contactPhone" label="联系电话">
            <Input placeholder="请输入联系电话" />
          </Form.Item>

          <Form.Item name="address" label="地址">
            <Input placeholder="请输入地址" />
          </Form.Item>

          <Form.Item name="remark" label="备注">
            <Input.TextArea rows={3} placeholder="请输入备注" />
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
