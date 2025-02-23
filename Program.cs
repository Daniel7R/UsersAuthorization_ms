
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsersAuthorization.Application.Interfaces;
using UsersAuthorization.Application.Mapping;
using UsersAuthorization.Application.Service;
using UsersAuthorization.Domain.Entities;
using UsersAuthorization.Infrastructure.Authentication;
using UsersAuthorization.Infrastructure.Data;
using UsersAuthorization.Infrastructure.EventBus;
using UsersAuthorization.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//to enable insert date time
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddNpgsql<UserDbContext>(builder.Configuration.GetConnectionString("dbConnectionUsers"));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("APISettings:JwtOptions"));

builder.Services.AddIdentity<
    ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IEventBusConsumer, EventBusConsumer>();
builder.Services.AddHostedService<EventBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



ApplyMigration();

app.Run();




void ApplyMigration()
{
    using(var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        if(_db.Database.GetPendingMigrations().Count() >0) _db.Database.Migrate();
    }
}