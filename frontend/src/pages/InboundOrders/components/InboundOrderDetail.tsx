import { useState, useEffect } from 'react';
import { Modal, Descriptions, Table, Tag, Spin } from 'antd';
import type { ColumnsType } from 'antd/es/table';
import dayjs from 'dayjs';
import { inboundOrderApi } from '../../../api';
import type { InboundOrder, InboundOrderItem } from '../../../types/models';

const statusColorMap: Record<string, string> = {
  '未处理': 'default',
  '正在处理': 'processing',
  '已完成': 'success',
  '手动关闭': 'error',
};

const auditStatusColorMap: Record<string, string> = {
  '待审核': 'warning',
  '已通过': 'success',
  '已驳回': 'error',
};

interface Props {
  open: boolean;
  orderId: number;
  onClose: () => void;
}

export default function InboundOrderDetail({ open, orderId, onClose }: Props) {
  const [order, setOrder] = useState<InboundOrder | null>(null);
  const [items, setItems] = useState<InboundOrderItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open && orderId) {
      loadData();
    }
  }, [open, orderId]);

  const loadData = async () => {
    setLoading(true);
    try {
      const [orderRes, itemsRes] = await Promise.all([
        inboundOrderApi.getById(orderId),
        inboundOrderApi.getItems(orderId),
      ]);
      if (orderRes.success && orderRes.data) {
        setOrder(orderRes.data);
      }
      if (itemsRes.success && itemsRes.data) {
        setItems(itemsRes.data);
      }
    } finally {
      setLoading(false);
    }
  };

  const itemColumns: ColumnsType<InboundOrderItem> = [
    { title: '序号', dataIndex: 'seqNo', width: 60 },
    { title: '物料编码', dataIndex: 'materialCode', width: 120, render: (v) => v || '-' },
    { title: '物料名称', dataIndex: 'materialName', width: 120, render: (v) => v || '-' },
    { title: '采购数量', dataIndex: 'purchaseQuantity', width: 100 },
    { title: '批次号', dataIndex: 'batchNo', width: 100, render: (v) => v || '-' },
    { title: '批次1', dataIndex: 'batch1', width: 80, render: (v) => v || '-' },
    { title: '批次2', dataIndex: 'batch2', width: 80, render: (v) => v || '-' },
    { title: '批次3', dataIndex: 'batch3', width: 80, render: (v) => v || '-' },
    { title: '批次4', dataIndex: 'batch4', width: 80, render: (v) => v || '-' },
    { title: '批次5', dataIndex: 'batch5', width: 80, render: (v) => v || '-' },
    { title: '预留备注1', dataIndex: 'reserveRemark1', width: 100, render: (v) => v || '-' },
    { title: '预留备注2', dataIndex: 'reserveRemark2', width: 100, render: (v) => v || '-' },
    { title: '预留备注3', dataIndex: 'reserveRemark3', width: 100, render: (v) => v || '-' },
  ];

  return (
    <Modal
      title="入库单详情"
      open={open}
      onCancel={onClose}
      footer={null}
      width={1400}
      destroyOnClose
    >
      <Spin spinning={loading}>
        {order && (
          <>
            <Descriptions bordered column={3} size="small" style={{ marginBottom: 16 }}>
              <Descriptions.Item label="入库单编号">{order.orderNo}</Descriptions.Item>
              <Descriptions.Item label="供应商名称">{order.supplierName || '-'}</Descriptions.Item>
              <Descriptions.Item label="来源">{order.source || '-'}</Descriptions.Item>
              <Descriptions.Item label="入库单类型">{order.orderType || '-'}</Descriptions.Item>
              <Descriptions.Item label="状态">
                <Tag color={statusColorMap[order.status] || 'default'}>{order.status}</Tag>
              </Descriptions.Item>
              <Descriptions.Item label="审核状态">
                <Tag color={auditStatusColorMap[order.auditStatus] || 'default'}>{order.auditStatus}</Tag>
              </Descriptions.Item>
              <Descriptions.Item label="创建人">{order.creator || '-'}</Descriptions.Item>
              <Descriptions.Item label="创建时间">
                {order.createdAt ? dayjs(order.createdAt).format('YYYY-MM-DD HH:mm') : '-'}
              </Descriptions.Item>
              <Descriptions.Item label="完成时间">
                {order.completedAt ? dayjs(order.completedAt).format('YYYY-MM-DD HH:mm') : '-'}
              </Descriptions.Item>
              {order.auditor && (
                <Descriptions.Item label="审核人">{order.auditor}</Descriptions.Item>
              )}
              {order.auditTime && (
                <Descriptions.Item label="审核时间">
                  {dayjs(order.auditTime).format('YYYY-MM-DD HH:mm')}
                </Descriptions.Item>
              )}
              {order.auditRemark && (
                <Descriptions.Item label="审核备注" span={2}>{order.auditRemark}</Descriptions.Item>
              )}
              {order.remark && (
                <Descriptions.Item label="备注" span={3}>{order.remark}</Descriptions.Item>
              )}
            </Descriptions>

            <div style={{ marginBottom: 8, fontWeight: 'bold' }}>入库明细</div>
            <Table
              columns={itemColumns}
              dataSource={items}
              rowKey="id"
              pagination={false}
              scroll={{ x: 1500 }}
              size="small"
            />
          </>
        )}
      </Spin>
    </Modal>
  );
}
