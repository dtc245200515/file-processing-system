namespace FileProcessing.Shared.Messages;

/// <summary>
/// Thông điệp gửi vào RabbitMQ queue khi tạo job mới
/// </summary>
public class JobMessage
{
    public int JobId { get; set; }
    public int FileRecordId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string ProcessingType { get; set; } = string.Empty;
}
