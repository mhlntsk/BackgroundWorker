using BackgroundWorker.Interfaces;
using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.Helpers;

public class TaskHandlerHelper : ITaskHandlerHelper
{
    public Task[] GetPreviousTask(int numberOfCurrentRow, GeneralTaskContext context)
    {
        return numberOfCurrentRow > 1
            ? GetPreviousPrintTasksIfNotFirstRow(context, numberOfCurrentRow)
            : [Task.CompletedTask];
    }

    public Task[] GetPreviousPrintTasksIfNotFirstRow(GeneralTaskContext context, int rowNumber)
    {
        Task[] tasks = new Task[rowNumber - 1];

        for (int i = 1; i < rowNumber; i++)
        {
            tasks[i - 1] = context.RowContexts[i - 1].TaskCompletion;
        }

        return tasks;
    }
}