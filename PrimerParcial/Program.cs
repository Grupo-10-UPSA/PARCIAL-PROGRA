using Microsoft.EntityFrameworkCore;
using PrimerParcial.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();