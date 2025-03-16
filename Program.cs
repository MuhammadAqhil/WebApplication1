using Microsoft.AspNetCore.Cors.Infrastructure;
using WebApplication1.Services.Transaction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string _appPolicy = "WebApp1";
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(_appPolicy, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddTransient<ITransactionService, TransactionService>();

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
