using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== เพิ่ม 3 บรรทัดนี้ =====
builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductTypeRepository>();
builder.Services.AddScoped<MemberRepository>();
// ==============================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// ให้เปิดโปรแกรมมาที่หน้าสินค้าเลย
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");


app.Run();
