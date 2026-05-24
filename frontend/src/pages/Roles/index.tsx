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
  InputNumber,
  Tree,
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
import type { DataNode } from 'antd/es/tree';
import { RoleDetail, CreateRoleInput, UpdateRoleInput, MenuTree } from '../../types/models';
import { roleApi, menuApi } from '../../api';
import './index.css';

export default function RolesPage() {
  const [roles, setRoles] = useState<RoleDetail[]>([]);
  const [menuTree, setMenuTree] = useState<MenuTree[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
    total: 0,
  });
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<RoleDetail | null>(null);
  const [checkedMenuKeys, setCheckedMenuKeys] = useState<number[]>([]);
  const [form] = Form.useForm();

  // 加载角色列表
  const loadRoles = async (pageIndex = 1, pageSize = 10, keyword?: string) => {
    setLoading(true);
    try {
      const res = await roleApi.getList({
        pageIndex,
        pageSize,
        keyword: keyword || undefined,
      });
      if (res.success && res.data) {
        setRoles(res.data.items);
        setPagination({
          current: res.data.pageIndex,
          pageSize: res.data.pageSize,
          total: res.data.total,
        });
      }
    } catch (error) {
      message.error('加载角色列表失败');
    } finally {
      setLoading(false);
    }
  };

  // 加载菜单树
  const loadMenuTree = async () => {
    try {
      const res = await menuApi.getTree();
      if (res.success && res.data) {
        setMenuTree(res.data);
      }
    } catch (error) {
      console.error('加载菜单树失败', error);
    }
  };

  useEffect(() => {
    loadRoles();
    loadMenuTree();
  }, []);

  // 将菜单树转换为 Ant Design Tree 数据格式
  const convertToTreeData = (menus: MenuTree[]): DataNode[] => {
    return menus.map((menu) => ({
      title: menu.menuName,
      key: menu.id,
      children: menu.children?.length ? convertToTreeData(menu.children) : undefined,
    }));
  };

  // 搜索
  const handleSearch = () => {
    loadRoles(1, pagination.pageSize, searchText);
  };

  // 打开新增/编辑弹窗
  const showModal = (role?: RoleDetail) => {
    setEditingRole(role || null);
    if (role) {
      form.setFieldsValue({
        roleCode: role.roleCode,
        roleName: role.roleName,
        sortOrder: role.sortOrder,
        status: role.status,
        remark: role.remark,
      });
      setCheckedMenuKeys(role.menuIds);
    } else {
      form.resetFields();
      setCheckedMenuKeys([]);
    }
    setIsModalOpen(true);
  };

  // 关闭弹窗
  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingRole(null);
    form.resetFields();
    setCheckedMenuKeys([]);
  };

  // 提交表单
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingRole) {
        // 更新角色
        const updateData: UpdateRoleInput = {
          id: editingRole.id,
          roleName: values.roleName,
          sortOrder: values.sortOrder,
          status: values.status,
          remark: values.remark,
          menuIds: checkedMenuKeys,
        };
        const res = await roleApi.update(updateData);
        if (res.success) {
          message.success('更新成功');
          loadRoles(pagination.current, pagination.pageSize, searchText);
          handleCancel();
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        // 新增角色
        const createData: CreateRoleInput = {
          roleCode: values.roleCode,
          roleName: values.roleName,
          sortOrder: values.sortOrder ?? 0,
          status: values.status ?? 1,
          remark: values.remark,
          menuIds: checkedMenuKeys,
        };
        const res = await roleApi.create(createData);
        if (res.success) {
          message.success('新增成功');
          loadRoles(pagination.current, pagination.pageSize, searchText);
          handleCancel();
        } else {
          message.error(res.message || '新增失败');
        }
      }
    } catch {
      // 表单验证失败
    }
  };

  // 删除角色
  const handleDelete = async (id: number) => {
    try {
      const res = await roleApi.delete(id);
      if (res.success) {
        message.success('删除成功');
        loadRoles(pagination.current, pagination.pageSize, searchText);
      } else {
        message.error(res.message || '删除失败');
      }
    } catch (error) {
      message.error('删除失败');
    }
  };

  // 表格列定义
  const columns: ColumnsType<RoleDetail> = [
    {
      title: '角色编码',
      dataIndex: 'roleCode',
      key: 'roleCode',
      width: 120,
    },
    {
      title: '角色名称',
      dataIndex: 'roleName',
      key: 'roleName',
      width: 150,
    },
    {
      title: '排序号',
      dataIndex: 'sortOrder',
      key: 'sortOrder',
      width: 80,
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
      title: '备注',
      dataIndex: 'remark',
      key: 'remark',
      width: 200,
      ellipsis: true,
    },
    {
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      width: 160,
      render: (time: string) => new Date(time).toLocaleString(),
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
            title="确定要删除此角色吗？"
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
    <div className="roles-page">
      <Card
        title="角色管理"
        extra={
          <Space>
            <Input
              placeholder="搜索角色编码/名称"
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
              新增角色
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={roles}
          rowKey="id"
          loading={loading}
          pagination={{
            ...pagination,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 条`,
            onChange: (page, pageSize) => {
              loadRoles(page, pageSize, searchText);
            },
          }}
        />
      </Card>

      {/* 新增/编辑角色弹窗 */}
      <Modal
        title={editingRole ? '编辑角色' : '新增角色'}
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={handleCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ status: 1, sortOrder: 0 }}
        >
          {!editingRole && (
            <Form.Item
              name="roleCode"
              label="角色编码"
              rules={[
                { required: true, message: '请输入角色编码' },
                { pattern: /^[a-zA-Z_]+$/, message: '只能包含英文字母和下划线' },
              ]}
            >
              <Input placeholder="请输入角色编码（英文）" />
            </Form.Item>
          )}

          <Form.Item
            name="roleName"
            label="角色名称"
            rules={[{ required: true, message: '请输入角色名称' }]}
          >
            <Input placeholder="请输入角色名称" />
          </Form.Item>

          <Form.Item name="sortOrder" label="排序号">
            <InputNumber min={0} max={9999} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="status" label="状态">
            <Select>
              <Select.Option value={1}>启用</Select.Option>
              <Select.Option value={0}>禁用</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="remark" label="备注">
            <Input.TextArea rows={2} placeholder="请输入备注" />
          </Form.Item>

          <Form.Item label="菜单权限">
            <div style={{ border: '1px solid #d9d9d9', borderRadius: 6, padding: 8, maxHeight: 300, overflow: 'auto' }}>
              <Tree
                checkable
                defaultExpandAll
                checkedKeys={checkedMenuKeys}
                onCheck={(checked) => {
                  setCheckedMenuKeys(checked as number[]);
                }}
                treeData={convertToTreeData(menuTree)}
              />
            </div>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
