using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using WebTinTucFLIC.Controllers;
using WebTinTucFLIC.Helper;

var builder = WebApplication.CreateBuilder(args);

// Login with Google
var googleClientId = EncryptionHelper.DecryptString(builder.Configuration["Authentication:Google:ClientId"]!);
var googleClientSecret = EncryptionHelper.DecryptString(builder.Configuration["Authentication:Google:ClientSecret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/LoginPage";
})
.AddGoogle(options =>
{
    options.ClientId = googleClientId!;
    options.ClientSecret = googleClientSecret!;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.CallbackPath = "/signin-google";
});

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ImageController>();
builder.Services.AddScoped<NewsTypeController>();
builder.Services.AddScoped<NewsController>();
builder.Services.AddScoped<AccessLogController>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

//app.MapGet("/signin-google", async context =>
//{
//    var result = await context.AuthenticateAsync("Google");
//    if (result.Succeeded)
//    {
//        // Thanh cong , tra ve
//        context.Response.Redirect("/Home/Profile");
//    }
//    else
//    {
//        // Loi
//        context.Response.Redirect("/LoginWithGoogle");
//    }
//});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=News}/{action=Index}/{id?}");

app.Run();
