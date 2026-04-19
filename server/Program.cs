using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת הגדרת CORS - מאפשר לכל אתר לגשת ל-API (שימושי לפיתוח)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// רישום ה-DbContext
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// הוספת תמיכה ב-Swagger (כלים לתצוגה גרפית של ה-API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// הגדרת פוליסי של CORS שמאפשר הכל
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // מאפשר מכל דומיין
              .AllowAnyMethod()   // מאפשר את כל סוגי הפעולות (GET, POST, וכו')
              .AllowAnyHeader();  // מאפשר את כל ה-Headers
    });
});

var app = builder.Build();

// הפעלת Swagger רק בסביבת פיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// הפעלת ה-CORS עם השם שהגדרנו
app.UseCors("AllowAll");

app.MapGet("/", () => "ToDo API is running!");

//---------------------------------Routes----------------------------

app.MapGet("/items", async (ToDoDbContext db) =>
    await db.Items.ToListAsync());

// שיניתי את הנתיב מ-addItem ל-/items כדי לשמור על אחידות (RESTful)
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    // עדכון הסטטוס תמיד
    item.IsComplete = inputItem.IsComplete;

    // עדכון השם רק אם נשלח שם חדש ולא ריק
    if (!string.IsNullOrEmpty(inputItem.Name))
    {
        item.Name = inputItem.Name;
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.Run();
