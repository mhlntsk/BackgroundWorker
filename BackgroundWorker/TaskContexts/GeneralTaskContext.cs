namespace BackgroundWorker.TaskContexts;

public class GeneralTaskContext(int totalRowsCount)
{
    public int TotalRowsCount { get; } = totalRowsCount;
    public RowTaskContext[] RowContexts { get; } = new RowTaskContext[totalRowsCount];
    public string StatusMessage { get; set; } = "Task matrix initialized.";
    public Exception? TaskException { get; set; }
}