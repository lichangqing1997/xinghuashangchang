import { useState, useMemo } from 'react';
import { Layout, Menu, Avatar, Dropdown, Button, Typography } from 'antd';
import {
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  DashboardOutlined,
  ShoppingOutlined,
  InboxOutlined,
  ScanOutlined,
  TeamOutlined,
  SettingOutlined,
  LogoutOutlined,
  UserOutlined,
  SafetyOutlined,
  MenuOutlined,
  FileOutlined,
  DatabaseOutlined,
  UnorderedListOutlined,
  QrcodeOutlined,
  AppstoreOutlined,
} from '@ant-design/icons';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../stores/authStore';
import type { MenuTree } from '../../types/models';
import './index.css';

const { Header, Sider, Content } = Layout;
const { Text } = Typography;

// 图标映射
const iconMap: Record<string, React.ReactNode> = {
  DashboardOutlined: <DashboardOutlined />,
  ShoppingOutlined: <ShoppingOutlined />,
  InboxOutlined: <InboxOutlined />,
  ScanOutlined: <ScanOutlined />,
  TeamOutlined: <TeamOutlined />,
  SettingOutlined: <SettingOutlined />,
  UserOutlined: <UserOutlined />,
  SafetyOutlined: <SafetyOutlined />,
  MenuOutlined: <MenuOutlined />,
  FileOutlined: <FileOutlined />,
  DatabaseOutlined: <DatabaseOutlined />,
  UnorderedListOutlined: <UnorderedListOutlined />,
  QrcodeOutlined: <QrcodeOutlined />,
  AppstoreOutlined: <AppstoreOutlined />,
};

// 将后端菜单树转换为 Ant Design Menu 数据格式
function buildMenuItems(tree: MenuTree[]): any[] {
  return tree
    .filter((item) => item.isVisible === 1 && item.status === 1)
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map((item) => {
      // 按钮类型不显示在菜单中
      if (item.menuType === 2) return null;

      const icon = item.icon ? iconMap[item.icon] || <AppstoreOutlined /> : <AppstoreOutlined />;
      const children = item.children?.filter((c) => c.menuType !== 2 && c.isVisible === 1 && c.status === 1);

      if (children && children.length > 0) {
        return {
          key: item.menuCode || `dir_${item.id}`,
          icon,
          label: item.menuName,
          children: buildMenuItems(children),
        };
      }

      return {
        key: item.path || `menu_${item.id}`,
        icon,
        label: item.menuName,
      };
    })
    .filter(Boolean);
}

// 从菜单树中查找需要展开的父菜单 key
function findOpenKeys(tree: MenuTree[], path: string, parents: string[]): string[] {
  for (const item of tree) {
    if (item.path === path) {
      return parents;
    }
    if (item.children?.length) {
      const found = findOpenKeys(item.children, path, [...parents, item.menuCode || `dir_${item.id}`]);
      if (found.length > 0) return found;
    }
  }
  return [];
}

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const [openKeys, setOpenKeys] = useState<string[]>([]);
  const { user, menus, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  // 动态生成菜单项
  const menuItems = useMemo(() => {
    if (menus && menus.length > 0) {
      return buildMenuItems(menus);
    }
    return [];
  }, [menus]);

  // 当前选中的菜单
  const selectedKeys = useMemo(() => [location.pathname], [location.pathname]);

  // 路由变化时，自动展开当前路径对应的父菜单
  useMemo(() => {
    const keys = findOpenKeys(menus, location.pathname, []);
    if (keys.length > 0) {
      setOpenKeys(keys);
    }
  }, [location.pathname, menus]);

  // 手风琴模式：展开新菜单时关闭其他同级菜单
  const handleOpenChange = (keys: string[]) => {
    // 找到本次新打开的 key
    const latestOpenKey = keys.find((key) => openKeys.indexOf(key) === -1);
    // 如果新打开的是顶级菜单，只保留它；否则保留所有
    const rootSubmenuKeys = menuItems
      .filter((item: any) => item.children)
      .map((item: any) => item.key);
    if (latestOpenKey && rootSubmenuKeys.indexOf(latestOpenKey) !== -1) {
      setOpenKeys([latestOpenKey]);
    } else {
      setOpenKeys(keys);
    }
  };

  // 用户下拉菜单
  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: '个人信息',
    },
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: '系统设置',
    },
    {
      type: 'divider' as const,
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: '退出登录',
      danger: true,
    },
  ];

  // 处理菜单点击
  const handleMenuClick = (info: { key: string }) => {
    if (info.key.startsWith('/')) {
      navigate(info.key);
    }
  };

  // 处理用户菜单点击
  const handleUserMenuClick = async (info: { key: string }) => {
    if (info.key === 'logout') {
      await logout();
      navigate('/login');
    } else if (info.key === 'profile') {
      navigate('/profile');
    } else if (info.key === 'settings') {
      navigate('/settings');
    }
  };

  return (
    <Layout className="main-layout">
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        className="main-sider"
        theme="dark"
      >
        <div className="logo">
          {collapsed ? '兴华' : '兴华商城'}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={selectedKeys}
          openKeys={openKeys}
          onOpenChange={handleOpenChange}
          items={menuItems}
          onClick={handleMenuClick}
        />
      </Sider>

      <Layout style={{ marginLeft: collapsed ? 80 : 200 }}>
        <Header className="main-header">
          <Button
            type="text"
            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
            onClick={() => setCollapsed(!collapsed)}
            className="trigger-btn"
          />

          <div className="header-right">
            <Dropdown
              menu={{
                items: userMenuItems,
                onClick: handleUserMenuClick,
              }}
              placement="bottomRight"
            >
              <div className="user-info">
                <Avatar icon={<UserOutlined />} />
                <Text className="username">{user?.name || '用户'}</Text>
              </div>
            </Dropdown>
          </div>
        </Header>

        <Content className="main-content">
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
