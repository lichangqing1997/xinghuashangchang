import { useState, useEffect } from 'react';
import { Modal, Form, Input, Table, Button, Space, message, Spin, InputNumber } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { inboundOrderApi } from '../../../api';
import type { CreateInboundOrderItemInput } from '../../../types/models';

const { TextArea } = Input;

interface LineItem {
  key: string;
  seqNo: number;
  materialCode: string;
  materialName: string;
  purchaseQuantity: number;
  batchNo: string;
  batch1: string;
  batch2: string;
  batch3: string;
  batch4: string;
  batch5: string;
  batch6: string;
  batch7: string;
  batch8: string;
  batch9: string;
  batch10: string;
  reserveRemark1: string;
  reserveRemark2: string;
  reserveRemark3: string;
  reserveRemark4: string;
  reserveRemark5: string;
  reserveRemark6: string;
  reserveRemark7: string;
  reserveRemark8: string;
  reserveRemark9: string;
  reserveRemark10: string;
}

let lineKeySeq = 0;
function newLineKey() { return `line_${++lineKeySeq}`; }

interface Props {
  open: boolean;
  orderId: number | null;
  onClose: (refresh?: boolean) => void;
}

export default function InboundOrderForm({ open, orderId, onClose }: Props) {
  const [form] = Form.useForm();
  const [lines, setLines] = useState<LineItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (open && orderId) {
      loadOrderData();
    } else if (open) {
      form.resetFields();
      setLines([createEmptyLine(1)]);
    }
  }, [open, orderId]);

  const loadOrderData = async () => {
    if (!orderId) return;
    setLoading(true);
    try {
      const [orderRes, itemsRes] = await Promise.all([
        inboundOrderApi.getById(orderId),
        inboundOrderApi.getItems(orderId),
      ]);
      if (orderRes.success && orderRes.data) {
        form.setFieldsValue({
          supplierName: orderRes.data.supplierName,
          source: orderRes.data.source,
          orderType: orderRes.data.orderType,
          creator: orderRes.data.creator,
          remark: orderRes.data.remark,
        });
      }
      if (itemsRes.success && itemsRes.data) {
        setLines(itemsRes.data.map((item) => ({
          key: newLineKey(),
          seqNo: item.seqNo,
          materialCode: item.materialCode || '',
          materialName: item.materialName || '',
          purchaseQuantity: item.purchaseQuantity,
          batchNo: item.batchNo || '',
          batch1: item.batch1 || '',
          batch2: item.batch2 || '',
          batch3: item.batch3 || '',
          batch4: item.batch4 || '',
          batch5: item.batch5 || '',
          batch6: item.batch6 || '',
          batch7: item.batch7 || '',
          batch8: item.batch8 || '',
          batch9: item.batch9 || '',
          batch10: item.batch10 || '',
          reserveRemark1: item.reserveRemark1 || '',
          reserveRemark2: item.reserveRemark2 || '',
          reserveRemark3: item.reserveRemark3 || '',
          reserveRemark4: item.reserveRemark4 || '',
          reserveRemark5: item.reserveRemark5 || '',
          reserveRemark6: item.reserveRemark6 || '',
          reserveRemark7: item.reserveRemark7 || '',
          reserveRemark8: item.reserveRemark8 || '',
          reserveRemark9: item.reserveRemark9 || '',
          reserveRemark10: item.reserveRemark10 || '',
        })));
      }
    } finally {
      setLoading(false);
    }
  };

  function createEmptyLine(seqNo: number): LineItem {
    return {
      key: newLineKey(),
      seqNo,
      materialCode: '',
      materialName: '',
      purchaseQuantity: 1,
      batchNo: '',
      batch1: '',
      batch2: '',
      batch3: '',
      batch4: '',
      batch5: '',
      batch6: '',
      batch7: '',
      batch8: '',
      batch9: '',
      batch10: '',
      reserveRemark1: '',
      reserveRemark2: '',
      reserveRemark3: '',
      reserveRemark4: '',
      reserveRemark5: '',
      reserveRemark6: '',
      reserveRemark7: '',
      reserveRemark8: '',
      reserveRemark9: '',
      reserveRemark10: '',
    };
  }

  const addLine = () => {
    const newSeqNo = lines.length > 0 ? Math.max(...lines.map(l => l.seqNo)) + 1 : 1;
    setLines([...lines, createEmptyLine(newSeqNo)]);
  };

  const removeLine = (key: string) => {
    if (lines.length <= 1) {
      message.warning('至少需要一行明细');
      return;
    }
    setLines(lines.filter((l) => l.key !== key));
  };

  const updateLine = (key: string, field: keyof LineItem, value: any) => {
    setLines(lines.map((l) => {
      if (l.key !== key) return l;
      return { ...l, [field]: value };
    }));
  };

  const handleSubmit = async () => {
    try {
      await form.validateFields();
    } catch {
      return;
    }

    for (let i = 0; i < lines.length; i++) {
      if (!lines[i].purchaseQuantity || lines[i].purchaseQuantity <= 0) {
        message.error(`第${i + 1}行采购数量必须大于0`);
        return;
      }
    }

    const values = form.getFieldsValue();
    const items: CreateInboundOrderItemInput[] = lines.map((l) => ({
      seqNo: l.seqNo,
      materialCode: l.materialCode || undefined,
      materialName: l.materialName || undefined,
      purchaseQuantity: l.purchaseQuantity,
      batchNo: l.batchNo || undefined,
      batch1: l.batch1 || undefined,
      batch2: l.batch2 || undefined,
      batch3: l.batch3 || undefined,
      batch4: l.batch4 || undefined,
      batch5: l.batch5 || undefined,
      batch6: l.batch6 || undefined,
      batch7: l.batch7 || undefined,
      batch8: l.batch8 || undefined,
      batch9: l.batch9 || undefined,
      batch10: l.batch10 || undefined,
      reserveRemark1: l.reserveRemark1 || undefined,
      reserveRemark2: l.reserveRemark2 || undefined,
      reserveRemark3: l.reserveRemark3 || undefined,
      reserveRemark4: l.reserveRemark4 || undefined,
      reserveRemark5: l.reserveRemark5 || undefined,
      reserveRemark6: l.reserveRemark6 || undefined,
      reserveRemark7: l.reserveRemark7 || undefined,
      reserveRemark8: l.reserveRemark8 || undefined,
      reserveRemark9: l.reserveRemark9 || undefined,
      reserveRemark10: l.reserveRemark10 || undefined,
    }));

    setSubmitting(true);
    try {
      if (orderId) {
        const res = await inboundOrderApi.update({
          id: orderId,
          supplierName: values.supplierName,
          source: values.source,
          orderType: values.orderType,
          creator: values.creator,
          remark: values.remark,
          items,
        });
        if (res.success) {
          message.success('更新成功');
          onClose(true);
        } else {
          message.error(res.message || '更新失败');
        }
      } else {
        const res = await inboundOrderApi.create({
          supplierName: values.supplierName,
          source: values.source,
          orderType: values.orderType,
          creator: values.creator,
          remark: values.remark,
          items,
        });
        if (res.success) {
          message.success('创建成功');
          onClose(true);
        } else {
          message.error(res.message || '创建失败');
        }
      }
    } finally {
      setSubmitting(false);
    }
  };

  const lineColumns: ColumnsType<LineItem> = [
    { title: '序号', width: 60, render: (_, r) => r.seqNo },
    {
      title: '物料编码', width: 120,
      render: (_, record) => (
        <Input
          value={record.materialCode}
          onChange={(e) => updateLine(record.key, 'materialCode', e.target.value)}
        />
      ),
    },
    {
      title: '物料名称', width: 120,
      render: (_, record) => (
        <Input
          value={record.materialName}
          onChange={(e) => updateLine(record.key, 'materialName', e.target.value)}
        />
      ),
    },
    {
      title: '采购数量', width: 100,
      render: (_, record) => (
        <InputNumber
          value={record.purchaseQuantity}
          min={1}
          onChange={(v) => updateLine(record.key, 'purchaseQuantity', v || 1)}
          style={{ width: '100%' }}
        />
      ),
    },
    {
      title: '批次号', width: 100,
      render: (_, record) => (
        <Input
          value={record.batchNo}
          onChange={(e) => updateLine(record.key, 'batchNo', e.target.value)}
        />
      ),
    },
    {
      title: '批次1', width: 80,
      render: (_, record) => (
        <Input
          value={record.batch1}
          onChange={(e) => updateLine(record.key, 'batch1', e.target.value)}
          size="small"
        />
      ),
    },
    {
      title: '批次2', width: 80,
      render: (_, record) => (
        <Input
          value={record.batch2}
          onChange={(e) => updateLine(record.key, 'batch2', e.target.value)}
          size="small"
        />
      ),
    },
    {
      title: '批次3', width: 80,
      render: (_, record) => (
        <Input
          value={record.batch3}
          onChange={(e) => updateLine(record.key, 'batch3', e.target.value)}
          size="small"
        />
      ),
    },
    {
      title: '预留备注1', width: 100,
      render: (_, record) => (
        <Input
          value={record.reserveRemark1}
          onChange={(e) => updateLine(record.key, 'reserveRemark1', e.target.value)}
          size="small"
        />
      ),
    },
    {
      title: '预留备注2', width: 100,
      render: (_, record) => (
        <Input
          value={record.reserveRemark2}
          onChange={(e) => updateLine(record.key, 'reserveRemark2', e.target.value)}
          size="small"
        />
      ),
    },
    {
      title: '操作', width: 60, fixed: 'right',
      render: (_, record) => (
        <Button
          type="link"
          danger
          icon={<DeleteOutlined />}
          onClick={() => removeLine(record.key)}
        />
      ),
    },
  ];

  return (
    <Modal
      title={orderId ? '编辑入库单' : '新增入库单'}
      open={open}
      onCancel={() => onClose()}
      onOk={handleSubmit}
      confirmLoading={submitting}
      width={1800}
      destroyOnClose
    >
      <Spin spinning={loading}>
        <Form form={form} layout="vertical">
          <Space style={{ width: '100%' }} size="middle" align="start" wrap>
            <Form.Item name="supplierName" label="供应商名称" style={{ width: 300 }}>
              <Input placeholder="请输入供应商名称" />
            </Form.Item>
            <Form.Item name="source" label="来源" style={{ width: 200 }}>
              <Input placeholder="请输入来源" />
            </Form.Item>
            <Form.Item name="orderType" label="入库单类型" style={{ width: 200 }}>
              <Input placeholder="请输入入库单类型" />
            </Form.Item>
            <Form.Item name="creator" label="创建人" style={{ width: 200 }}>
              <Input placeholder="请输入创建人" />
            </Form.Item>
            <Form.Item name="remark" label="备注" style={{ width: 400 }}>
              <TextArea rows={1} placeholder="请输入备注" />
            </Form.Item>
          </Space>
        </Form>

        <div style={{ marginBottom: 8, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <strong>入库明细</strong>
          <Button type="dashed" icon={<PlusOutlined />} onClick={addLine}>添加行</Button>
        </div>

        <Table
          columns={lineColumns}
          dataSource={lines}
          rowKey="key"
          pagination={false}
          scroll={{ x: 2000 }}
          size="small"
        />
      </Spin>
    </Modal>
  );
}
