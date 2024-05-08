using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.Server;
using SV20T1020607.Wed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie( option =>
    {
        option.Cookie.Name = "AuthenticationCookie";
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/AccessDenined";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    });


builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    hostEnvironment: app.Services.GetService<IWebHostEnvironment>()
);
string connectionString = "Server=localhost;port=3306;Database=LiteCommerceDBUpdate2023 ;user=root;password=; Persist security Info = true";

SV20T1020067.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();

