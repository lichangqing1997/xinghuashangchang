import { useState } from 'react';
import { Modal, Form, Input, Radio, message } from 'antd';
import { inboundOrderApi } from '../../../api';

const { TextArea } = Input;

interface Props {
  open: boolean;
  orderId: number;
  onClose: (refresh?: boolean) => void;
}

export default function AuditModal({ open, orderId, onClose }: Props) {
  const [form] = Form.useForm();
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async () => {
    try {
      await form.validateFields();
    } catch {
      return;
    }

    const values = form.getFieldsValue();
    setSubmitting(true);
    try {
      const res = await inboundOrderApi.audit(orderId, {
        isApproved: values.isApproved,
        auditor: values.auditor,
        auditRemark: values.auditRemark,
      });
      if (res.success) {
        message.success('审核成功');
        onClose(true);
      } else {
        message.error(res.message || '审核失败');
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      title="入库单审核"
      open={open}
      onCancel={() => onClose()}
      onOk={handleSubmit}
      confirmLoading={submitting}
      width={500}
      destroyOnClose
    >
      <Form form={form} layout="vertical" initialValues={{ isApproved: true }}>
        <Form.Item
          name="isApproved"
          label="审核结果"
          rules={[{ required: true, message: '请选择审核结果' }]}
        >
          <Radio.Group>
            <Radio value={true}>通过</Radio>
            <Radio value={false}>驳回</Radio>
          </Radio.Group>
        </Form.Item>
        <Form.Item name="auditor" label="审核人">
          <Input placeholder="请输入审核人" />
        </Form.Item>
        <Form.Item name="auditRemark" label="审核备注">
          <TextArea rows={3} placeholder="请输入审核备注" />
        </Form.Item>
      </Form>
    </Modal>
  );
}
