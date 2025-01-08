namespace BackgroundWorker.TaskContexts;

public class RowTaskContext(int rowNumber)
{
    public Task TaskCompletion { get; set; }
    public int RowNumber { get; } = rowNumber;
    public string StatusMessage { get; set; } = "Not started.";
    public Exception? TaskException { get; set; }
    public bool IsCompleted { get; set; }
}