# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

兴华商城是一个服装类仓储管理系统，提供商品管理、进货、销售、库存管理和 PDA 扫码出入库功能。

## 技术栈

### 后端
- **框架**: ASP.NET Core 6.0 (C#)
- **ORM**: SqlSugarCore 5.1.4.214
- **数据库**: MySQL
- **API 文档**: Swagger (Swashbuckle.AspNetCore)

### 前端
- **框架**: React 18 + TypeScript
- **构建工具**: Vite
- **UI 组件库**: Ant Design
- **状态管理**: React Query
- **路由**: React Router v6
- **HTTP 客户端**: Axios

## 常用命令

### 后端命令

| 命令 | 用途 |
|------|------|
| `dotnet build XingHuaShangChang.sln` | 构建整个解决方案 |
| `dotnet run --project XingHuaShangChang.Web.UI` | 启动开发服务器 (http://localhost:5242) |
| `dotnet restore` | 还原 NuGet 包 |
| `dotnet publish` | 发布生产版本 |

### 前端命令

| 命令 | 用途 |
|------|------|
| `cd frontend && npm install` | 安装前端依赖 |
| `cd frontend && npm run dev` | 启动前端开发服务器 (http://localhost:3000) |
| `cd frontend && npm run build` | 构建生产版本 |
| `cd frontend && npm run preview` | 预览生产构建 |

## 项目架构

采用前后端分离架构：

```
xinghuashangchang/
├── XingHuaShangChang.sln              # .NET 解决方案文件
├── XingHuaShangChang.Web.UI/          # 后端 - API 控制器和配置
│   ├── Controllers/                   # API 端点
│   ├── Program.cs                     # 应用入口
│   └── appsettings.json               # 配置文件（含数据库连接字符串）
├── XingHuaShangChang.Application/     # 后端 - 业务逻辑层
│   ├── Const/                         # 状态常量
│   ├── Dto/                           # 数据传输对象
│   ├── Entity/                        # 数据库实体（中文命名）
│   └── Services/                      # 业务服务
├── XingHuaShangChang.Common/          # 后端 - 公共工具层
│   └── Util/Result.cs                 # 统一返回结果封装
└── frontend/                          # 前端 - React 应用
    ├── src/
    │   ├── api/                       # API 服务层
    │   ├── components/                # 可复用组件
    │   ├── layouts/                   # 布局组件
    │   ├── pages/                     # 页面组件
    │   │   ├── Login/                 # 登录页面
    │   │   ├── Dashboard/             # 首页仪表盘
    │   │   ├── Products/              # 商品管理
    │   │   ├── Inventory/             # 库存管理
    │   │   ├── PDA/                   # PDA 扫码
    │   │   └── Suppliers/             # 供应商管理
    │   ├── stores/                    # 状态管理
    │   ├── types/                     # TypeScript 类型定义
    │   └── utils/                     # 工具函数
    ├── package.json                   # 依赖配置
    └── vite.config.ts                 # Vite 配置
```

## 代码规范

### 后端命名约定
- **实体类和属性**: 使用中文命名（如 `商品`、`商品编码`），通过 SqlSugar 的 `[SugarTable]` 和 `[SugarColumn]` 特性映射
- **服务类**: 中文名 + Service 后缀（如 `商品Service`、`PdaService`）
- **Controller**: 中文名 + Controller 后缀（如 `商品Controller`）
- **DTO**: 描述性中文类名（如 `扫码入库输入`）

### 前端命名约定
- **组件**: PascalCase（如 `MainLayout`、`DashboardPage`）
- **文件夹**: kebab-case（如 `api`、`pages`）
- **类型定义**: 与后端实体保持一致的中文命名

### API 路由
- 路由前缀: `/api/`
- 使用中文路由段（如 `/api/商品/list`、`/api/PDA/scan-in`）

### 依赖注入
- 服务注册为 Scoped 生命周期（在 `Program.cs` 中配置）
- SqlSugar 客户端注册为 Singleton

### 统一返回格式
所有 API 返回 `Result` 对象（定义在 `XingHuaShangChang.Common/Util/Result.cs`），包含：
- `Success`: 布尔值，表示操作是否成功
- `ErrorCode`: 错误码（可选）
- `Data`: 返回数据
- `Message`: 错误信息（失败时）
- `Timestamp`: 时间戳

## 核心业务模块

| 模块 | 功能 | API 前缀 |
|------|------|----------|
| 基础数据 | 商品、供应商、库位管理 | `/api/商品`、`/api/供应商`、`/api/库位` |
| PDA | 扫码入库、出库、上架 | `/api/PDA` |
| 进货 | 进货单管理（待实现） | - |
| 销售 | 销售单管理（待实现） | - |

## 前端页面

| 页面 | 路由 | 功能 |
|------|------|------|
| 登录页 | `/login` | 用户登录认证 |
| 首页 | `/` | 仪表盘，显示统计数据和快捷操作 |
| 商品管理 | `/products` | 商品 CRUD 操作 |
| 库存管理 | `/inventory` | 库存查询和统计 |
| PDA 扫码 | `/pda` | 扫码入库/出库操作 |
| 供应商管理 | `/suppliers` | 供应商 CRUD 操作 |

## 数据库

- 连接字符串在 `appsettings.json` 的 `ConnectionStrings:MySql`
- 使用 SqlSugar 的 `SqlSugarScope` 管理数据库连接
- 启用 `IsAutoCloseConnection` 自动关闭连接
- 使用 `InitKeyType.Attribute` 从实体特性读取表结构
- **自动初始化**: 应用启动时自动创建数据库、表结构和种子数据（见 `DatabaseInitializer.cs`）

## 开发流程

1. **启动后端**: `dotnet run --project XingHuaShangChang.Web.UI`
2. **启动前端**: `cd frontend && npm run dev`
3. **访问应用**: http://localhost:3000
4. **API 文档**: http://localhost:5242/swagger
