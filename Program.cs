
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core with in-memory database
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// POST: Create a new task
// to create a task
// Using: curl -X POST http://localhost:5004/api/tasks -H "Content-Type: application/json" -d '{"name":"Finish API","isComplete":false}'
app.MapPost("/api/tasks", async (TaskItem task, TodoDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
});

// GET: Retrieve all tasks
// Using $ curl http://localhost:5004/api/tasks
app.MapGet("/api/tasks", async (TodoDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});

// GET: Retrieve a task by ID
// Using: $ curl http://localhost:5004/api/tasks/1
app.MapGet("/api/tasks/{id}", async (int id, TodoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

// PUT: Update an existing task
// Using: $ curl -X PUT http://localhost:5004/api/tasks/1 -H "Content-Type: application/json" -d '{"id":1,"name":"Updated Task","isComplete":true}'
app.MapPut("/api/tasks/{id}", async (int id, TaskItem updatedTask, TodoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name = updatedTask.Name;
    task.IsComplete = updatedTask.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Delete a task
// Using: $ curl -X DELETE http://localhost:5004/api/tasks/1
app.MapDelete("/api/tasks/{id}", async (int id, TodoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


// Define TaskItem model
public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}


// Define DbContext
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    public DbSet<TaskItem> Tasks { get; set; }
}
