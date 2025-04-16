using API.ML.BOBase;
using API.ML.Common;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Use HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7198);
    //options.ListenLocalhost(7197, listenOptions =>
    //{
    //    listenOptions.UseHttps();
    //});
});

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;  //Pascal Case
});

builder.Services.AddHttpClient();

// Add MySQL database context
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!String.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseMySQL(connectionString: connectionString));
}

//CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://gudfud.online", "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<CleanupService>();
builder.Services.AddHostedService<FakeDataService>();
builder.Services.AddHostedService<SimulateOrderService>();

Config.ReadConfig(builder);
//Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = CommonValue.TokenValidationParameters;

                    if (Config.UseCookie)
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var authToken = context.HttpContext.Request.Cookies["AuthToken"];

                                if (!string.IsNullOrEmpty(authToken) && authToken == Session.Token)
                                {
                                    context.Token = authToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    }
                });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDBContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseRouting();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Disable caching for all static files
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    }
});

app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();
