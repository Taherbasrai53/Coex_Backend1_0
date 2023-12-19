using COeX_India1._0.Data;
using COeX_India1._0.Helper;
using COeX_India1._0.Models;
using COeX_India1._0.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string CurrentConnString = null;
if (builder.Environment.IsDevelopment())
{
    CommonHelper.CurrentConnString = builder.Configuration.GetConnectionString("DefaultConnection");
    //CommonHelper.CurrentConnString = builder.Configuration.GetConnectionString("StagingConnection");
}
else
{
    CommonHelper.CurrentConnString = builder.Configuration.GetConnectionString("StagingConnection");
}

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(CommonHelper.CurrentConnString));
CommonHelper.Configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        Console.WriteLine();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async (context) =>
            {
                Console.WriteLine("Printing in the delegate OnAuthFailed");
            },
            OnChallenge = async (context) =>
            {

                context.HandleResponse();

                if (context.AuthenticateFailure != null)
                {
                    context.Response.StatusCode = 401;
                    //context.Response.Body = new Response(false, "");
                    var res = new { success = false, Message = "Unauthorized Access, Access Denied" };
                    //context.Response.Body = res;
                    //await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(res));
                    await context.HttpContext.Response.WriteAsJsonAsync(res);
                }
            }
        };
    }
    );

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    //serverOptions.ListenLocalhost(5001);
    //serverOptions.Limits.MaxConcurrentConnections = 100;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            //builder.WithOrigins("https://www.cutiee.com")
            //                    .AllowAnyHeader()
            //                    .AllowAnyMethod();
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddTransient<IActivityRepo, ActivityRepo>();

//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddQuartz(q =>
//    {
//        //q.UseMicrosoftDependencyInjectionJobFactory();
//        //var jobKey = new JobKey("TestJob");
//        //q.AddJob<TestJob>(opts => opts.WithIdentity(jobKey));
//        //q.AddTrigger(opts => opts
//        //    .ForJob(jobKey)
//        //    .WithIdentity("TestJob-trigger")
//        //    .WithCronSchedule("0 0/5 * * * ?"));
//        Console.WriteLine("In the add quartz method");

//        var activityJobKey = new JobKey("ActivityJob");
//        q.AddJob<ActivityJob>(opts => opts.WithIdentity(activityJobKey));
//        q.AddTrigger(opts => opts
//            .ForJob(activityJobKey)
//            .WithIdentity("ActivityJob-trigger")
//            .WithCronSchedule("*/10 * * * * ?"));
//    });
//    //*/5 * * * *
//    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
//}

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

app.Run();
