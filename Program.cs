using RestfulDemo.Database;
using RestfulDemo.Services;
using SQLitePCL;

Batteries.Init();

SQLiteManager.connect("todo.sqlite");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<TodoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();