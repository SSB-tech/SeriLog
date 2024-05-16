using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Ui.MsSqlServerProvider;
using Serilog.Ui.Web;
using SeriLog.DB;
using SeriLogProject;
using SeriLogProject.BusinessLogic;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var constr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(constr));


builder.Services.AddTransient<GlobalExceptionMW>(); //MiddleWare


builder.Services.AddSerilogUi(options => options.UseSqlServer(constr, "Logs"));
//---------SeriLog


Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
//---------SeriLog

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

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMW>();//MiddleWare

app.UseSerilogUi();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//---------Procedure Seeding

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    app.DbProcedureUpdateExecute(ref env);
}

//---------Procedure Seeding
app.Run();
