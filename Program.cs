using BackendHrdAgro.Models.Database.MySql;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BackendHrdAgro.Services.Email;
using BackendHrdAgro.Services.Scheduler;
using NReco.Logging.File;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Penambahan
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = int.MaxValue; });
builder.Services.Configure<FormOptions>(options => { options.ValueLengthLimit = int.MaxValue; options.MultipartBodyLengthLimit = int.MaxValue; options.MultipartHeadersLengthLimit = int.MaxValue; });

builder.Services.AddDbContext<DatabaseContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailService>();
//

builder.Services.AddLogging(x => {
    var loogingSection = builder.Configuration.GetSection("Logging");
    x.AddFile($"Logs/{DateTime.Now:yyyy/MM}/{DateTime.Now:dd}/{DateTime.Now:HH}.log", append: true);
});


//builder.Services.AddHostedService<OvertimeNotificationService>();  // Tambahan samsul
/*builder.Services.AddSingleton<IJob, IntegrateScheduler>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = provider.GetService<ISchedulerFactory>();
    return schedulerFactory.GetScheduler().Result;
});
builder.Services.AddQuartz();

builder.Services.AddQuartzHostedService(x =>
{
    x.WaitForJobsToComplete = true;
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
