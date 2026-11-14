using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.MapGet("/items", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});

app.MapPost("/items", async (Item item, ToDoDbContext db) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("/items/{id}", async (
    int id, Item input, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);

    if (item == null)
        return Results.NotFound();

    item.Name = input.Name;
    item.IsComplete = input.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (
    int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);

    if (item == null)
        return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/", () => "Todo API is running!");
app.Run();