using MESAmetrics.Hubs;
using MESAmetrics.Models;
using MESAmetrics.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddScoped<IMetricsService, MetricsService>();

var connection = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(connection);
});

var allowedConnection = builder.Configuration.GetValue<string>("OrigenesPermitidos")!
        .Split(',')
        .Select(origin => origin.Trim())
        .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin => true);
        }
        else
        {
            policy.WithOrigins(allowedConnection);
        }

        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.MapHub<MachineHub>("/machineHub");

app.Run();