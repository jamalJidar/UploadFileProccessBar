using UploadFileProccessBar.Models.DataBase;
using UploadFileProccessBar.Services.DocumentItemService;
using UploadFileProccessBar.Services.DocumentService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<FileUploadContext>(
    builder.Configuration.GetSection("UploadFileDataBase"));


builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentItemService, DocumentItemService>();
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

app.Run();
