using Application.Abstractions.IHubs;
using Application.Abstractions.IUnitOfWork;
using Infrastructure;
using Infrastructure.UnitOfWork;
using Serilog;
using Web.Extensions;
using Web.Filters;
using Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<FileValidationFilter>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IChatHub, ChatHub>();
builder.Services.AddSwaggerAuthConfig();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelValidationFilter>();
});
builder.Services.AddCustomExceptionHandler(builder.Environment);
builder.Host.UseSerilog();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<ChatHub>("/api/Chat");
app.UseExceptionHandler(); 
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();