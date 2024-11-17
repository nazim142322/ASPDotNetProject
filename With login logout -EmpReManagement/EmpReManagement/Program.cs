using EmpReManagement.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EmpReManagement.MapperProfiler;
using EmpReManagement.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register DbContext
var connectionString = builder.Configuration.GetConnectionString("AEmpMngtcs");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<EmailService>();

//add session service
builder.Services.AddSession();


// Add AutoMapper
var mapperConfiguration = new MapperConfiguration(cgf =>
{
    cgf.AddProfile(typeof(YourMappingProfile));
});
var mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//add session middleware
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserLoginRegistration}/{action=Login}/{id?}");

app.Run();
