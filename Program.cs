var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();

// ЭНДПОИНТ /health
app.MapGet("/health", () => new
{
    status = "ok",
    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
});

// ЭНДПОИНТ /version
app.MapGet("/version", () => new
{
    appName = "IsLabApp",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
});

// Хранилище заметок
var notes = new List<Note>();
int nextId = 1;

// GET /api/notes - все заметки
app.MapGet("/api/notes", () => notes);

// GET /api/notes/{id} - одна заметка
app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note == null ? Results.NotFound() : Results.Ok(note);
});

// POST /api/notes - создать заметку
app.MapPost("/api/notes", (Note newNote) =>
{
    newNote.Id = nextId++;
    newNote.CreatedAt = DateTime.Now;
    notes.Add(newNote);
    return Results.Created($"/api/notes/{newNote.Id}", newNote);
});

// DELETE /api/notes/{id} - удалить заметку
app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note == null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

// ЭНДПОИНТ /db/ping
app.MapGet("/db/ping", () => new
{
    status = "error",
    message = "SQL Server not configured yet"
});

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();

// МОДЕЛЬ ЗАМЕТКИ
public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}