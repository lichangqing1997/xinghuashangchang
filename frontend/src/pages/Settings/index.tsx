import { useState, useEffect } from 'react';
import { Card, Form, Switch, Select, Button, message, Divider, Space, InputNumber } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import './index.css';

interface SystemSettings {
  pageSize: number;
  autoRefresh: boolean;
  refreshInterval: number;
  showStockWarning: boolean;
  stockWarningThreshold: number;
  compactMode: boolean;
}

const defaultSettings: SystemSettings = {
  pageSize: 10,
  autoRefresh: false,
  refreshInterval: 30,
  showStockWarning: true,
  stockWarningThreshold: 10,
  compactMode: false,
};

const SETTINGS_KEY = 'system_settings';

function loadSettings(): SystemSettings {
  try {
    const stored = localStorage.getItem(SETTINGS_KEY);
    if (stored) {
      return { ...defaultSettings, ...JSON.parse(stored) };
    }
  } catch {
    // ignore
  }
  return defaultSettings;
}

export default function SettingsPage() {
  const [form] = Form.useForm();
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    form.setFieldsValue(loadSettings());
  }, []);

  const handleSave = async () => {
    try {
      const values = await form.validateFields();
      setSaving(true);
      localStorage.setItem(SETTINGS_KEY, JSON.stringify(values));
      message.success('设置已保存');
    } catch {
      // validation failed
    } finally {
      setSaving(false);
    }
  };

  const handleReset = () => {
    form.setFieldsValue(defaultSettings);
    localStorage.setItem(SETTINGS_KEY, JSON.stringify(defaultSettings));
    message.success('已恢复默认设置');
  };

  return (
    <div className="settings-page">
      <Card
        title="系统设置"
        extra={
          <Space>
            <Button onClick={handleReset}>恢复默认</Button>
            <Button type="primary" icon={<SaveOutlined />} onClick={handleSave} loading={saving}>
              保存设置
            </Button>
          </Space>
        }
      >
        <Form form={form} layout="vertical" initialValues={defaultSettings}>
          <Divider orientation="left">表格设置</Divider>

          <Form.Item name="pageSize" label="默认每页条数">
            <Select>
              <Select.Option value={10}>10 条</Select.Option>
              <Select.Option value={20}>20 条</Select.Option>
              <Select.Option value={50}>50 条</Select.Option>
              <Select.Option value={100}>100 条</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="compactMode" label="紧凑模式" valuePropName="checked">
            <Switch />
          </Form.Item>

          <Divider orientation="left">库存预警</Divider>

          <Form.Item name="showStockWarning" label="显示库存预警" valuePropName="checked">
            <Switch />
          </Form.Item>

          <Form.Item name="stockWarningThreshold" label="预警阈值">
            <InputNumber min={1} max={999} addonAfter="件" />
          </Form.Item>

          <Divider orientation="left">自动刷新</Divider>

          <Form.Item name="autoRefresh" label="启用自动刷新" valuePropName="checked">
            <Switch />
          </Form.Item>

          <Form.Item name="refreshInterval" label="刷新间隔">
            <Select>
              <Select.Option value={15}>15 秒</Select.Option>
              <Select.Option value={30}>30 秒</Select.Option>
              <Select.Option value={60}>1 分钟</Select.Option>
              <Select.Option value={300}>5 分钟</Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
}
