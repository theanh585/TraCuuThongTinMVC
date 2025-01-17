﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using TraCuuThongTinMVC.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SchoolContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SCHOOLDB"));
});
builder.Services.AddDistributedMemoryCache(); // Dùng bộ nhớ để lưu trữ session (yêu cầu)
builder.Services.AddSession(); // Kích hoạt dịch vụ session

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie hết hạn sau 30 phút
        options.SlidingExpiration = true; // Cập nhật lại thời gian hết hạn mỗi lần người dùng hoạt động
        options.Cookie.HttpOnly = true; // Tăng bảo mật
    });

builder.Services.AddAuthorization();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;  // Kích thước tối đa của mỗi giá trị trong form
    options.MultipartBodyLengthLimit = int.MaxValue;  // Kích thước tối đa của body form (bao gồm các tệp tải lên)
});
builder.Services.AddSingleton<IViewEngine, RazorViewEngine>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
