using FileProcessing.Api.Data;
using FileProcessing.Shared.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext với SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=/app/data/fileprocessing.db"));

builder.Services.AddOpenApi();

var app = builder.Build();

// Tự động tạo DB khi khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Tắt redirect HTTPS khi chạy local/Docker
// app.UseHttpsRedirection();

// =====================
// POST /files — Upload file
// =====================
app.MapPost("/files", async (IFormFile file, AppDbContext db) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "Không có file hoặc file rỗng." });

    // Tạo thư mục lưu file
    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    Directory.CreateDirectory(uploadDir);

    // Tạo tên file duy nhất
    var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var filePath = Path.Combine(uploadDir, storedFileName);

    // Lưu file lên disk
    using (var stream = File.Create(filePath))
        await file.CopyToAsync(stream);

    // Lưu thông tin vào DB
    var record = new FileRecord
    {
        OriginalFileName = file.FileName,
        StoredFileName   = storedFileName,
        FilePath         = filePath,
        ContentType      = file.ContentType,
        FileSize         = file.Length,
        UploadedAt       = DateTime.UtcNow
    };
    db.FileRecords.Add(record);
    await db.SaveChangesAsync();

    return Results.Created($"/files/{record.Id}", new
    {
        record.Id,
        record.OriginalFileName,
        record.FileSize,
        record.UploadedAt
    });
})
.WithName("UploadFile")
.DisableAntiforgery(); // cần thiết cho multipart/form-data với Minimal API

// =====================
// GET /jobs — Danh sách jobs (skeleton tuần 1)
// =====================
app.MapGet("/jobs", async (AppDbContext db) =>
{
    var jobs = await db.ProcessingJobs
        .Include(j => j.FileRecord)
        .Select(j => new
        {
            j.Id,
            j.Status,
            j.ProcessingType,
            j.CreatedAt,
            j.ProcessedAt,
            FileName = j.FileRecord.OriginalFileName
        })
        .ToListAsync();

    return Results.Ok(jobs);
})
.WithName("GetJobs");

app.Run();
