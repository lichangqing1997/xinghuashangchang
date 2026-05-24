import { useState, useEffect } from 'react';
import { Modal, Form, Input, Button, Space, message, Card, Table } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';
import { companyApi } from '../../../api';
import type { CompanyDetailInput } from '../../../types/models';

interface Props {
  id: number | null;
  onClose: (refresh?: boolean) => void;
}

export default function CompanyForm({ id, onClose }: Props) {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [details, setDetails] = useState<CompanyDetailInput[]>([]);

  useEffect(() => {
    if (id) {
      loadCompany();
    }
  }, [id]);

  const loadCompany = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const [companyRes, detailsRes] = await Promise.all([
        companyApi.getById(id),
        companyApi.getDetails(id),
      ]);
      if (companyRes.success && companyRes.data) {
        form.setFieldsValue(companyRes.data);
      }
      if (detailsRes.success && detailsRes.data) {
        setDetails(detailsRes.data.map(d => ({ fieldName: d.fieldName, fieldValue: d.fieldValue })));
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
        details: details.filter(d => d.fieldName),
      };

      if (id) {
        const res = await companyApi.update({ id, ...input });
        if (res.success) {
          message.success('更新成功');
          onClose(true);
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        const res = await companyApi.create(input);
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

  const addDetail = () => {
    setDetails([...details, { fieldName: '', fieldValue: '' }]);
  };

  const removeDetail = (index: number) => {
    setDetails(details.filter((_, i) => i !== index));
  };

  const updateDetail = (index: number, field: keyof CompanyDetailInput, value: string) => {
    const newDetails = [...details];
    newDetails[index] = { ...newDetails[index], [field]: value };
    setDetails(newDetails);
  };

  const detailColumns = [
    {
      title: '字段名称',
      dataIndex: 'fieldName',
      key: 'fieldName',
      width: 200,
      render: (_: unknown, __: CompanyDetailInput, index?: number) => (
        <Input
          value={index !== undefined ? details[index].fieldName : ''}
          onChange={(e) => index !== undefined && updateDetail(index, 'fieldName', e.target.value)}
          placeholder="请输入字段名称"
        />
      ),
    },
    {
      title: '字段值',
      dataIndex: 'fieldValue',
      key: 'fieldValue',
      width: 300,
      render: (_: unknown, __: CompanyDetailInput, index?: number) => (
        <Input
          value={index !== undefined ? details[index].fieldValue : ''}
          onChange={(e) => index !== undefined && updateDetail(index, 'fieldValue', e.target.value)}
          placeholder="请输入字段值"
        />
      ),
    },
    {
      title: '操作',
      key: 'action',
      width: 80,
      render: (_: unknown, __: CompanyDetailInput, index?: number) => (
        <Button
          type="link"
          danger
          icon={<DeleteOutlined />}
          onClick={() => index !== undefined && removeDetail(index)}
        />
      ),
    },
  ];

  return (
    <Modal
      title={id ? '编辑公司' : '新增公司'}
      open
      onOk={handleSubmit}
      onCancel={() => onClose()}
      confirmLoading={loading}
      width={800}
      destroyOnClose
    >
      <Form
        form={form}
        layout="vertical"
        initialValues={{ status: 1 }}
      >
        <Form.Item
          name="companyName"
          label="公司名称"
          rules={[{ required: true, message: '请输入公司名称' }]}
        >
          <Input placeholder="请输入公司名称" />
        </Form.Item>

        <Form.Item
          name="legalPerson"
          label="法人"
        >
          <Input placeholder="请输入法人" />
        </Form.Item>

        <Form.Item
          name="businessLicenseNo"
          label="营业执照号"
        >
          <Input placeholder="请输入营业执照号" />
        </Form.Item>

        <Form.Item
          name="companyCode"
          label="公司单位编码"
        >
          <Input placeholder="请输入公司单位编码" />
        </Form.Item>
      </Form>

      <Card
        title="扩展信息"
        size="small"
        extra={
          <Button
            type="dashed"
            icon={<PlusOutlined />}
            onClick={addDetail}
            size="small"
          >
            添加字段
          </Button>
        }
      >
        <Table
          columns={detailColumns}
          dataSource={details}
          rowKey={(_, index) => (index ?? 0).toString()}
          pagination={false}
          size="small"
        />
      </Card>
    </Modal>
  );
}
