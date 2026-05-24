import { createBrowserRouter, Navigate } from 'react-router-dom';
import MainLayout from '../layouts/MainLayout';
import LoginPage from '../pages/Login';
import DashboardPage from '../pages/Dashboard';
import ProductsPage from '../pages/Products';
import PDAPage from '../pages/PDA';
import InventoryPage from '../pages/Inventory';
import SuppliersPage from '../pages/Suppliers';
import CompaniesPage from '../pages/Companies';
import OutboundOrdersPage from '../pages/OutboundOrders';
import OutboundFlowsPage from '../pages/OutboundFlows';
import UsersPage from '../pages/Users';
import RolesPage from '../pages/Roles';
import MenusPage from '../pages/Menus';
import FilesPage from '../pages/Files';
import ProfilePage from '../pages/Profile';
import SettingsPage from '../pages/Settings';

// 路由守卫组件
function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem('token');
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
}

// 路由配置
export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: 'products',
        element: <ProductsPage />,
      },
      {
        path: 'inventory',
        element: <InventoryPage />,
      },
      {
        path: 'pda',
        element: <PDAPage />,
      },
      {
        path: 'suppliers',
        element: <SuppliersPage />,
      },
      {
        path: 'companies',
        element: <CompaniesPage />,
      },
      {
        path: 'outbound-orders',
        element: <OutboundOrdersPage />,
      },
      {
        path: 'outbound-flows',
        element: <OutboundFlowsPage />,
      },
      {
        path: 'system/users',
        element: <UsersPage />,
      },
      {
        path: 'system/roles',
        element: <RolesPage />,
      },
      {
        path: 'system/menus',
        element: <MenusPage />,
      },
      {
        path: 'system/files',
        element: <FilesPage />,
      },
      {
        path: 'profile',
        element: <ProfilePage />,
      },
      {
        path: 'settings',
        element: <SettingsPage />,
      },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);
