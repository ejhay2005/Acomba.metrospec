using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Controllers;
using acomba.zuper_api.Helper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IinvoiceService,InvoiceService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAcombaConnection, AcombaConnection>();
builder.Services.AddScoped<IEmployeeService,EmployeeService>();
builder.Services.AddScoped<ICountryHelper, CountryHelper>();
builder.Services.AddSwaggerGen( c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Api Key to access the api.",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme ="ApiKeyScheme"
    });
    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme,new List<string>()}
    };
    c.AddSecurityRequirement(requirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();
app.UseSwaggerUI();
//use api-key authentication
//app.UseMiddleware<ApiKeyAuthentication>();
app.UseCors(policy =>
    policy.WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithHeaders(HeaderNames.ContentType));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
