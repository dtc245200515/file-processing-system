namespace FileProcessing.Shared.Models;

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public enum ProcessingType
{
    Thumbnail,
    Compress
}

public class ProcessingJob
{
    public int Id { get; set; }
    public int FileRecordId { get; set; }
    public FileRecord FileRecord { get; set; } = null!;
    public ProcessingType ProcessingType { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public string? ResultPath { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
