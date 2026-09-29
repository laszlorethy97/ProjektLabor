using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using KeyManagement.Api.Data;
using KeyManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.UTF8.GetBytes(
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing."));

builder.Services.AddControllers();

builder.Services.AddServices();

builder.Services.AddOpenApi(options =>
{
    options.CreateSchemaReferenceId = typeInfo => typeInfo.Type.FullName;
    options.AddDocumentTransformer((document, _, _) =>
    {
        var components = document.Components ??= new OpenApiComponents();
        components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        return Task.CompletedTask;
    });
    options.AddOperationTransformer((operation, context, _) =>
    {
        var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (endpointMetadata.OfType<IAuthorizeData>().Any()
            && !endpointMetadata.OfType<IAllowAnonymous>().Any())
        {
            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = new List<string>()
            };
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(securityRequirement);
        }

        return Task.CompletedTask;
    });
});
builder.Services.AddDbContext<KeyManagement.Api.Data.KeyManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KeyManagementDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "KeyManagement API");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
