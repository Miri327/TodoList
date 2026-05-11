using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// תיקון עבור Render: מניעת קריסה בגלל מעקב אחרי קבצים
builder.Configuration.Sources.OfType<Microsoft.Extensions.Configuration.Json.JsonConfigurationSource>()
    .ToList().ForEach(source => source.ReloadOnChange = false);

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// רישום ה-DbContext
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'ToDoDB' is missing! Add it to Render Environment Variables with key: ConnectionStrings__ToDoDB"
    );
}

Console.WriteLine("Connection String configured");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// יצירת הטבלה ידנית עם Raw SQL - פותר את הבעיה של EnsureCreated
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();

        Console.WriteLine("Attempting to create items table...");

        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS `items` (
                `Id` int NOT NULL AUTO_INCREMENT,
                `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
                `IsComplete` tinyint(1) NOT NULL DEFAULT 0,
                PRIMARY KEY (`Id`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
        ");

        Console.WriteLine("Items table ready!");

        var itemCount = db.Items.Count();
        Console.WriteLine($"Items table verified! Current count: {itemCount}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
}

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new { message = "ToDo API is running!" }));

app.MapGet("/items", async (ToDoDbContext db) =>
{
    try
    {
        var items = await db.Items.ToListAsync();
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"GET /items error: {ex.Message}");
        return Results.StatusCode(500);
    }
});

app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    try
    {
        db.Items.Add(newItem);
        await db.SaveChangesAsync();
        return Results.Created($"/items/{newItem.Id}", newItem);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"POST /items error: {ex.Message}");
        return Results.StatusCode(500);
    }
});

app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.IsComplete = inputItem.IsComplete;
        if (!string.IsNullOrEmpty(inputItem.Name))
        {
            item.Name = inputItem.Name;
        }

        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"PUT /items/{id} error: {ex.Message}");
        return Results.StatusCode(500);
    }
});

app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DELETE /items/{id} error: {ex.Message}");
        return Results.StatusCode(500);
    }
});

app.Run();