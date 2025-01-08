using BackgroundWorker;
using BackgroundWorker.Helpers;
using BackgroundWorker.Interfaces;
using BackgroundWorker.TaskContexts;

namespace BackgroundWorker.ConsoleTest;

class Program
{
    private object _contextLock = new object();

    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();

        Console.WriteLine("Press 'q' to stop execution.");
        Task.Run(() =>
        {
            while (true)
            {
                if (Console.ReadKey(true).Key != ConsoleKey.Q) continue;
                cts.Cancel();
                break;
            }
        });

        try
        {
            ITaskHandlerHelper taskHandlerHelper = new TaskHandlerHelper();
            IPrintFormService printFormService = new PrintFormService();
            ITaskMatrixHandler taskMatrixHandler = new TaskMatrixHandler(taskHandlerHelper, printFormService);
            
            var progress = new Progress<GeneralTaskContext>(context =>
            {
                lock (taskMatrixHandler)
                {
                    foreach (var rowContext in context.RowContexts)
                    {
                        Console.WriteLine($"{rowContext.StatusMessage}");
                    }

                    Console.WriteLine($"General Status: {context.StatusMessage}");
                    Console.WriteLine($"---------------------------------------");
                }
            });

            await taskMatrixHandler.ExecuteTaskMatrixAsync(employmentIds: [1, 2, 3, 4, 5], formTypeIds: [1, 2],
                pdfTypeIds: [3, 4], cts.Token, progress);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Execution stopped by user.");
        }
    }
}