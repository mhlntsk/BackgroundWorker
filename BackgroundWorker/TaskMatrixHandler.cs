using BackgroundWorker.Interfaces;
using BackgroundWorker.TaskContexts;

namespace BackgroundWorker;

public class TaskMatrixHandler(ITaskHandlerHelper taskHandlerHelper, IPrintFormService printFormService)
    : ITaskMatrixHandler
{
    public async Task ExecuteTaskMatrixAsync(List<int> employmentIds, List<int> formTypeIds, List<int> pdfTypeIds,
        CancellationToken cancellationToken, IProgress<GeneralTaskContext> progress)
    {
        var context = new GeneralTaskContext(totalRowsCount:
            employmentIds.Count * formTypeIds.Count +
            employmentIds.Count * pdfTypeIds.Count);

        try
        {
            int numberOfCurrentRow = 0;
            for (int currentEmploymentId = 1; currentEmploymentId <= employmentIds.Count; currentEmploymentId++)
            {
                for (int currentFormTypeId = 1; currentFormTypeId <= formTypeIds.Count; currentFormTypeId++)
                {
                    numberOfCurrentRow++;
                    var currentRowContext = new RowTaskContext(numberOfCurrentRow);
                    var previousTasks = taskHandlerHelper.GetPreviousTask(numberOfCurrentRow, context);

                    currentRowContext.TaskCompletion = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessGetPersonViewModel(cancellationToken, context, currentRowContext, progress);
                            await ProcessGenerateDocx(cancellationToken, context, currentRowContext, progress);
                            await ProcessConvertToPdf(cancellationToken, context, currentRowContext, progress);
                            Task.WaitAll(previousTasks, cancellationToken);
                            await ProcessPrint(cancellationToken, context, currentRowContext, progress);
                            
                            CompleteSingleRow(currentRowContext);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            HandleInCircleException(ex, context, currentRowContext, progress);
                        }
                    }, cancellationToken);
                }

                for (int currentPdfTypeId = 1; currentPdfTypeId <= pdfTypeIds.Count; currentPdfTypeId++)
                {
                    numberOfCurrentRow++;
                    var currentRowContext = new RowTaskContext(numberOfCurrentRow);
                    var previousTasks = taskHandlerHelper.GetPreviousTask(numberOfCurrentRow, context);

                    currentRowContext.TaskCompletion = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessGenerateReport(cancellationToken, context, currentRowContext, progress);
                            Task.WaitAll(previousTasks, cancellationToken);
                            await ProcessPrint(cancellationToken, context, currentRowContext, progress);

                            CompleteSingleRow(currentRowContext);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            HandleInCircleException(ex, context, currentRowContext, progress);
                        }
                    }, cancellationToken);
                }
            }

            await Task.WhenAll(context.RowContexts.Select(rc => rc.TaskCompletion));
            context.StatusMessage = "All tasks completed.";
            progress?.Report(context);
        }
        catch (OperationCanceledException)
        {
            context.StatusMessage = "Operation aborted.";
            progress?.Report(context);
            throw;
        }
        catch (Exception ex)
        {
            context.StatusMessage = $"An unexpected error occurred: {ex.Message}";
            context.TaskException = ex;
            progress?.Report(context);
            throw;
        }
    }
    

    private async Task ProcessGetPersonViewModel(CancellationToken cancellationToken, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        cancellationToken.ThrowIfCancellationRequested();
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Starting GetPersonViewModel.";
        progress?.Report(context);
        await printFormService.GetPersonViewModel(cancellationToken, currentRowContext);
    }

    private async Task ProcessGenerateDocx(CancellationToken cancellationToken, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        cancellationToken.ThrowIfCancellationRequested();
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Starting GenerateDocx.";
        progress?.Report(context);
        await printFormService.GenerateDocx(cancellationToken, currentRowContext);
    }

    private async Task ProcessConvertToPdf(CancellationToken cancellationToken, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        cancellationToken.ThrowIfCancellationRequested();
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Starting ConvertToPdf.";
        progress?.Report(context);
        await printFormService.ConvertToPdf(cancellationToken, currentRowContext);
    }

    private async Task ProcessPrint(CancellationToken cancellationToken, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        cancellationToken.ThrowIfCancellationRequested();
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Starting Print.";
        progress?.Report(context);
        await printFormService.Print(cancellationToken, currentRowContext);
    }

    private async Task ProcessGenerateReport(CancellationToken cancellationToken, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        cancellationToken.ThrowIfCancellationRequested();
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Starting Generating Report.";
        progress?.Report(context);
        await printFormService.GenerateReport(cancellationToken, currentRowContext);
    }

    private void HandleInCircleException(Exception ex, GeneralTaskContext context,
        RowTaskContext currentRowContext, IProgress<GeneralTaskContext> progress)
    {
        currentRowContext.TaskException = ex;
        currentRowContext.StatusMessage =
            $"Row {currentRowContext.RowNumber} - Failed: {ex.Message}.";
        progress?.Report(context);
    }
    
    private void CompleteSingleRow(RowTaskContext currentRowContext)
    {
        currentRowContext.IsCompleted = true;
        currentRowContext.StatusMessage = $"Row {currentRowContext.RowNumber} - Completed.";
    }
}