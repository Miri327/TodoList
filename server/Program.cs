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
        "❌ Connection string 'ToDoDB' is missing!\n" +
        "Add it to Render Environment Variables with key: ConnectionStrings__ToDoDB"
    );
}

Console.WriteLine($"✅ Connection String configured");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// **חשוב: יצירת הטבלאות ב-Startup**
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        
        Console.WriteLine("🔄 Attempting to create database and tables...");
        
        // יצירה של הבסיס נתונים והטבלאות
        db.Database.EnsureCreated();
        
        Console.WriteLine("✅ Database and tables created successfully!");
        
        // בדיקה שהטבלה קיימת
        var itemCount = db.Items.Count();
        Console.WriteLine($"✅ Items table verified! Current count: {itemCount}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
        Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
        
        // אל תעצור את האפליקציה, רק לוג את השגיאה
        // throw; // אם אתה רוצה להעצור, בטל את ההערה
    }
}

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health check endpoint
app.MapGet("/", () => Results.Ok(new { message = "ToDo API is running!" }));

// GET /items - קבל את כל המשימות
app.MapGet("/items", async (ToDoDbContext db) =>
{
    try
    {
        var items = await db.Items.ToListAsync();
        Console.WriteLine($"✅ GET /items - Retrieved {items.Count} items");
        return Results.Ok(items);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ GET /items error: {ex.Message}");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

// POST /items - הוסף משימה חדשה
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(newItem.Name))
        {
            return Results.BadRequest("Name is required");
        }

        db.Items.Add(newItem);
        await db.SaveChangesAsync();
        
        Console.WriteLine($"✅ POST /items - Created item: {newItem.Name} (ID: {newItem.Id})");
        return Results.Created($"/items/{newItem.Id}", newItem);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ POST /items error: {ex.Message}");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

// PUT /items/{id} - עדכן משימה
app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null)
        {
            Console.WriteLine($"⚠️  PUT /items/{id} - Item not found");
            return Results.NotFound();
        }

        item.IsComplete = inputItem.IsComplete;
        if (!string.IsNullOrEmpty(inputItem.Name))
        {
            item.Name = inputItem.Name;
        }

        await db.SaveChangesAsync();
        
        Console.WriteLine($"✅ PUT /items/{id} - Updated item");
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ PUT /items/{id} error: {ex.Message}");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

// DELETE /items/{id} - מחק משימה
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null)
        {
            Console.WriteLine($"⚠️  DELETE /items/{id} - Item not found");
            return Results.NotFound();
        }

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        
        Console.WriteLine($"✅ DELETE /items/{id} - Deleted item");
        return Results.Ok(item);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ DELETE /items/{id} error: {ex.Message}");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

app.Run();
