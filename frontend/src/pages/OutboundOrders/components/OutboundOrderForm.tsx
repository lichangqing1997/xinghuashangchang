import { useState, useEffect, useCallback } from 'react';
import { Modal, Form, Input, Table, Button, Select, InputNumber, DatePicker, Space, message, Spin } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { outboundOrderApi, productApi, locationApi } from '../../../api';
import type { Product, Location, CreateOutboundOrderItemInput } from '../../../types/models';

const { TextArea } = Input;

interface LineItem {
  key: string;
  productId: number | undefined;
  productName: string;
  productCode: string;
  specification: string;
  manufacturer: string;
  registrationNo: string;
  unitPrice: number;
  quantity: number;
  amount: number;
  expiryDate: dayjs.Dayjs | null;
  productionDate: dayjs.Dayjs | null;
  batchNo: string;
  locationId: number | undefined;
  companyName: string;
}

let lineKeySeq = 0;
function newLineKey() { return `line_${++lineKeySeq}`; }

interface Props {
  open: boolean;
  orderId: number | null;
  onClose: (refresh?: boolean) => void;
}

export default function OutboundOrderForm({ open, orderId, onClose }: Props) {
  const [form] = Form.useForm();
  const [lines, setLines] = useState<LineItem[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [productSearchLoading, setProductSearchLoading] = useState(false);

  const loadProducts = useCallback(async (keyword?: string) => {
    setProductSearchLoading(true);
    try {
      const res = await productApi.getList({ keyword, pageIndex: 1, pageSize: 50 });
      if (res.success && res.data) {
        setProducts(res.data.items);
      }
    } finally {
      setProductSearchLoading(false);
    }
  }, []);

  const loadLocations = useCallback(async () => {
    const res = await locationApi.getList({ pageIndex: 1, pageSize: 100 });
    if (res.success && res.data) {
      setLocations(res.data.items);
    }
  }, []);

  useEffect(() => {
    loadProducts();
    loadLocations();
  }, []);

  useEffect(() => {
    if (open && orderId) {
      loadOrderData();
    } else if (open) {
      form.resetFields();
      setLines([createEmptyLine()]);
    }
  }, [open, orderId]);

  const loadOrderData = async () => {
    if (!orderId) return;
    setLoading(true);
    try {
      const [orderRes, itemsRes] = await Promise.all([
        outboundOrderApi.getById(orderId),
        outboundOrderApi.getItems(orderId),
      ]);
      if (orderRes.success && orderRes.data) {
        form.setFieldsValue({
          companyName: orderRes.data.companyName,
          operator: orderRes.data.operator,
          remark: orderRes.data.remark,
        });
      }
      if (itemsRes.success && itemsRes.data) {
        setLines(itemsRes.data.map((item) => ({
          key: newLineKey(),
          productId: item.productId,
          productName: '',
          productCode: '',
          specification: item.specification || '',
          manufacturer: item.manufacturer || '',
          registrationNo: item.registrationNo || '',
          unitPrice: item.unitPrice,
          quantity: item.quantity,
          amount: item.amount,
          expiryDate: item.expiryDate ? dayjs(item.expiryDate) : null,
          productionDate: item.productionDate ? dayjs(item.productionDate) : null,
          batchNo: item.batchNo || '',
          locationId: item.locationId,
          companyName: item.companyName || '',
        })));
      }
    } finally {
      setLoading(false);
    }
  };

  function createEmptyLine(): LineItem {
    return {
      key: newLineKey(),
      productId: undefined,
      productName: '',
      productCode: '',
      specification: '',
      manufacturer: '',
      registrationNo: '',
      unitPrice: 0,
      quantity: 1,
      amount: 0,
      expiryDate: null,
      productionDate: null,
      batchNo: '',
      locationId: undefined,
      companyName: '',
    };
  }

  const addLine = () => {
    setLines([...lines, createEmptyLine()]);
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
      const updated = { ...l, [field]: value };
      if (field === 'unitPrice' || field === 'quantity') {
        updated.amount = (updated.unitPrice || 0) * (updated.quantity || 0);
      }
      return updated;
    }));
  };

  const handleProductSelect = (key: string, productId: number) => {
    const product = products.find((p) => p.id === productId);
    if (product) {
      setLines(lines.map((l) => {
        if (l.key !== key) return l;
        return {
          ...l,
          productId: product.id,
          productName: product.productName,
          productCode: product.productCode,
          specification: product.specification || '',
          manufacturer: product.manufacturer || '',
          registrationNo: product.registrationNo || '',
          unitPrice: product.salePrice,
          amount: product.salePrice * (l.quantity || 1),
          quantity: l.quantity || 1,
        };
      }));
    }
  };

  const totalAmount = lines.reduce((sum, l) => sum + (l.amount || 0), 0);

  const handleSubmit = async () => {
    try {
      await form.validateFields();
    } catch {
      return;
    }

    for (let i = 0; i < lines.length; i++) {
      if (!lines[i].productId) {
        message.error(`第${i + 1}行请选择商品`);
        return;
      }
      if (!lines[i].unitPrice || lines[i].unitPrice <= 0) {
        message.error(`第${i + 1}行销售单价必须大于0`);
        return;
      }
      if (!lines[i].quantity || lines[i].quantity <= 0) {
        message.error(`第${i + 1}行出库数量必须大于0`);
        return;
      }
    }

    const values = form.getFieldsValue();
    const items: CreateOutboundOrderItemInput[] = lines.map((l) => ({
      productId: l.productId!,
      unitPrice: l.unitPrice,
      quantity: l.quantity,
      expiryDate: l.expiryDate?.toISOString(),
      productionDate: l.productionDate?.toISOString(),
      batchNo: l.batchNo || undefined,
      locationId: l.locationId || undefined,
      companyName: l.companyName || undefined,
    }));

    setSubmitting(true);
    try {
      if (orderId) {
        const res = await outboundOrderApi.update({
          id: orderId,
          companyName: values.companyName,
          operator: values.operator,
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
        const res = await outboundOrderApi.create({
          companyName: values.companyName,
          operator: values.operator,
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
    {
      title: '商品', width: 200,
      render: (_, record) => (
        <Select
          showSearch
          placeholder="搜索商品"
          value={record.productId}
          onChange={(v) => handleProductSelect(record.key, v)}
          style={{ width: '100%' }}
          loading={productSearchLoading}
          filterOption={false}
          onSearch={(v) => loadProducts(v)}
          notFoundContent={productSearchLoading ? <Spin size="small" /> : '无匹配商品'}
        >
          {products.map((p) => (
            <Select.Option key={p.id} value={p.id}>
              {p.productCode} - {p.productName}
            </Select.Option>
          ))}
        </Select>
      ),
    },
    { title: '商品名', width: 120, render: (_, r) => r.productName || '-' },
    { title: '规格', width: 100, render: (_, r) => r.specification || '-' },
    {
      title: '销售单价', width: 120,
      render: (_, record) => (
        <InputNumber
          value={record.unitPrice}
          min={0}
          precision={2}
          onChange={(v) => updateLine(record.key, 'unitPrice', v || 0)}
          style={{ width: '100%' }}
        />
      ),
    },
    {
      title: '出库数量', width: 100,
      render: (_, record) => (
        <InputNumber
          value={record.quantity}
          min={1}
          onChange={(v) => updateLine(record.key, 'quantity', v || 1)}
          style={{ width: '100%' }}
        />
      ),
    },
    { title: '出库总价', width: 100, render: (_, r) => `¥${(r.amount || 0).toFixed(2)}` },
    { title: '生产厂家', width: 120, render: (_, r) => r.manufacturer || '-' },
    {
      title: '有效期', width: 140,
      render: (_, record) => (
        <DatePicker
          value={record.expiryDate}
          onChange={(d) => updateLine(record.key, 'expiryDate', d)}
          style={{ width: '100%' }}
        />
      ),
    },
    {
      title: '生产日期', width: 140,
      render: (_, record) => (
        <DatePicker
          value={record.productionDate}
          onChange={(d) => updateLine(record.key, 'productionDate', d)}
          style={{ width: '100%' }}
        />
      ),
    },
    {
      title: '批号', width: 120,
      render: (_, record) => (
        <Input
          value={record.batchNo}
          onChange={(e) => updateLine(record.key, 'batchNo', e.target.value)}
        />
      ),
    },
    { title: '注册证号', width: 120, render: (_, r) => r.registrationNo || '-' },
    {
      title: '库位', width: 150,
      render: (_, record) => (
        <Select
          showSearch
          placeholder="选择库位"
          value={record.locationId}
          onChange={(v) => updateLine(record.key, 'locationId', v)}
          style={{ width: '100%' }}
          allowClear
          optionFilterProp="children"
        >
          {locations.map((loc) => (
            <Select.Option key={loc.id} value={loc.id}>
              {loc.locationCode} - {loc.locationName}
            </Select.Option>
          ))}
        </Select>
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
      title={orderId ? '编辑出库单' : '新增出库单'}
      open={open}
      onCancel={() => onClose()}
      onOk={handleSubmit}
      confirmLoading={submitting}
      width={1600}
      destroyOnClose
    >
      <Spin spinning={loading}>
        <Form form={form} layout="vertical">
          <Space style={{ width: '100%' }} size="middle" align="start" wrap>
            <Form.Item name="companyName" label="收货公司名" style={{ width: 300 }}>
              <Input placeholder="请输入收货公司名" />
            </Form.Item>
            <Form.Item name="operator" label="制单人" style={{ width: 200 }}>
              <Input placeholder="请输入制单人" />
            </Form.Item>
            <Form.Item name="remark" label="备注" style={{ width: 400 }}>
              <TextArea rows={1} placeholder="请输入备注" />
            </Form.Item>
          </Space>
        </Form>

        <div style={{ marginBottom: 8, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <strong>出库明细</strong>
          <Button type="dashed" icon={<PlusOutlined />} onClick={addLine}>添加行</Button>
        </div>

        <Table
          columns={lineColumns}
          dataSource={lines}
          rowKey="key"
          pagination={false}
          scroll={{ x: 1800 }}
          size="small"
          footer={() => (
            <div style={{ textAlign: 'right', fontWeight: 'bold' }}>
              合计金额: ¥{totalAmount.toFixed(2)}
            </div>
          )}
        />
      </Spin>
    </Modal>
  );
}
