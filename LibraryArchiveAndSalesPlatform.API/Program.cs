using FluentValidation;
using LibraryArchiveAndSalesPlatform.API.Application.AppServices;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Account;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Book;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Note;
using LibraryArchiveAndSalesPlatform.API.Application.Dtos.Shelf;
using LibraryArchiveAndSalesPlatform.API.Application.IAppServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;
using LibraryArchiveAndSalesPlatform.API.Infrastructure;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Data;
using LibraryArchiveAndSalesPlatform.API.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region unitOfWork di
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
#endregion


#region Application Services di
builder.Services.AddScoped<IUserAppService, UserAppService>();
builder.Services.AddScoped<IBookAppService, BookAppService>();
builder.Services.AddScoped<INoteAppService, NoteAppService>();
builder.Services.AddScoped<IShelfAppService, ShelfAppService>();
#endregion

#region other services
builder.Services.AddScoped<ISaveChangesInterceptor, EntityInterceptor>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
#endregion

#region mail sender
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailProducer>();
builder.Services.AddHostedService<EmailConsumer>();
#endregion


#region Context
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);
#endregion

#region fluent validation di
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<IValidator<UserDetailsDto>, UserDetailsDtoValidator>();
builder.Services.AddScoped<IValidator<UserRole>, UserRoleValidator>();
builder.Services.AddScoped<IValidator<CreateBookDto>, CreateBookDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateBookDto>, UpdateBookDtoValidator>();
builder.Services.AddScoped<IValidator<CreateNoteDto>, CreateNoteDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateNoteDto>, UpdateNoteDtoValidator>();
builder.Services.AddScoped<IValidator<CreateShelfDto>, CreateShelfDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateShelfDto>, UpdateShelfDtoValidator>();
#endregion

#region Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
#endregion


#region Authentication Authorization
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
});
#endregion

#region serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
#endregion

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

app.UseSerilogRequestLogging();

app.Run();
