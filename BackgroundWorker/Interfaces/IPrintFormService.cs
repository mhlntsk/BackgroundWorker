using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.Interfaces;

public interface IPrintFormService
{
    public Task GetPersonViewModel(CancellationToken cancellationToken, RowTaskContext rowContext);
    public Task GenerateDocx(CancellationToken cancellationToken, RowTaskContext rowContext);
    public Task ConvertToPdf(CancellationToken cancellationToken, RowTaskContext rowContext);
    public Task Print(CancellationToken cancellationToken, RowTaskContext rowContext);
    public Task GenerateReport(CancellationToken cancellationToken, RowTaskContext rowContext);
}