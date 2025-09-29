using Configurations;
using Microsoft.EntityFrameworkCore;
using TheSocialCebu_Capstone.Context;
using TheSocialCebu_Capstone.Controllers;
using TheSocialCebu_Capstone.Hubs;
using TheSocialCebu_Capstone.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//SignalR
builder.Services.AddSignalR();
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<EmailService>();

//Database
builder.Services.AddDbContext<MyDBContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Default").Value,
            sql => sql.EnableRetryOnFailure())
        .EnableSensitiveDataLogging(),
    ServiceLifetime.Transient
);

//builder.Services.AddTransient<MyDBContext>();
//builder.Services.AddTransient<OrderController>();
//builder.Services.AddTransient<CashierController>();
//builder.Services.AddTransient<KitchenController>();

//Session service
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

//Session
app.UseSession();
app.UseRouting();

//SignalR Mapping
app.MapHub<ConnectorHub>("/connectorHub");

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
