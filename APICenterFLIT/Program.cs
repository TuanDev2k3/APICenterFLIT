using APICenterFlit;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DbnewsContext>(o => o.UseSqlServer(EncryptionHelper.DecryptString(builder.Configuration.GetConnectionString("apiconn")!)));
builder.Services.AddAutoMapper(typeof(MyAutoMapper).Assembly);
builder.Services.AddSwaggerGen();

DI.DI_Portal(builder.Services);
DI.DI_Users(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
