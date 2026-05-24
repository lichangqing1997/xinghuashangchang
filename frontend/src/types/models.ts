// Product
export interface Product {
  id: number;
  productCode: string;
  productName: string;
  barcode: string;
  SKU: string;
  category: string;
  color?: string;
  size?: string;
  specification?: string;
  manufacturer?: string;
  registrationNo?: string;
  purchasePrice: number;
  salePrice: number;
  unit: string;
  supplierId?: number;
  status: number;
  createdAt: string;
  updatedAt?: string;
}

// Supplier
export interface Supplier {
  id: number;
  supplierCode: string;
  supplierName: string;
  contactPerson?: string;
  contactPhone?: string;
  address?: string;
  remark?: string;
  status: number;
  createdAt: string;
  updatedAt?: string;
}

// Location
export interface Location {
  id: number;
  locationCode: string;
  locationName: string;
  warehouse?: string;
  shelf?: string;
  floor?: number;
  position?: number;
  status: string;
  createdAt: string;
}

// Inventory
export interface Inventory {
  id: number;
  productId: number;
  locationId: number;
  quantity: number;
  lockedQuantity: number;
  availableQuantity: number;
  createdAt: string;
  updatedAt?: string;
}

// ScanRecord
export interface ScanRecord {
  id: number;
  barcode: string;
  productId: number;
  operationType: string;
  locationId?: number;
  quantity: number;
  referenceNo?: string;
  operator?: string;
  scanTime: string;
}

// PurchaseOrder
export interface PurchaseOrder {
  id: number;
  orderNo: string;
  supplierId: number;
  totalAmount: number;
  status: string;
  remark?: string;
  createdAt: string;
  receivedAt?: string;
  stockedAt?: string;
}

// PurchaseOrderItem
export interface PurchaseOrderItem {
  id: number;
  purchaseOrderId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  amount: number;
  receivedQuantity: number;
  stockedQuantity: number;
  createdAt: string;
}

// SalesOrder
export interface SalesOrder {
  id: number;
  orderNo: string;
  customerName?: string;
  customerPhone?: string;
  totalAmount: number;
  paidAmount: number;
  status: string;
  remark?: string;
  createdAt: string;
  shippedAt?: string;
  completedAt?: string;
}

// SalesOrderItem
export interface SalesOrderItem {
  id: number;
  salesOrderId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  amount: number;
  shippedQuantity: number;
  createdAt: string;
}

// PDA Scan In Input
export interface PdaScanInInput {
  barcode: string;
  locationCode: string;
  quantity: number;
  purchaseOrderNo?: string;
  operator?: string;
}

// PDA Scan Out Input
export interface PdaScanOutInput {
  barcode: string;
  quantity: number;
  salesOrderNo?: string;
  operator?: string;
}

// PDA Shelf Input
export interface PdaShelfInput {
  barcode: string;
  locationCode: string;
  quantity: number;
  operator?: string;
}

// Mobile Receive Input
export interface MobileReceiveInput {
  purchaseOrderNo: string;
  barcode?: string;
  quantity: number;
  operator?: string;
}

// User
export interface User {
  id: number;
  username: string;
  name: string;
  role: string;
  token?: string;
}

// Login Input
export interface LoginInput {
  username: string;
  password: string;
}

// ===== 用户管理相关类型 =====

// 角色输出
export interface RoleOutput {
  id: number;
  roleCode: string;
  roleName: string;
}

// 用户详情输出
export interface UserDetail {
  id: number;
  username: string;
  realName?: string;
  phone?: string;
  email?: string;
  avatar?: string;
  status: number;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
  lastLoginAt?: string;
  roles: RoleOutput[];
  roleIds: number[];
}

// 创建用户输入
export interface CreateUserInput {
  username: string;
  password: string;
  realName?: string;
  phone?: string;
  email?: string;
  status: number;
  remark?: string;
  roleIds: number[];
}

// 更新用户输入
export interface UpdateUserInput {
  id: number;
  realName?: string;
  phone?: string;
  email?: string;
  status: number;
  remark?: string;
  roleIds: number[];
}

// 修改密码输入
export interface ChangePasswordInput {
  userId: number;
  oldPassword: string;
  newPassword: string;
}

// ===== 角色管理相关类型 =====

// 角色
export interface Role {
  id: number;
  roleCode: string;
  roleName: string;
  sortOrder: number;
  status: number;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
}

// 角色详情输出
export interface RoleDetail {
  id: number;
  roleCode: string;
  roleName: string;
  sortOrder: number;
  status: number;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
  menuIds: number[];
}

// 创建角色输入
export interface CreateRoleInput {
  roleCode: string;
  roleName: string;
  sortOrder: number;
  status: number;
  remark?: string;
  menuIds: number[];
}

