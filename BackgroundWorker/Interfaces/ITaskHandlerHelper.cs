using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.Interfaces;

public interface ITaskHandlerHelper
{
    Task[] GetPreviousTask(int numberOfCurrentRow, GeneralTaskContext context);

    Task[] GetPreviousPrintTasksIfNotFirstRow(GeneralTaskContext context, int rowNumber);
}