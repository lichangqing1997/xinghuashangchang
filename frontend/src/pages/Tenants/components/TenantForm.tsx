import { useState, useEffect } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, message } from 'antd';
import { tenantApi } from '../../../api';
import dayjs from 'dayjs';

interface Props {
  id: number | null;
  onClose: (refresh?: boolean) => void;
}

export default function TenantForm({ id, onClose }: Props) {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      loadTenant();
    }
  }, [id]);

  const loadTenant = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const res = await tenantApi.getById(id);
      if (res.success && res.data) {
        form.setFieldsValue({
          ...res.data,
          expireAt: res.data.expireAt ? dayjs(res.data.expireAt) : undefined,
        });
      }
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);

      const input = {
        ...values,
        expireAt: values.expireAt ? values.expireAt.toISOString() : undefined,
      };

      if (id) {
        const res = await tenantApi.update({ id, ...input });
        if (res.success) {
          message.success('更新成功');
          onClose(true);
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        const res = await tenantApi.create(input);
        if (res.success) {
          message.success('创建成功');
          onClose(true);
        } else {
          message.error(res.message || '创建失败');
        }
      }
    } catch {
      // validation failed
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={id ? '编辑租户' : '新增租户'}
      open
      onOk={handleSubmit}
      onCancel={() => onClose()}
      confirmLoading={loading}
      width={700}
      destroyOnClose
    >
      <Form
        form={form}
        layout="vertical"
        initialValues={{ status: 1, maxUsers: 100 }}
      >
        <Form.Item
          name="tenantCode"
          label="租户编码"
          rules={[
            { required: true, message: '请输入租户编码' },
            { pattern: /^[a-zA-Z0-9_-]+$/, message: '只能包含字母、数字、下划线和横线' },
          ]}
        >
          <Input placeholder="请输入租户编码（唯一标识）" disabled={!!id} />
        </Form.Item>

        <Form.Item
          name="tenantName"
          label="租户名称"
          rules={[{ required: true, message: '请输入租户名称' }]}
        >
          <Input placeholder="请输入租户名称" />
        </Form.Item>

        <Form.Item
          name="contactPerson"
          label="联系人"
        >
          <Input placeholder="请输入联系人" />
        </Form.Item>

        <Form.Item
          name="contactPhone"
          label="联系电话"
        >
          <Input placeholder="请输入联系电话" />
        </Form.Item>

        <Form.Item
          name="contactEmail"
          label="联系邮箱"
          rules={[{ type: 'email', message: '请输入正确的邮箱格式' }]}
        >
          <Input placeholder="请输入联系邮箱" />
        </Form.Item>

        <Form.Item
          name="address"
          label="地址"
        >
          <Input placeholder="请输入地址" />
        </Form.Item>

        <Form.Item
          name="databaseName"
          label="数据库名称"
        >
          <Input placeholder="请输入数据库名称" />
        </Form.Item>

        <Form.Item
          name="connectionString"
          label="数据库连接字符串"
        >
          <Input.TextArea rows={2} placeholder="请输入数据库连接字符串（独立数据库模式）" />
        </Form.Item>

        <Form.Item
          name="maxUsers"
          label="最大用户数"
          rules={[{ required: true, message: '请输入最大用户数' }]}
        >
          <InputNumber min={1} max={10000} style={{ width: '100%' }} placeholder="请输入最大用户数" />
        </Form.Item>

        <Form.Item
          name="expireAt"
          label="过期时间"
        >
          <DatePicker showTime style={{ width: '100%' }} placeholder="留空表示永不过期" />
        </Form.Item>

        <Form.Item
          name="remark"
          label="备注"
        >
          <Input.TextArea rows={3} placeholder="请输入备注" />
        </Form.Item>
      </Form>
    </Modal>
  );
}
