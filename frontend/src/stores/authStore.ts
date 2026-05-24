import { useState, useCallback, useEffect } from 'react';
import { User, LoginInput, MenuTree } from '../types/models';
import { authApi } from '../api/auth';
import { menuApi } from '../api/menu';

// 认证状态 Hook
export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [menus, setMenus] = useState<MenuTree[]>([]);
  const [loading, setLoading] = useState(true);

  // 加载用户菜单
  const loadMenus = useCallback(async (userId: number) => {
    try {
      const res = await menuApi.getUserTree(userId);
      if (res.success && res.data) {
        setMenus(res.data);
        localStorage.setItem('userMenus', JSON.stringify(res.data));
      }
    } catch (err) {
      console.error('加载用户菜单失败', err);
    }
  }, []);

  // 初始化时检查本地存储的用户信息
  useEffect(() => {
    const storedUser = localStorage.getItem('user');
    const storedToken = localStorage.getItem('token');
    if (storedUser && storedToken) {
      try {
        const parsed = JSON.parse(storedUser);
        setUser(parsed);
        // 从本地缓存恢复菜单
        const storedMenus = localStorage.getItem('userMenus');
        if (storedMenus) {
          setMenus(JSON.parse(storedMenus));
        }
        // 异步刷新菜单
        if (parsed.id) {
          loadMenus(parsed.id);
        }
      } catch {
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        localStorage.removeItem('userMenus');
      }
    }
    setLoading(false);
  }, [loadMenus]);

  // 登录
  const login = useCallback(async (input: LoginInput) => {
    const result = await authApi.login(input);
    if (result.success && result.data) {
      const userData = result.data;
      setUser(userData);
      localStorage.setItem('user', JSON.stringify(userData));
      if (userData.token) {
        localStorage.setItem('token', userData.token);
      }
      // 加载用户菜单
      if (userData.id) {
        await loadMenus(userData.id);
      }
      return true;
    }
    throw new Error(result.message || '登录失败');
  }, [loadMenus]);

  // 退出登录
  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } catch {
      // 忽略退出登录的错误
    } finally {
      setUser(null);
      setMenus([]);
      localStorage.removeItem('user');
      localStorage.removeItem('token');
      localStorage.removeItem('userMenus');
    }
  }, []);

  // 检查是否已登录
  const isAuthenticated = !!user;

  // 检查是否有特定角色
  const hasRole = useCallback(
    (role: string) => user?.role === role,
    [user]
  );

  return {
    user,
    menus,
    loading,
    login,
    logout,
    isAuthenticated,
    hasRole,
  };
}