// 更新角色输入
export interface UpdateRoleInput {
  id: number;
  roleName: string;
  sortOrder: number;
  status: number;
  remark?: string;
  menuIds: number[];
}

// ===== 菜单管理相关类型 =====

// 菜单
export interface Menu {
  id: number;
  parentId: number;
  menuName: string;
  menuCode: string;
  path?: string;
  icon?: string;
  menuType: number;
  permission?: string;
  sortOrder: number;
  status: number;
  isVisible: number;
  createdAt: string;
  updatedAt?: string;
}

// 菜单树输出
export interface MenuTree {
  id: number;
  parentId: number;
  menuName: string;
  menuCode: string;
  path?: string;
  icon?: string;
  menuType: number;
  permission?: string;
  sortOrder: number;
  status: number;
  isVisible: number;
  children: MenuTree[];
}

// 创建菜单输入
export interface CreateMenuInput {
  parentId: number;
  menuName: string;
  menuCode: string;
  path?: string;
  icon?: string;
  menuType: number;
  permission?: string;
  sortOrder: number;
  status: number;
  isVisible: number;
}

// 更新菜单输入
export interface UpdateMenuInput {
  id: number;
  parentId: number;
  menuName: string;
  path?: string;
  icon?: string;
  menuType: number;
  permission?: string;
  sortOrder: number;
  status: number;
  isVisible: number;
}

// ===== 文件管理相关类型 =====

// 文件附件
export interface FileAttachment {
  id: number;
  fileName: string;
  storedFileName: string;
  filePath: string;
  contentType: string;
  fileSize: number;
  fileType: string;
  businessType?: string;
  businessId?: number;
  version?: string;
  versionCode?: number;
  status: number;
  createdAt: string;
  createdBy?: string;
}

// ===== 出库单管理相关类型 =====

// 出库单
export interface OutboundOrder {
  id: number;
  orderNo: string;
  companyName?: string;
  totalAmount: number;
  status: string;
  operator?: string;
  remark?: string;
  createdAt: string;
  outboundAt?: string;
  updatedAt?: string;
}

// 出库单明细
export interface OutboundOrderItem {
  id: number;
  outboundOrderId: number;
  productId: number;
  specification?: string;
  unitPrice: number;
  quantity: number;
  amount: number;
  manufacturer?: string;
  expiryDate?: string;
  productionDate?: string;
  batchNo?: string;
  registrationNo?: string;
  locationId?: number;
  companyName?: string;
  createdAt: string;
}

// 出库流水
export interface OutboundFlow {
  id: number;
  orderNo: string;
  productId: number;
  productName?: string;
  productCode?: string;
  locationId: number;
  locationCode?: string;
  quantity: number;
  unitPrice: number;
  amount: number;
  batchNo?: string;
  operator?: string;
  flowTime: string;
}

// 创建出库单输入
export interface CreateOutboundOrderInput {
  companyName?: string;
  remark?: string;
  operator?: string;
  items: CreateOutboundOrderItemInput[];
}

// 创建出库单明细输入
export interface CreateOutboundOrderItemInput {
  productId: number;
  unitPrice: number;
  quantity: number;
  expiryDate?: string;
  productionDate?: string;
  batchNo?: string;
  locationId?: number;
  companyName?: string;
}

// 更新出库单输入
export interface UpdateOutboundOrderInput {
  id: number;
  companyName?: string;
  remark?: string;
  operator?: string;
  items: CreateOutboundOrderItemInput[];
}

// 出库单查询输入
export interface OutboundOrderQueryInput {
  keyword?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
  pageIndex?: number;
  pageSize?: number;
}

// ===== 公司管理相关类型 =====

// 公司
export interface Company {
  id: number;
  companyName: string;
  legalPerson?: string;
  businessLicenseNo?: string;
  companyCode?: string;
  status: number;
  createdAt: string;
  updatedAt?: string;
}

// 公司扩展信息
export interface CompanyDetail {
  id: number;
  companyId: number;
  fieldName: string;
  fieldValue?: string;
  createdAt: string;
}

// 创建公司输入
export interface CreateCompanyInput {
  companyName: string;
  legalPerson?: string;
  businessLicenseNo?: string;
  companyCode?: string;
  details?: CompanyDetailInput[];
}

// 更新公司输入
export interface UpdateCompanyInput {
  id: number;
  companyName: string;
  legalPerson?: string;
  businessLicenseNo?: string;
  companyCode?: string;
  status?: number;
  details?: CompanyDetailInput[];
}

// 公司扩展信息输入
export interface CompanyDetailInput {
  fieldName: string;
  fieldValue?: string;
}
