using ObjectBinderDemo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc(options => {
    options.ModelBinderProviders.Insert(0, new HiddenTypeModelBinderProvider());
});
builder.Services.Configure<HiddenTypeModelBinderOptions>(a => { });

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    //mvcBuilder.AddRazorRuntimeCompilation();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
