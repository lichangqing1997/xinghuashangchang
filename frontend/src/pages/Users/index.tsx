import { useState, useEffect } from 'react';
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
  Switch,
} from 'antd';
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
  KeyOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { UserDetail, CreateUserInput, UpdateUserInput, Role } from '../../types/models';
import { userApi, roleApi } from '../../api';
import './index.css';

export default function UsersPage() {
  const [users, setUsers] = useState<UserDetail[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserDetail | null>(null);
  const [form] = Form.useForm();
  const [passwordForm] = Form.useForm();

  // 加载用户列表
  const loadUsers = async (pageIndex = 1, pageSize = 10, keyword?: string) => {
    setLoading(true);
    try {
      const res = await userApi.getList({
        pageIndex,
        pageSize,
        keyword: keyword || undefined,
      });
      if (res.success && res.data) {
        setUsers(res.data.items);
        setPagination({
          current: res.data.pageIndex,
          pageSize: res.data.pageSize,
          total: res.data.total,
        });
      }
    } catch (error) {
      message.error('加载用户列表失败');
    } finally {
      setLoading(false);
    }
  };

  // 加载角色列表
  const loadRoles = async () => {
    try {
      const res = await roleApi.getAll();
      if (res.success && res.data) {
        setRoles(res.data);
      }
    } catch (error) {
      console.error('加载角色列表失败', error);
    }
  };

  useEffect(() => {
    loadUsers();
    loadRoles();
  }, []);

  // 搜索
  const handleSearch = () => {
    loadUsers(1, pagination.pageSize, searchText);
  };

  // 打开新增/编辑弹窗
  const showModal = (user?: UserDetail) => {
    setEditingUser(user || null);
    if (user) {
      form.setFieldsValue({
        realName: user.realName,
        phone: user.phone,
        email: user.email,
        status: user.status,
        remark: user.remark,
        roleIds: user.roleIds,
      });
    } else {
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  // 关闭弹窗
  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingUser(null);
    form.resetFields();
  };

  // 提交表单
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingUser) {
        // 更新用户
        const updateData: UpdateUserInput = {
          id: editingUser.id,
          realName: values.realName,
          phone: values.phone,
          email: values.email,
          status: values.status,
          remark: values.remark,
          roleIds: values.roleIds || [],
        };
        const res = await userApi.update(updateData);
        if (res.success) {
          message.success('更新成功');
          loadUsers(pagination.current, pagination.pageSize, searchText);
          handleCancel();
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        // 新增用户
        const createData: CreateUserInput = {
          username: values.username,
          password: values.password,
          realName: values.realName,
          phone: values.phone,
          email: values.email,
          status: values.status ?? 1,
          remark: values.remark,
          roleIds: values.roleIds || [],
        };
        const res = await userApi.create(createData);
        if (res.success) {
          message.success('新增成功');
          loadUsers(pagination.current, pagination.pageSize, searchText);
          handleCancel();
        } else {
          message.error(res.message || '新增失败');
        }
      }
    } catch {
      // 表单验证失败
    }
  };

  // 删除用户
  const handleDelete = async (id: number) => {
    try {
      const res = await userApi.delete(id);
      if (res.success) {
        message.success('删除成功');
        loadUsers(pagination.current, pagination.pageSize, searchText);
      } else {
        message.error(res.message || '删除失败');
      }
    } catch (error) {
      message.error('删除失败');
    }
  };

  // 打开修改密码弹窗
  const showPasswordModal = (user: UserDetail) => {
    setEditingUser(user);
    passwordForm.resetFields();
    setIsPasswordModalOpen(true);
  };

  // 修改密码
  const handlePasswordSubmit = async () => {
    try {
      const values = await passwordForm.validateFields();
      if (editingUser) {
        const res = await userApi.changePassword({
          userId: editingUser.id,
          newPassword: values.newPassword,
        });
        if (res.success) {
          message.success('密码修改成功');
          setIsPasswordModalOpen(false);
          passwordForm.resetFields();
        } else {
          message.error(res.message || '密码修改失败');
        }
      }
    } catch {
      // 表单验证失败
    }
  };

  // 表格列定义
  const columns: ColumnsType<UserDetail> = [
    {
      title: '用户名',
      dataIndex: 'username',
      key: 'username',
      width: 120,
    },
    {
      title: '真实姓名',
      dataIndex: 'realName',
      key: 'realName',
      width: 100,
    },
    {
      title: '手机号',
      dataIndex: 'phone',
      key: 'phone',
      width: 120,
    },
    {
      title: '邮箱',
      dataIndex: 'email',
      key: 'email',
      width: 180,
    },
    {
      title: '角色',
      dataIndex: 'roles',
      key: 'roles',
      width: 200,
      render: (_, record) => (
        <Space>
          {record.roles.map((role) => (
            <Tag key={role.id} color="blue">
              {role.roleName}
            </Tag>
          ))}
        </Space>
      ),
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      width: 80,
      render: (status: number) => (
        <Tag color={status === 1 ? 'green' : 'red'}>
          {status === 1 ? '启用' : '禁用'}
        </Tag>
      ),
    },
    {
      title: '最后登录',
      dataIndex: 'lastLoginAt',
      key: 'lastLoginAt',
      width: 160,
      render: (time: string) => (time ? new Date(time).toLocaleString() : '-'),
    },
    {
      title: '操作',
      key: 'action',
      width: 200,
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => showModal(record)}
          >
            编辑
          </Button>
          <Button
            type="link"
            icon={<KeyOutlined />}
            onClick={() => showPasswordModal(record)}
          >
            密码
          </Button>
          {record.username !== 'admin' && (
            <Popconfirm
              title="确定要删除此用户吗？"
              onConfirm={() => handleDelete(record.id)}
            >
              <Button type="link" danger icon={<DeleteOutlined />}>
                删除
              </Button>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div className="users-page">
      <Card
        title="用户管理"
        extra={
          <Space>
            <Input
              placeholder="搜索用户名/姓名/手机号"
              prefix={<SearchOutlined />}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              onPressEnter={handleSearch}
              style={{ width: 250 }}
            />
            <Button type="primary" icon={<SearchOutlined />} onClick={handleSearch}>
              搜索
            </Button>
            <Button
              type="primary"
              icon={<PlusOutlined />}
              onClick={() => showModal()}
            >
              新增用户
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={users}
          rowKey="id"
          loading={loading}
          pagination={{
            ...pagination,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
            onChange: (page, pageSize) => {
              loadUsers(page, pageSize, searchText);
            },
          }}
        />
      </Card>

      {/* 新增/编辑用户弹窗 */}
      <Modal
        title={editingUser ? '编辑用户' : '新增用户'}
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={handleCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: 1, roleIds: [] }}
        >
          {!editingUser && (
            <>
              <Form.Item
                name="username"
                label="用户名"
                rules={[
                  { required: true, message: '请输入用户名' },
                  { min: 3, message: '用户名至少3个字符' },
                ]}
              >
                <Input placeholder="请输入用户名" />
              </Form.Item>

              <Form.Item
                name="password"
                label="密码"
                rules={[
                  { required: true, message: '请输入密码' },
                  { min: 6, message: '密码至少6个字符' },
                ]}
              >
                <Input.Password placeholder="请输入密码" />
              </Form.Item>
            </>
          )}

          <Form.Item name="realName" label="真实姓名">
            <Input placeholder="请输入真实姓名" />
          </Form.Item>

          <Form.Item
            name="phone"
            label="手机号"
            rules={[{ pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号' }]}
          >
            <Input placeholder="请输入手机号" />
          </Form.Item>

          <Form.Item
            name="email"
            label="邮箱"
            rules={[{ type: 'email', message: '请输入正确的邮箱' }]}
          >
            <Input placeholder="请输入邮箱" />
          </Form.Item>

          <Form.Item
            name="roleIds"
            label="角色"
            rules={[{ required: true, message: '请选择角色' }]}
          >
            <Select
              mode="multiple"
              placeholder="请选择角色"
              optionFilterProp="children"
            >
              {roles.map((role) => (
                <Select.Option key={role.id} value={role.id}>
                  {role.roleName}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item name="status" label="状态">
            <Select>
              <Select.Option value={1}>启用</Select.Option>
              <Select.Option value={0}>禁用</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="remark" label="备注">
            <Input.TextArea rows={3} placeholder="请输入备注" />
          </Form.Item>
        </Form>
      </Modal>

      {/* 修改密码弹窗 */}
      <Modal
        title="修改密码"
        open={isPasswordModalOpen}
        onOk={handlePasswordSubmit}
        onCancel={() => {
          setIsPasswordModalOpen(false);
          passwordForm.resetFields();
        }}
        width={400}
      >
        <Form form={passwordForm} layout="vertical">
          <Form.Item label="当前用户">
            <Input value={editingUser?.username} disabled />
          </Form.Item>
          <Form.Item
            name="newPassword"
            label="新密码"
            rules={[
              { required: true, message: '请输入新密码' },
              { min: 6, message: '密码至少6个字符' },
            ]}
          >
            <Input.Password placeholder="请输入新密码" />
          </Form.Item>
          <Form.Item
            name="confirmPassword"
            label="确认密码"
            dependencies={['newPassword']}
            rules={[
              { required: true, message: '请确认密码' },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('newPassword') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('两次密码不一致'));
                },
              }),
            ]}
          >
            <Input.Password placeholder="请再次输入新密码" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
