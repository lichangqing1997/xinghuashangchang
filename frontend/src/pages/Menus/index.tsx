import { useState, useEffect, useMemo, useCallback } from 'react';
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
  TreeSelect,
  message,
  Popconfirm,
  Tooltip,
} from 'antd';
import {
  PlusOutlined,
  SearchOutlined,
  EditOutlined,
  DeleteOutlined,
  DownOutlined,
  RightOutlined,
  FolderOutlined,
  MenuOutlined as MenuIconOutlined,
  AppstoreOutlined,
} from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { Menu, MenuTree, CreateMenuInput, UpdateMenuInput } from '../../types/models';
import { menuApi } from '../../api';
import './index.css';

// 菜单类型映射
const menuTypeMap: Record<number, { label: string; color: string }> = {
  0: { label: '目录', color: 'blue' },
  1: { label: '菜单', color: 'green' },
  2: { label: '按钮', color: 'orange' },
};

// 扁平数据转树形结构
function buildTree(list: Menu[], parentId: number = 0): Menu[] {
  return list
    .filter((item) => item.parentId === parentId)
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map((item) => {
      const children = buildTree(list, item.id);
      return children.length > 0 ? { ...item, children } : { ...item };
    });
}

// 收集所有有子节点的 key
function collectParentKeys(list: Menu[]): number[] {
  const keys: number[] = [];
  list.forEach((item) => {
    if (list.some((child) => child.parentId === item.id)) {
      keys.push(item.id);
    }
  });
  return keys;
}

