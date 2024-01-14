using Infra.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Entites.User;
using Infra.Data.IdentityErrors;
using Domain.Interfaces.Identity;
using Application.Services.Account;
using Domain.Interfaces.Infra;
using Infra.Data.Data.Repositories;
using Domain.Interfaces.Vagas;
using Application.Services.Vagas;
using Infra.Data.Support.Pagination;
using Infra.Data.Support.Email;

var builder = WebApplication.CreateBuilder(args);


//settings para usar o appsettinhs json no projeto
builder.Configuration.AddJsonFile("appsettings.json");

//settings limite de upload
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 30 * 1024 * 1024;
});

//settings para usar o www.root
builder.Services.AddSingleton(builder.Environment);

builder.Services.AddControllersWithViews();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

//settings para conexão com database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connection).UseLazyLoadingProxies()); ;

//settings gerais para usar o identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
          .AddErrorDescriber<PortugueseMessages>();

builder.Services.Configure<IdentityOptions>(options =>
{
    //register settings
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;

    //lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;


});

//configuração do cookie gerado pelo identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "AspNetCore.Cookies";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = false;
    options.LoginPath = "/Identity/Account/Login";
});

//settinhs de roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
         policy => policy.RequireRole("Admin"));
});


//injeções de dependências gerais
builder.Services.AddScoped<IAccountInterface, AccountService>();
builder.Services.AddScoped<IVagas, VagasService>();
builder.Services.AddScoped<IVaga, VagasRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPagination, PaginationService>();
builder.Services.AddScoped<IEmail, EmailService>();

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

//await CriarPerfisUsuariosAsync(app);

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "Identity",
      pattern: "{area:exists}/{controller=Account}/{action=Index}/{id?}"
    );
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "Vagas",
      pattern: "{area:exists}/{controller=Vagas}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




app.Run();

