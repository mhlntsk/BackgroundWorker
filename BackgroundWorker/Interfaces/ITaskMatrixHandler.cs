using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.Interfaces;

public interface ITaskMatrixHandler
{
    Task ExecuteTaskMatrixAsync(List<int> employmentIds, List<int> formTypeIds, List<int> pdfTypeIds,
        CancellationToken cancellationToken, IProgress<GeneralTaskContext> progress);
}