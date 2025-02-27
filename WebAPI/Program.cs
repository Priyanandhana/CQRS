using Application;
using Presentation;
using Infrastructure;
using Serilog;
using Application.Interfaces;
using Infrastructure.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using Application.Common;
using Z.Dapper.Plus;

var builder = WebApplication.CreateBuilder(args);

// Register encoding provider to support code pages like 1252
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// 1. Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonDateOnlyConverter());
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

// 2. Register Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register application layers with correct connection string
builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration.GetConnectionString("DefaultConnection"));

// 4. Register DbContext and IAppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// 5. Register validators and validation service
builder.Services.AddScoped<PurchaseOrderHeaderValidator>();
builder.Services.AddScoped<PurchaseOrderDetailValidator>();
builder.Services.AddScoped<PurchaseOrderValidationService>();

// 6. Register the file conversion service
builder.Services.AddScoped<IFileConversionService, PurchaseOrderFileService>();

// 7. Register the purchase order repository properly (Fix for dbContext issue)
builder.Services.AddScoped<IPurchaseOrderRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var dbContext = provider.GetRequiredService<AppDbContext>();
    return new PurchaseOrderRepository(config, dbContext);
});

// 8. Register Dapper's IDbConnection (as a transient service for short-lived use)
builder.Services.AddTransient<IDbConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

// 9. Register MediatR for CQRS using a valid assembly marker
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

// 10. Configure Serilog for logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 11. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Build the application
var app = builder.Build();

// 12. Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 13. Register Dapper Plus Entities for Bulk Insert
DapperPlusManager.Entity<Domain.Entities.PurchaseOrderHeader>().Table("PurchaseOrderHeader");
DapperPlusManager.Entity<Domain.Entities.PurchaseOrderDetail>().Table("PurchaseOrderDetail");

// Run the application
app.Run();

// Custom JSON Converter for Date Formatting
public class JsonDateOnlyConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}
