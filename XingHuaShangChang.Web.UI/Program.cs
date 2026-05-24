using SqlSugar;
using XingHuaShangChang.Application.Services.ProductService;
using XingHuaShangChang.Application.Services.PDA;
using XingHuaShangChang.Application.Services.UserService;
using XingHuaShangChang.Application.Services.RoleService;
using XingHuaShangChang.Application.Services.MenuService;
using XingHuaShangChang.Application.Services.FileService;
using XingHuaShangChang.Application.Services.OutboundOrderService;
using XingHuaShangChang.Application.Services.CompanyService;
using XingHuaShangChang.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<ISqlSugarClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySql");
    var db = new SqlSugarScope(new ConnectionConfig
    {
        ConnectionString = connectionString,
        DbType = DbType.MySql,
        IsAutoCloseConnection = true,
        InitKeyType = InitKeyType.Attribute
    },
    db =>
    {
        var env = sp.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine($"[SQL] {sql}");
            };
        }
    });
    return db;
});

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPdaService, PdaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IOutboundOrderService, OutboundOrderService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    DatabaseInitializer.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
