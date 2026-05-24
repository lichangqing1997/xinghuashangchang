using SqlSugar;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.FileService;
using XingHuaShangChang.Application.Services.OutboundOrderService;

namespace XingHuaShangChang.Web.UI;

public static class DatabaseInitializer
{
    public static void Initialize(ISqlSugarClient db)
    {
        db.DbMaintenance.CreateDatabase();

        db.CodeFirst.InitTables(
            typeof(Product),
            typeof(Supplier),
            typeof(Location),
            typeof(PurchaseOrder),
            typeof(PurchaseOrderItem),
            typeof(Inventory),
            typeof(SalesOrder),
            typeof(SalesOrderItem),
            typeof(ScanRecord),
            typeof(User),
            typeof(Role),
            typeof(Menu),
            typeof(UserRole),
            typeof(RoleMenu),
            typeof(FileAttachment),
            typeof(OutboundOrder),
            typeof(OutboundOrderItem),
            typeof(OutboundFlow),
            typeof(Company),
            typeof(CompanyDetail)
        );

        SeedData(db);
    }

    private static void SeedData(ISqlSugarClient db)
    {
        if (!db.Queryable<Supplier>().Any())
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { SupplierCode = "SUP001", SupplierName = "广州时尚服饰有限公司", ContactPerson = "张经理", ContactPhone = "13800138001", Address = "广州市天河区服装城A区101", Remark = "", Status = 1, UpdatedAt = DateTime.UtcNow },
                new Supplier { SupplierCode = "SUP002", SupplierName = "深圳潮流服饰工厂", ContactPerson = "李总", ContactPhone = "13800138002", Address = "深圳市南山区工业园B栋", Remark = "", Status = 1, UpdatedAt = DateTime.UtcNow },
                new Supplier { SupplierCode = "SUP003", SupplierName = "东莞制衣厂", ContactPerson = "王主管", ContactPhone = "13800138003", Address = "东莞市长安镇工业区", Remark = "", Status = 1, UpdatedAt = DateTime.UtcNow }
            };
            db.Insertable(suppliers).ExecuteCommand();
        }

        if (!db.Queryable<Location>().Any())
        {
            var locations = new List<Location>
            {
                new Location { LocationCode = "A-01-01-01", LocationName = "A区1排1层1位", Warehouse = "主仓库", Shelf = "A-01", Floor = 1, Position = 1, Status = "空闲" },
                new Location { LocationCode = "A-01-01-02", LocationName = "A区1排1层2位", Warehouse = "主仓库", Shelf = "A-01", Floor = 1, Position = 2, Status = "空闲" },
                new Location { LocationCode = "A-01-02-01", LocationName = "A区1排2层1位", Warehouse = "主仓库", Shelf = "A-01", Floor = 2, Position = 1, Status = "空闲" },
                new Location { LocationCode = "A-02-01-01", LocationName = "A区2排1层1位", Warehouse = "主仓库", Shelf = "A-02", Floor = 1, Position = 1, Status = "空闲" },
                new Location { LocationCode = "B-01-01-01", LocationName = "B区1排1层1位", Warehouse = "主仓库", Shelf = "B-01", Floor = 1, Position = 1, Status = "空闲" },
                new Location { LocationCode = "B-01-01-02", LocationName = "B区1排1层2位", Warehouse = "主仓库", Shelf = "B-01", Floor = 1, Position = 2, Status = "空闲" }
            };
            db.Insertable(locations).ExecuteCommand();
        }

        if (!db.Queryable<Product>().Any())
        {
            var now = DateTime.UtcNow;
            var products = new List<Product>
            {
                new Product { ProductCode = "PROD001", ProductName = "男士休闲T恤", Barcode = "6901234567890", SKU = "T恤-男-白色-L", Category = "T恤", Color = "白色", Size = "L", PurchasePrice = 45.00m, SalePrice = 89.00m, Unit = "件", SupplierId = 1, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD002", ProductName = "男士休闲T恤", Barcode = "6901234567891", SKU = "T恤-男-黑色-M", Category = "T恤", Color = "黑色", Size = "M", PurchasePrice = 45.00m, SalePrice = 89.00m, Unit = "件", SupplierId = 1, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD003", ProductName = "女士连衣裙", Barcode = "6901234567892", SKU = "连衣裙-女-红色-S", Category = "连衣裙", Color = "红色", Size = "S", PurchasePrice = 68.00m, SalePrice = 138.00m, Unit = "件", SupplierId = 2, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD004", ProductName = "女士连衣裙", Barcode = "6901234567893", SKU = "连衣裙-女-蓝色-M", Category = "连衣裙", Color = "蓝色", Size = "M", PurchasePrice = 68.00m, SalePrice = 138.00m, Unit = "件", SupplierId = 2, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD005", ProductName = "男士牛仔裤", Barcode = "6901234567894", SKU = "牛仔裤-男-蓝色-32", Category = "裤子", Color = "蓝色", Size = "32", PurchasePrice = 75.00m, SalePrice = 159.00m, Unit = "条", SupplierId = 3, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD006", ProductName = "女士休闲裤", Barcode = "6901234567895", SKU = "休闲裤-女-黑色-S", Category = "裤子", Color = "黑色", Size = "S", PurchasePrice = 55.00m, SalePrice = 119.00m, Unit = "条", SupplierId = 3, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD007", ProductName = "儿童运动鞋", Barcode = "6901234567896", SKU = "运动鞋-童-白色-30", Category = "鞋子", Color = "白色", Size = "30", PurchasePrice = 60.00m, SalePrice = 129.00m, Unit = "双", SupplierId = 1, Status = 1, UpdatedAt = now },
                new Product { ProductCode = "PROD008", ProductName = "男士皮鞋", Barcode = "6901234567897", SKU = "皮鞋-男-黑色-42", Category = "鞋子", Color = "黑色", Size = "42", PurchasePrice = 120.00m, SalePrice = 259.00m, Unit = "双", SupplierId = 2, Status = 1, UpdatedAt = now }
            };
            db.Insertable(products).ExecuteCommand();
        }

        if (!db.Queryable<Inventory>().Any())
        {
            var now = DateTime.UtcNow;
            var stocks = new List<Inventory>
            {
                new Inventory { ProductId = 1, LocationId = 1, Quantity = 100, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 2, LocationId = 2, Quantity = 80, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 3, LocationId = 3, Quantity = 50, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 4, LocationId = 4, Quantity = 60, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 5, LocationId = 5, Quantity = 45, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 6, LocationId = 6, Quantity = 70, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 7, LocationId = 1, Quantity = 30, LockedQuantity = 0, UpdatedAt = now },
                new Inventory { ProductId = 8, LocationId = 2, Quantity = 25, LockedQuantity = 0, UpdatedAt = now }
            };
            db.Insertable(stocks).ExecuteCommand();
        }

        // 初始化菜单数据
        if (!db.Queryable<Menu>().Any())
        {
            var menus = new List<Menu>
            {
                // 一级菜单（目录）
                new Menu { Id = 1, ParentId = 0, MenuName = "首页", MenuCode = "dashboard", Path = "/", Icon = "DashboardOutlined", MenuType = 1, SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 2, ParentId = 0, MenuName = "基础数据", MenuCode = "basic_data", Path = "", Icon = "DatabaseOutlined", MenuType = 0, SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 3, ParentId = 0, MenuName = "库存管理", MenuCode = "inventory", Path = "", Icon = "InboxOutlined", MenuType = 0, SortOrder = 2, Status = 1, IsVisible = 1 },
                new Menu { Id = 4, ParentId = 0, MenuName = "PDA操作", MenuCode = "pda", Path = "", Icon = "ScanOutlined", MenuType = 0, SortOrder = 3, Status = 1, IsVisible = 1 },
                new Menu { Id = 5, ParentId = 0, MenuName = "系统管理", MenuCode = "system", Path = "", Icon = "SettingOutlined", MenuType = 0, SortOrder = 10, Status = 1, IsVisible = 1 },

                // 基础数据子菜单
                new Menu { Id = 6, ParentId = 2, MenuName = "商品管理", MenuCode = "product_manage", Path = "/products", Icon = "ShoppingOutlined", MenuType = 1, Permission = "product:view", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 7, ParentId = 2, MenuName = "供应商管理", MenuCode = "supplier_manage", Path = "/suppliers", Icon = "TeamOutlined", MenuType = 1, Permission = "supplier:view", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 30, ParentId = 2, MenuName = "公司管理", MenuCode = "company_manage", Path = "/companies", Icon = "BankOutlined", MenuType = 1, Permission = "company:view", SortOrder = 2, Status = 1, IsVisible = 1 },

                // 库存管理子菜单
                new Menu { Id = 8, ParentId = 3, MenuName = "库存查询", MenuCode = "inventory_query", Path = "/inventory", Icon = "UnorderedListOutlined", MenuType = 1, Permission = "inventory:view", SortOrder = 0, Status = 1, IsVisible = 1 },

                // 出库管理目录
                new Menu { Id = 29, ParentId = 0, MenuName = "出库管理", MenuCode = "outbound", Path = "", Icon = "ExportOutlined", MenuType = 0, SortOrder = 3, Status = 1, IsVisible = 1 },
                new Menu { Id = 23, ParentId = 29, MenuName = "出库单管理", MenuCode = "outbound_order_manage", Path = "/outbound-orders", Icon = "FileTextOutlined", MenuType = 1, Permission = "outbound:view", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 24, ParentId = 29, MenuName = "出库流水", MenuCode = "outbound_flow", Path = "/outbound-flows", Icon = "TransactionOutlined", MenuType = 1, Permission = "outbound:flow", SortOrder = 1, Status = 1, IsVisible = 1 },

                // PDA操作子菜单
                new Menu { Id = 9, ParentId = 4, MenuName = "扫码操作", MenuCode = "pda_scan", Path = "/pda", Icon = "QrcodeOutlined", MenuType = 1, Permission = "pda:scan", SortOrder = 0, Status = 1, IsVisible = 1 },

                // 系统管理子菜单
                new Menu { Id = 10, ParentId = 5, MenuName = "用户管理", MenuCode = "user_manage", Path = "/system/users", Icon = "UserOutlined", MenuType = 1, Permission = "user:view", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 11, ParentId = 5, MenuName = "角色管理", MenuCode = "role_manage", Path = "/system/roles", Icon = "SafetyOutlined", MenuType = 1, Permission = "role:view", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 12, ParentId = 5, MenuName = "菜单管理", MenuCode = "menu_manage", Path = "/system/menus", Icon = "MenuOutlined", MenuType = 1, Permission = "menu:view", SortOrder = 2, Status = 1, IsVisible = 1 },
                new Menu { Id = 22, ParentId = 5, MenuName = "文件管理", MenuCode = "file_manage", Path = "/system/files", Icon = "FileOutlined", MenuType = 1, Permission = "file:view", SortOrder = 3, Status = 1, IsVisible = 1 },

                // 按钮权限
                new Menu { Id = 13, ParentId = 6, MenuName = "商品新增", MenuCode = "product_add", MenuType = 2, Permission = "product:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 14, ParentId = 6, MenuName = "商品编辑", MenuCode = "product_edit", MenuType = 2, Permission = "product:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 15, ParentId = 6, MenuName = "商品删除", MenuCode = "product_delete", MenuType = 2, Permission = "product:delete", SortOrder = 2, Status = 1, IsVisible = 1 },
                new Menu { Id = 16, ParentId = 10, MenuName = "用户新增", MenuCode = "user_add", MenuType = 2, Permission = "user:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 17, ParentId = 10, MenuName = "用户编辑", MenuCode = "user_edit", MenuType = 2, Permission = "user:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 18, ParentId = 10, MenuName = "用户删除", MenuCode = "user_delete", MenuType = 2, Permission = "user:delete", SortOrder = 2, Status = 1, IsVisible = 1 },
                new Menu { Id = 19, ParentId = 11, MenuName = "角色新增", MenuCode = "role_add", MenuType = 2, Permission = "role:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 20, ParentId = 11, MenuName = "角色编辑", MenuCode = "role_edit", MenuType = 2, Permission = "role:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 21, ParentId = 11, MenuName = "角色删除", MenuCode = "role_delete", MenuType = 2, Permission = "role:delete", SortOrder = 2, Status = 1, IsVisible = 1 },

                // 出库单按钮权限
                new Menu { Id = 25, ParentId = 23, MenuName = "出库单新增", MenuCode = "outbound_add", MenuType = 2, Permission = "outbound:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 26, ParentId = 23, MenuName = "出库单编辑", MenuCode = "outbound_edit", MenuType = 2, Permission = "outbound:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 27, ParentId = 23, MenuName = "出库单删除", MenuCode = "outbound_delete", MenuType = 2, Permission = "outbound:delete", SortOrder = 2, Status = 1, IsVisible = 1 },
                new Menu { Id = 28, ParentId = 23, MenuName = "出库确认", MenuCode = "outbound_confirm", MenuType = 2, Permission = "outbound:confirm", SortOrder = 3, Status = 1, IsVisible = 1 },

                // 公司管理按钮权限
                new Menu { Id = 31, ParentId = 30, MenuName = "公司新增", MenuCode = "company_add", MenuType = 2, Permission = "company:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { Id = 32, ParentId = 30, MenuName = "公司编辑", MenuCode = "company_edit", MenuType = 2, Permission = "company:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                new Menu { Id = 33, ParentId = 30, MenuName = "公司删除", MenuCode = "company_delete", MenuType = 2, Permission = "company:delete", SortOrder = 2, Status = 1, IsVisible = 1 }
            };
            db.Insertable(menus).ExecuteCommand();
        }

        // 迁移：如果出库单菜单已在库存管理下，移到独立顶级目录
        var existingOutboundMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "outbound_order_manage").First();
        if (existingOutboundMenu != null)
        {
            var inventoryMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "inventory").First();
            if (inventoryMenu != null && existingOutboundMenu.ParentId == inventoryMenu.Id)
            {
                // 创建新的顶级目录
                var newDir = new Menu { MenuName = "出库管理", MenuCode = "outbound", Path = "", Icon = "ExportOutlined", MenuType = 0, SortOrder = 3, Status = 1, IsVisible = 1 };
                var newDirId = db.Insertable(newDir).ExecuteReturnIdentity();

                // 给新目录分配角色权限
                var roles = db.Queryable<Role>().ToList();
                var dirRoleMenus = new List<RoleMenu>();
                foreach (var role in roles)
                    dirRoleMenus.Add(new RoleMenu { RoleId = role.Id, MenuId = newDirId, CreatedAt = DateTime.UtcNow });
                if (dirRoleMenus.Any())
                    db.Insertable(dirRoleMenus).ExecuteCommand();

                // 移动出库单菜单和出库流水到新目录
                existingOutboundMenu.ParentId = newDirId;
                existingOutboundMenu.Icon = "FileTextOutlined";
                db.Updateable(existingOutboundMenu).UpdateColumns(x => new { x.ParentId, x.Icon }).ExecuteCommandHasChangeAsync();

                var flowMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "outbound_flow").First();
                if (flowMenu != null)
                {
                    flowMenu.ParentId = newDirId;
                    db.Updateable(flowMenu).UpdateColumns(x => new { x.ParentId }).ExecuteCommandHasChangeAsync();
                }
            }
        }

        // 增量添加出库单菜单（兼容已有数据）
        if (db.Queryable<Menu>().Any() && !db.Queryable<Menu>().Where(m => m.MenuCode == "outbound_order_manage").Any())
        {
            // 创建独立的"出库管理"顶级目录
            var outboundDir = new Menu { MenuName = "出库管理", MenuCode = "outbound", Path = "", Icon = "ExportOutlined", MenuType = 0, SortOrder = 3, Status = 1, IsVisible = 1 };
            var dirId = db.Insertable(outboundDir).ExecuteReturnIdentity();

            var newMenus = new List<Menu>
            {
                new Menu { ParentId = dirId, MenuName = "出库单管理", MenuCode = "outbound_order_manage", Path = "/outbound-orders", Icon = "FileTextOutlined", MenuType = 1, Permission = "outbound:view", SortOrder = 0, Status = 1, IsVisible = 1 },
                new Menu { ParentId = dirId, MenuName = "出库流水", MenuCode = "outbound_flow", Path = "/outbound-flows", Icon = "TransactionOutlined", MenuType = 1, Permission = "outbound:flow", SortOrder = 1, Status = 1, IsVisible = 1 }
            };
            db.Insertable(newMenus).ExecuteCommand();

            var outboundMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "outbound_order_manage").First();
            var flowMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "outbound_flow").First();

            if (outboundMenu != null)
            {
                var buttonMenus = new List<Menu>
                {
                    new Menu { ParentId = outboundMenu.Id, MenuName = "出库单新增", MenuCode = "outbound_add", MenuType = 2, Permission = "outbound:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                    new Menu { ParentId = outboundMenu.Id, MenuName = "出库单编辑", MenuCode = "outbound_edit", MenuType = 2, Permission = "outbound:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                    new Menu { ParentId = outboundMenu.Id, MenuName = "出库单删除", MenuCode = "outbound_delete", MenuType = 2, Permission = "outbound:delete", SortOrder = 2, Status = 1, IsVisible = 1 },
                    new Menu { ParentId = outboundMenu.Id, MenuName = "出库确认", MenuCode = "outbound_confirm", MenuType = 2, Permission = "outbound:confirm", SortOrder = 3, Status = 1, IsVisible = 1 }
                };
                db.Insertable(buttonMenus).ExecuteCommand();
            }

            // 为所有角色添加出库单菜单权限
            var allNewMenuIds = new List<int> { dirId };
            if (outboundMenu != null) allNewMenuIds.Add(outboundMenu.Id);
            if (flowMenu != null) allNewMenuIds.Add(flowMenu.Id);
            allNewMenuIds.AddRange(db.Queryable<Menu>().Where(m => m.MenuCode.StartsWith("outbound_") && m.MenuType == 2).Select(m => m.Id).ToList());

            var roles = db.Queryable<Role>().ToList();
            var newRoleMenus = new List<RoleMenu>();
            foreach (var role in roles)
            {
                foreach (var menuId in allNewMenuIds)
                {
                    newRoleMenus.Add(new RoleMenu { RoleId = role.Id, MenuId = menuId, CreatedAt = DateTime.UtcNow });
                }
            }
            if (newRoleMenus.Any())
                db.Insertable(newRoleMenus).ExecuteCommand();
        }

        // 增量添加公司管理菜单（兼容已有数据）
        if (db.Queryable<Menu>().Any() && !db.Queryable<Menu>().Where(m => m.MenuCode == "company_manage").Any())
        {
            var basicDataMenu = db.Queryable<Menu>().Where(m => m.MenuCode == "basic_data").First();
            if (basicDataMenu != null)
            {
                var companyMenu = new Menu { ParentId = basicDataMenu.Id, MenuName = "公司管理", MenuCode = "company_manage", Path = "/companies", Icon = "BankOutlined", MenuType = 1, Permission = "company:view", SortOrder = 2, Status = 1, IsVisible = 1 };
                var companyMenuId = db.Insertable(companyMenu).ExecuteReturnIdentity();

                var buttonMenus = new List<Menu>
                {
                    new Menu { ParentId = companyMenuId, MenuName = "公司新增", MenuCode = "company_add", MenuType = 2, Permission = "company:add", SortOrder = 0, Status = 1, IsVisible = 1 },
                    new Menu { ParentId = companyMenuId, MenuName = "公司编辑", MenuCode = "company_edit", MenuType = 2, Permission = "company:edit", SortOrder = 1, Status = 1, IsVisible = 1 },
                    new Menu { ParentId = companyMenuId, MenuName = "公司删除", MenuCode = "company_delete", MenuType = 2, Permission = "company:delete", SortOrder = 2, Status = 1, IsVisible = 1 }
                };
                db.Insertable(buttonMenus).ExecuteCommand();

                // 为所有角色添加公司管理菜单权限
                var allCompanyMenuIds = new List<int> { companyMenuId };
                allCompanyMenuIds.AddRange(db.Queryable<Menu>().Where(m => m.MenuCode.StartsWith("company_") && m.MenuType == 2).Select(m => m.Id).ToList());

                var roles = db.Queryable<Role>().ToList();
                var newRoleMenus = new List<RoleMenu>();
                foreach (var role in roles)
                {
                    foreach (var menuId in allCompanyMenuIds)
                    {
                        newRoleMenus.Add(new RoleMenu { RoleId = role.Id, MenuId = menuId, CreatedAt = DateTime.UtcNow });
                    }
                }
                if (newRoleMenus.Any())
                    db.Insertable(newRoleMenus).ExecuteCommand();
            }
        }

        // 初始化角色数据
        if (!db.Queryable<Role>().Any())
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, RoleCode = "admin", RoleName = "超级管理员", SortOrder = 0, Status = 1, Remark = "拥有所有权限", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, RoleCode = "warehouse", RoleName = "仓库管理员", SortOrder = 1, Status = 1, Remark = "负责仓库日常管理", CreatedAt = DateTime.UtcNow },
                new Role { Id = 3, RoleCode = "operator", RoleName = "操作员", SortOrder = 2, Status = 1, Remark = "基础操作权限", CreatedAt = DateTime.UtcNow }
            };
            db.Insertable(roles).ExecuteCommand();
        }

        // 初始化用户数据（密码：123456）
        if (!db.Queryable<User>().Any())
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456", BCrypt.Net.BCrypt.GenerateSalt(12));
            var users = new List<User>
            {
                new User { Id = 1, Username = "admin", Password = passwordHash, RealName = "系统管理员", Phone = "13800000000", Email = "admin@example.com", Status = 1, Remark = "超级管理员账户", CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Username = "warehouse", Password = passwordHash, RealName = "仓库管理员", Phone = "13800000001", Email = "warehouse@example.com", Status = 1, Remark = "仓库管理员账户", CreatedAt = DateTime.UtcNow },
                new User { Id = 3, Username = "operator", Password = passwordHash, RealName = "操作员", Phone = "13800000002", Email = "operator@example.com", Status = 1, Remark = "操作员账户", CreatedAt = DateTime.UtcNow }
            };
            db.Insertable(users).ExecuteCommand();
        }

        // 初始化用户角色关联
        if (!db.Queryable<UserRole>().Any())
        {
            var userRoles = new List<UserRole>
            {
                new UserRole { UserId = 1, RoleId = 1, CreatedAt = DateTime.UtcNow },  // admin -> 超级管理员
                new UserRole { UserId = 2, RoleId = 2, CreatedAt = DateTime.UtcNow },  // warehouse -> 仓库管理员
                new UserRole { UserId = 3, RoleId = 3, CreatedAt = DateTime.UtcNow }   // operator -> 操作员
            };
            db.Insertable(userRoles).ExecuteCommand();
        }

        // 初始化角色菜单关联（超级管理员拥有所有菜单）
        if (!db.Queryable<RoleMenu>().Any())
        {
            var roleMenus = new List<RoleMenu>();

            // 超级管理员拥有所有菜单
            for (int menuId = 1; menuId <= 33; menuId++)
            {
                roleMenus.Add(new RoleMenu { RoleId = 1, MenuId = menuId, CreatedAt = DateTime.UtcNow });
            }

            // 仓库管理员拥有基础数据和库存管理权限
            var warehouseMenuIds = new[] { 1, 2, 3, 4, 6, 7, 8, 9, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33 };
            foreach (var menuId in warehouseMenuIds)
            {
                roleMenus.Add(new RoleMenu { RoleId = 2, MenuId = menuId, CreatedAt = DateTime.UtcNow });
            }

            // 操作员拥有基础查看和PDA操作权限
            var operatorMenuIds = new[] { 1, 2, 3, 4, 6, 7, 8, 9, 23, 24, 29, 30 };
            foreach (var menuId in operatorMenuIds)
            {
                roleMenus.Add(new RoleMenu { RoleId = 3, MenuId = menuId, CreatedAt = DateTime.UtcNow });
            }

            db.Insertable(roleMenus).ExecuteCommand();
        }
    }
}