export default function MenusPage() {
  const [menus, setMenus] = useState<Menu[]>([]);
  const [menuTree, setMenuTree] = useState<MenuTree[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingMenu, setEditingMenu] = useState<Menu | null>(null);
  const [form] = Form.useForm();
  const [menuType, setMenuType] = useState(1);
  const [expandedRowKeys, setExpandedRowKeys] = useState<React.Key[]>([]);
  const [filterType, setFilterType] = useState<number | null>(null);

  // 加载菜单列表
  const loadMenus = async (keyword?: string) => {
    setLoading(true);
    try {
      const res = await menuApi.getList({
        keyword: keyword || undefined,
      });
      if (res.success && res.data) {
        setMenus(res.data);
      }
    } catch (error) {
      message.error('加载菜单列表失败');
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
    loadMenus();
    loadMenuTree();
  }, []);

  // 将扁平数据转为树形数据（带 children）
  const treeData = useMemo(() => buildTree(menus), [menus]);

  // 按类型筛选后的树形数据
  const filteredTreeData = useMemo(() => {
    if (filterType === null) return treeData;
    // 递归筛选：保留匹配类型的节点及其祖先
    const filterTree = (nodes: Menu[]): Menu[] => {
      return nodes
        .map((node) => {
          const filteredChildren = node.children ? filterTree(node.children as Menu[]) : [];
          if (node.menuType === filterType || filteredChildren.length > 0) {
            return { ...node, children: filteredChildren.length > 0 ? filteredChildren : undefined };
          }
          return null;
        })
        .filter(Boolean) as Menu[];
    };
    return filterTree(treeData);
  }, [treeData, filterType]);

  // 数据变化时自动展开所有有子节点的行
  useEffect(() => {
    if (menus.length > 0) {
      setExpandedRowKeys(collectParentKeys(menus));
    } else {
      setExpandedRowKeys([]);
    }
  }, [menus]);

  // 展开全部
  const handleExpandAll = useCallback(() => {
    setExpandedRowKeys(collectParentKeys(menus));
  }, [menus]);

  // 折叠全部
  const handleCollapseAll = useCallback(() => {
    setExpandedRowKeys([]);
  }, []);

  // 处理行展开/折叠
  const handleExpand = (expanded: boolean, record: Menu) => {
    setExpandedRowKeys((prev) =>
      expanded ? [...prev, record.id] : prev.filter((key) => key !== record.id)
    );
  };

  // 将菜单树转换为 TreeSelect 数据格式
  const convertToTreeSelectData = (menus: MenuTree[]): any[] => {
    return menus
      .filter((menu) => menu.menuType !== 2) // 过滤掉按钮类型
      .map((menu) => ({
        title: menu.menuName,
        value: menu.id,
        children: menu.children?.length
          ? convertToTreeSelectData(menu.children)
          : undefined,
      }));
  };

  // 搜索
  const handleSearch = () => {
    loadMenus(searchText);
  };

  // 打开新增/编辑弹窗
  const showModal = (menu?: Menu, parentId?: number) => {
    setEditingMenu(menu || null);
    if (menu) {
      form.setFieldsValue({
        parentId: menu.parentId,
        menuName: menu.menuName,
        menuCode: menu.menuCode,
        path: menu.path,
        icon: menu.icon,
        menuType: menu.menuType,
        permission: menu.permission,
        sortOrder: menu.sortOrder,
        status: menu.status,
        isVisible: menu.isVisible,
      });
      setMenuType(menu.menuType);
    } else {
      form.resetFields();
      form.setFieldsValue({
        parentId: parentId || 0,
        menuType: 1,
        status: 1,
        isVisible: 1,
        sortOrder: 0,
      });
      setMenuType(1);
    }
    setIsModalOpen(true);
  };

  // 关闭弹窗
  const handleCancel = () => {
    setIsModalOpen(false);
    setEditingMenu(null);
    form.resetFields();
  };

  // 提交表单
  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingMenu) {
        // 更新菜单
        const updateData: UpdateMenuInput = {
          id: editingMenu.id,
          parentId: values.parentId || 0,
          menuName: values.menuName,
          path: values.path,
          icon: values.icon,
          menuType: values.menuType,
          permission: values.permission,
          sortOrder: values.sortOrder ?? 0,
          status: values.status ?? 1,
          isVisible: values.isVisible ?? 1,
        };
        const res = await menuApi.update(updateData);
        if (res.success) {
          message.success('更新成功');
          loadMenus(searchText);
          loadMenuTree();
          handleCancel();
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        // 新增菜单
        const createData: CreateMenuInput = {
          parentId: values.parentId || 0,
          menuName: values.menuName,
          menuCode: values.menuCode,
          path: values.path,
          icon: values.icon,
          menuType: values.menuType,
          permission: values.permission,
          sortOrder: values.sortOrder ?? 0,
          status: values.status ?? 1,
          isVisible: values.isVisible ?? 1,
        };
        const res = await menuApi.create(createData);
        if (res.success) {
          message.success('新增成功');
          loadMenus(searchText);
          loadMenuTree();
          handleCancel();
        } else {
          message.error(res.message || '新增失败');
        }
      }
    } catch {
      // 表单验证失败
    }
  };

  // 删除菜单
  const handleDelete = async (id: number) => {
    try {
      const res = await menuApi.delete(id);
      if (res.success) {
        message.success('删除成功');
        loadMenus(searchText);
        loadMenuTree();
      } else {
        message.error(res.message || '删除失败');
      }
    } catch (error) {
      message.error('删除失败');
    }
  };

  // 表格列定义
  const columns: ColumnsType<Menu> = [
    {
      title: '序号',
      key: 'index',
      width: 60,
      render: (_, __, index) => index + 1,
    },
    {
      title: '菜单名称',
      dataIndex: 'menuName',
      key: 'menuName',
      width: 200,
    },
    {
      title: '菜单编码',
      dataIndex: 'menuCode',
      key: 'menuCode',
      width: 150,
    },
    {
      title: '类型',
      dataIndex: 'menuType',
      key: 'menuType',
      width: 80,
      render: (type: number) => {
        const config = menuTypeMap[type];
        return <Tag color={config?.color}>{config?.label}</Tag>;
      },
    },
    {
      title: '路由路径',
      dataIndex: 'path',
      key: 'path',
      width: 150,
      ellipsis: true,
    },
    {
      title: '图标',
      dataIndex: 'icon',
      key: 'icon',
      width: 120,
    },
    {
      title: '权限标识',
      dataIndex: 'permission',
      key: 'permission',
      width: 150,
      ellipsis: true,
    },
    {
      title: '排序',
      dataIndex: 'sortOrder',
      key: 'sortOrder',
      width: 60,
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
      title: '可见',
      dataIndex: 'isVisible',
      key: 'isVisible',
      width: 60,
      render: (isVisible: number) => (
        <Tag color={isVisible === 1 ? 'green' : 'red'}>
          {isVisible === 1 ? '是' : '否'}
        </Tag>
      ),
    },
    {
      title: '操作',
      key: 'action',
      width: 200,
      render: (_, record) => (
        <Space>
          {record.menuType !== 2 && (
            <Button
              type="link"
              size="small"
              onClick={() => showModal(undefined, record.id)}
            >
              新增子菜单
            </Button>
          )}
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => showModal(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定要删除此菜单吗？"
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
    <div className="menus-page">
      <Card
        title="菜单管理"
        extra={
          <Space>
            <Input
              placeholder="搜索菜单名称/编码"
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
              新增菜单
            </Button>
          </Space>
        }
      >
        <div className="menus-toolbar">
          <Space>
            <Tooltip title="展开全部">
              <Button size="small" icon={<DownOutlined />} onClick={handleExpandAll}>
                展开
              </Button>
            </Tooltip>
            <Tooltip title="折叠全部">
              <Button size="small" icon={<RightOutlined />} onClick={handleCollapseAll}>
                折叠
              </Button>
            </Tooltip>
          </Space>
          <Space>
            <Button
              size="small"
              type={filterType === null ? 'primary' : 'default'}
              icon={<AppstoreOutlined />}
              onClick={() => setFilterType(null)}
            >
              全部
            </Button>
            <Button
              size="small"
              type={filterType === 0 ? 'primary' : 'default'}
              icon={<FolderOutlined />}
              onClick={() => setFilterType(filterType === 0 ? null : 0)}
            >
              目录
            </Button>
            <Button
              size="small"
              type={filterType === 1 ? 'primary' : 'default'}
              icon={<MenuIconOutlined />}
              onClick={() => setFilterType(filterType === 1 ? null : 1)}
            >
              菜单
            </Button>
            <Button
              size="small"
              type={filterType === 2 ? 'primary' : 'default'}
              onClick={() => setFilterType(filterType === 2 ? null : 2)}
            >
              按钮
            </Button>
          </Space>
        </div>
        <Table
          columns={columns}
          dataSource={filteredTreeData}
          rowKey="id"
          loading={loading}
          pagination={false}
          scroll={{ x: 1310 }}
          expandable={{
            expandedRowKeys,
            onExpand: handleExpand,
            childrenColumnName: 'children',
          }}
        />
      </Card>

      {/* 新增/编辑菜单弹窗 */}
      <Modal
        title={editingMenu ? '编辑菜单' : '新增菜单'}
        open={isModalOpen}
        onOk={handleSubmit}
        onCancel={handleCancel}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ menuType: 1, status: 1, isVisible: 1, sortOrder: 0, parentId: 0 }}
        >
          <Form.Item name="parentId" label="上级菜单">
            <TreeSelect
              placeholder="请选择上级菜单（不选则为顶级菜单）"
              allowClear
              treeData={[
                { title: '顶级菜单', value: 0, children: convertToTreeSelectData(menuTree) },
              ]}
              treeDefaultExpandAll
            />
          </Form.Item>

          <Form.Item
            name="menuType"
            label="菜单类型"
            rules={[{ required: true, message: '请选择菜单类型' }]}
          >
            <Select onChange={(value) => setMenuType(value)}>
              <Select.Option value={0}>目录</Select.Option>
              <Select.Option value={1}>菜单</Select.Option>
              <Select.Option value={2}>按钮</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="menuName"
            label="菜单名称"
            rules={[{ required: true, message: '请输入菜单名称' }]}
          >
            <Input placeholder="请输入菜单名称" />
          </Form.Item>

          {!editingMenu && (
            <Form.Item
              name="menuCode"
              label="菜单编码"
              rules={[
                { required: true, message: '请输入菜单编码' },
                { pattern: /^[a-zA-Z_:]+$/, message: '只能包含英文字母、下划线和冒号' },
              ]}
            >
              <Input placeholder="请输入菜单编码（英文）" />
            </Form.Item>
          )}

          {menuType !== 2 && (
            <>
              <Form.Item name="path" label="路由路径">
                <Input placeholder="请输入路由路径" />
              </Form.Item>

              <Form.Item name="icon" label="图标">
                <Input placeholder="请输入图标名称（如：UserOutlined）" />
              </Form.Item>
            </>
          )}

          <Form.Item name="permission" label="权限标识">
            <Input placeholder="请输入权限标识（如：user:add）" />
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

          {menuType !== 2 && (
            <Form.Item name="isVisible" label="是否可见">
              <Select>
                <Select.Option value={1}>显示</Select.Option>
                <Select.Option value={0}>隐藏</Select.Option>
              </Select>
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
}
