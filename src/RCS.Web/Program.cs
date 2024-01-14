using log4net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Log4net 
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();


var log = LogManager.GetLogger(typeof(Program));

try {
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

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    log.Info("Application is starting");
    app.Run();

}
catch (Exception ex)
{
    log.Fatal($"Application can not start.\n{ex}");
}
