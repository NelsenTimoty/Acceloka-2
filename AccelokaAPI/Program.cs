using AccelokaAPI.Data;
using AccelokaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using AccelokaAPI.Features.Tickets.Queries;
using AccelokaAPI.Features.Tickets.Validators;
using AccelokaAPI.Features.BookedTickets.Validators;
using Serilog;
using AccelokaAPI.Validator;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure Serilog logging
var logFilePath = Path.Combine("logs", $"Log-{DateTime.UtcNow:yyyyMMdd}.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // ✅ Log level: Information and above
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .WriteTo.Console() // Optional: Log to console
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day) // ✅ Save logs to /logs folder
    .CreateLogger();


// ✅ Add services to the container
builder.Services.AddControllers();

// ✅ Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MediatR
builder.Services.AddMediatR(typeof(GetAvailableTicketsQuery).Assembly);

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<GetAvailableTicketsValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBookedTicketsValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RevokeBookedTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditBookedTicketValidator>();

// ✅ Configure Database Connection (use appsettings.json connection string)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register Repositories
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IBookedTicketRepository, BookedTicketRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// ✅ Configure RFC 7807 Problem Details for Validation Errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

var app = builder.Build();
app.UseCors("AllowAll");

// ✅ Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
