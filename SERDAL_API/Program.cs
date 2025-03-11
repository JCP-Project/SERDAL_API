using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SERDAL_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(option =>
{
   option.UseSqlServer(builder.Configuration.GetConnectionString("con"));
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")  // React app URL
              .AllowAnyMethod()
              .AllowAnyHeader());

});

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);




var app = builder.Build();

app.UseCors("AllowReactApp");
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
