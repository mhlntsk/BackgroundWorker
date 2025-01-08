using BackgroundWorker.Interfaces;
using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.Helpers;

public class PrintFormService : IPrintFormService
{
    private readonly Random _random = new Random(); // TODO in dev: ro remove.

    public async Task GetPersonViewModel(CancellationToken cancellationToken, RowTaskContext rowContext)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Delay(cancellationToken);
        rowContext.StatusMessage = $"Row {rowContext.RowNumber} - GetPersonViewModel finished.";
    }

    public async Task GenerateDocx(CancellationToken cancellationToken, RowTaskContext rowContext)
    {
        if (rowContext.RowNumber == 3)
        {
            throw new InvalidOperationException();
        }

        cancellationToken.ThrowIfCancellationRequested();
        await Delay(cancellationToken);
        rowContext.StatusMessage = $"Row {rowContext.RowNumber} - GenerateDocx finished.";
    }

    public async Task ConvertToPdf(CancellationToken cancellationToken, RowTaskContext rowContext)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Delay(cancellationToken);
        rowContext.StatusMessage = $"Row {rowContext.RowNumber} - ConvertToPdf finished.";
    }

    public async Task Print(CancellationToken cancellationToken, RowTaskContext rowContext)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Delay(cancellationToken);
        rowContext.StatusMessage = $"Row {rowContext.RowNumber} - Print finished.";
    }

    public async Task GenerateReport(CancellationToken cancellationToken, RowTaskContext rowContext)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Delay(cancellationToken);
        rowContext.StatusMessage = $"Row {rowContext.RowNumber} - Generating Report finished.";
    }

    private async Task Delay(CancellationToken cancellationToken) // TODO in dev: ro remove.
    {
        await Task.Delay(_random.Next(1000, 3000), cancellationToken);
    }
}