using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;

var builder = WebApplication.CreateBuilder(args);
// Veritaban� ba�lant�s�n� projeye tan�t�r
builder.Services.AddDbContext<OrendaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Orenda.Web.Services.ILogService, Orenda.Web.Services.LogService>();

// Authentication & Authorization Ayarları
builder.Services.AddAuthentication("OrendaAuthCookie")
    .AddCookie("OrendaAuthCookie", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "OrendaAuthCookie";
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

app.UseAuthentication(); // Önemli: Authorization'dan önce gelmeli
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Account}/{action=Login}/{id?}");
    //pattern: "{controller=Account}/{action=Register}/{id?}");
    //pattern: "{controller=ToDo}/{action=Index}/{id?}");

app.Run();
