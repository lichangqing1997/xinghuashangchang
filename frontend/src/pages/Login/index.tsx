import { useState, useEffect } from 'react';
import { Form, Input, Button, Card, message, Typography, Space, Tag } from 'antd';
import { UserOutlined, LockOutlined, AndroidOutlined, CloudDownloadOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../stores/authStore';
import { LoginInput, FileAttachment } from '../../types/models';
import { fileApi } from '../../api/file';
import './index.css';

const { Title, Text } = Typography;

// 检测是否为移动端
const isMobile = () => {
  const userAgent = navigator.userAgent || '';
  return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(userAgent);
};

// 检测是否为 Android 设备
const isAndroid = () => {
  const userAgent = navigator.userAgent || '';
  return /Android/i.test(userAgent);
};

export default function LoginPage() {
  const [loading, setLoading] = useState(false);
  const [upgradeLoading, setUpgradeLoading] = useState(false);
  const [latestApk, setLatestApk] = useState<FileAttachment | null>(null);
  const { login } = useAuth();
  const navigate = useNavigate();

  // 检查是否有最新 APK
  useEffect(() => {
    if (isAndroid()) {
      checkLatestApk();
    }
  }, []);

  const checkLatestApk = async () => {
    try {
      const res = await fileApi.getLatestApk();
      if (res.success && res.data) {
        setLatestApk(res.data as unknown as FileAttachment);
      }
    } catch (error) {
      // 静默失败，不影响登录
      console.error('检查更新失败:', error);
    }
  };

  const handleUpgrade = async () => {
    if (!latestApk) {
      message.info('暂无可用更新');
      return;
    }

    setUpgradeLoading(true);
    try {
      // 下载 APK 文件
      const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5242';
      const downloadUrl = `${baseUrl}/api/File/download/${latestApk.id}`;

      // 创建临时链接触发下载
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.download = latestApk.fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      message.success('正在下载最新版本...');
    } catch (error) {
      message.error('下载失败，请稍后重试');
    } finally {
      setUpgradeLoading(false);
    }
  };

  const onFinish = async (values: LoginInput) => {
    setLoading(true);
    try {
      await login(values);
      message.success('登录成功');
      navigate('/');
    } catch (error) {
      message.error(error instanceof Error ? error.message : '登录失败');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-content">
        <Card className="login-card">
          <div className="login-header">
            <Title level={2} className="login-title">
              兴华商城管理系统
            </Title>
            <Text type="secondary">服装仓储管理系统</Text>
          </div>

          <Form
            name="login"
            onFinish={onFinish}
            autoComplete="off"
            size="large"
          >
            <Form.Item
              name="username"
              rules={[{ required: true, message: '请输入用户名' }]}
            >
              <Input
                prefix={<UserOutlined />}
                placeholder="用户名"
              />
            </Form.Item>

            <Form.Item
              name="password"
              rules={[{ required: true, message: '请输入密码' }]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder="密码"
              />
            </Form.Item>

            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={loading}
                block
              >
                登录
              </Button>
            </Form.Item>
          </Form>

          <div className="login-footer">
            <Text type="secondary" className="login-hint">
              默认账号: admin / admin123
            </Text>
          </div>

          {/* Android 设备显示自动升级按钮 */}
          {isAndroid() && latestApk && (
            <div className="login-upgrade">
              <Button
                type="default"
                icon={<CloudDownloadOutlined />}
                loading={upgradeLoading}
                onClick={handleUpgrade}
                block
                style={{ marginTop: 16 }}
              >
                <Space>
                  <AndroidOutlined />
                  自动升级
                  {latestApk.version && (
                    <Tag color="blue" style={{ marginLeft: 4 }}>
                      v{latestApk.version}
                    </Tag>
                  )}
                </Space>
              </Button>
            </div>
          )}
        </Card>
      </div>
    </div>
  );
}
