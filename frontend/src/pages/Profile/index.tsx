import { useState, useEffect } from 'react';
import {
  Card,
  Form,
  Input,
  Button,
  Space,
  message,
  Descriptions,
  Tag,
  Divider,
  Modal,
  Avatar,
  Upload,
} from 'antd';
import { UserOutlined, EditOutlined, LockOutlined, UploadOutlined } from '@ant-design/icons';
import type { UploadProps } from 'antd';
import { useAuth } from '../../stores/authStore';
import { userApi } from '../../api';
import { UserDetail } from '../../types/models';
import { apiClient } from '../../api/client';
import './index.css';

export default function ProfilePage() {
  const { user } = useAuth();
  const [userDetail, setUserDetail] = useState<UserDetail | null>(null);
  const [loading, setLoading] = useState(false);
  const [editForm] = Form.useForm();
  const [pwdForm] = Form.useForm();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isPwdModalOpen, setIsPwdModalOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [avatarUrl, setAvatarUrl] = useState<string>('');

  const loadUserDetail = async () => {
    if (!user?.id) return;
    setLoading(true);
    try {
      const res = await userApi.getById(user.id);
      if (res.success && res.data) {
        setUserDetail(res.data);
        if (res.data.avatar) {
          setAvatarUrl(`${apiClient.defaults.baseURL}${res.data.avatar}`);
        }
      }
    } catch {
      message.error('加载用户信息失败');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUserDetail();
  }, [user?.id]);

  const handleEdit = () => {
    if (userDetail) {
      editForm.setFieldsValue({
        realName: userDetail.realName,
        phone: userDetail.phone,
        email: userDetail.email,
      });
    }
    setIsEditModalOpen(true);
  };

  const handleEditSubmit = async () => {
    if (!user?.id) return;
    try {
      const values = await editForm.validateFields();
      setSaving(true);
      const res = await userApi.update({
        id: user.id,
        ...values,
        status: userDetail?.status ?? 1,
        roleIds: userDetail?.roleIds ?? [],
      });
      if (res.success) {
        message.success('更新成功');
        setIsEditModalOpen(false);
        loadUserDetail();
      } else {
        message.error(res.message || '更新失败');
      }
    } catch {
      // validation failed
    } finally {
      setSaving(false);
    }
  };

  const handlePwdSubmit = async () => {
    if (!user?.id) return;
    try {
      const values = await pwdForm.validateFields();
      setSaving(true);
      const res = await userApi.changePassword({
        userId: user.id,
        oldPassword: values.oldPassword,
        newPassword: values.newPassword,
      });
      if (res.success) {
        message.success('密码修改成功');
        setIsPwdModalOpen(false);
        pwdForm.resetFields();
      } else {
        message.error(res.message || '密码修改失败');
      }
    } catch {
      // validation failed
    } finally {
      setSaving(false);
    }
  };

  const handleAvatarChange: UploadProps['onChange'] = async (info) => {
    if (info.file.status === 'done') {
      const res = info.file.response;
      if (res?.success) {
        const url = `${apiClient.defaults.baseURL}${res.data}`;
        setAvatarUrl(url);
        message.success('头像上传成功');
        loadUserDetail();
      } else {
        message.error(res?.message || '头像上传失败');
      }
    } else if (info.file.status === 'error') {
      message.error('头像上传失败');
    }
  };

  const beforeUpload = (file: File) => {
    const isImage = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'].includes(file.type);
    if (!isImage) {
      message.error('只能上传 JPG、PNG、GIF、WEBP 格式的图片');
      return false;
    }
    const isLt5M = file.size / 1024 / 1024 < 5;
    if (!isLt5M) {
      message.error('图片大小不能超过 5MB');
      return false;
    }
    return true;
  };

  return (
    <div className="profile-page">
      <Card loading={loading}>
        <div className="profile-header">
          <div className="profile-avatar-wrapper">
            <Upload
              name="file"
              action={`/api/User/avatar?userId=${user?.id}`}
              headers={{ Authorization: `Bearer ${localStorage.getItem('token')}` }}
              showUploadList={false}
              beforeUpload={beforeUpload}
              onChange={handleAvatarChange}
              accept="image/*"
            >
              <div className="profile-avatar-edit">
                <Avatar
                  size={80}
                  src={avatarUrl || undefined}
                  icon={!avatarUrl ? <UserOutlined /> : undefined}
                />
                <div className="profile-avatar-overlay">
                  <UploadOutlined />
                </div>
              </div>
            </Upload>
          </div>
          <div className="profile-header-info">
            <h2>{userDetail?.realName || userDetail?.username || '用户'}</h2>
            <Space>
              <Tag color="blue">{user?.role || '未知角色'}</Tag>
              <span style={{ color: '#999' }}>@{userDetail?.username}</span>
            </Space>
          </div>
        </div>

        <Divider />

        <Descriptions
          title="基本信息"
          column={2}
          extra={
            <Space>
              <Button icon={<EditOutlined />} onClick={handleEdit}>
                编辑资料
              </Button>
              <Button icon={<LockOutlined />} onClick={() => setIsPwdModalOpen(true)}>
                修改密码
              </Button>
            </Space>
          }
        >
          <Descriptions.Item label="用户名">{userDetail?.username}</Descriptions.Item>
          <Descriptions.Item label="真实姓名">{userDetail?.realName || '-'}</Descriptions.Item>
          <Descriptions.Item label="手机号">{userDetail?.phone || '-'}</Descriptions.Item>
          <Descriptions.Item label="邮箱">{userDetail?.email || '-'}</Descriptions.Item>
          <Descriptions.Item label="角色">
            {userDetail?.roles?.map((r) => (
              <Tag key={r.id} color="blue">
                {r.roleName}
              </Tag>
            )) || '-'}
          </Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag color={userDetail?.status === 1 ? 'green' : 'red'}>
              {userDetail?.status === 1 ? '启用' : '禁用'}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="创建时间">{userDetail?.createdAt}</Descriptions.Item>
          <Descriptions.Item label="最后登录">{userDetail?.lastLoginAt || '-'}</Descriptions.Item>
        </Descriptions>
      </Card>

      {/* 编辑资料弹窗 */}
      <Modal
        title="编辑资料"
        open={isEditModalOpen}
        onOk={handleEditSubmit}
        onCancel={() => setIsEditModalOpen(false)}
        confirmLoading={saving}
        width={480}
      >
        <Form form={editForm} layout="vertical">
          <Form.Item name="realName" label="真实姓名">
            <Input placeholder="请输入真实姓名" />
          </Form.Item>
          <Form.Item
            name="phone"
            label="手机号"
            rules={[{ pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确' }]}
          >
            <Input placeholder="请输入手机号" />
          </Form.Item>
          <Form.Item name="email" label="邮箱" rules={[{ type: 'email', message: '邮箱格式不正确' }]}>
            <Input placeholder="请输入邮箱" />
          </Form.Item>
        </Form>
      </Modal>

      {/* 修改密码弹窗 */}
      <Modal
        title="修改密码"
        open={isPwdModalOpen}
        onOk={handlePwdSubmit}
        onCancel={() => {
          setIsPwdModalOpen(false);
          pwdForm.resetFields();
        }}
        confirmLoading={saving}
        width={480}
      >
        <Form form={pwdForm} layout="vertical">
          <Form.Item
            name="oldPassword"
            label="原密码"
            rules={[{ required: true, message: '请输入原密码' }]}
          >
            <Input.Password placeholder="请输入原密码" />
          </Form.Item>
          <Form.Item
            name="newPassword"
            label="新密码"
            rules={[
              { required: true, message: '请输入新密码' },
              { min: 6, message: '密码长度至少6个字符' },
            ]}
          >
            <Input.Password placeholder="请输入新密码" />
          </Form.Item>
          <Form.Item
            name="confirmPassword"
            label="确认新密码"
            dependencies={['newPassword']}
            rules={[
              { required: true, message: '请确认新密码' },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('newPassword') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('两次输入的密码不一致'));
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
