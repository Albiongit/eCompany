using eCompany.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using eCompany.Shared;
using eCompany.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using eCompany.Hubs;
using eCompany;
using System.Reflection;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection");;



// Add services to the container.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization(option =>
                {
                    var type = typeof(SharedResource);
                    var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                    var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                    var localizer = factory.Create("SharedResource", assemblyName.Name);
                    option.DataAnnotationLocalizerProvider = (t, f) => localizer;
                });

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
     ));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddSignalR();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("de-DE")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.MapHub<TaskHub>("/taskHub");

app.Run();
