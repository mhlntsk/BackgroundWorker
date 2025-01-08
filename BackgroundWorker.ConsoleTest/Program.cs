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
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);

                    foreach (var rowContext in context.RowContexts)
                    {
                        Console.ForegroundColor = GetColorByStatus(rowContext.StatusMessage);
                        Console.WriteLine($"{rowContext.StatusMessage}");
                    }

                    Console.ResetColor();
                    Console.WriteLine($"General Status: {context.StatusMessage}");
                    Console.WriteLine($"---------------------------------------");
                }
            });

            await taskMatrixHandler.ExecuteTaskMatrixAsync(employmentIds: [1, 2, 3], formTypeIds: [1, 2],
                pdfTypeIds: [3, 4], cts.Token, progress);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Execution stopped by user.");
        }
    }

    static ConsoleColor GetColorByStatus(string statusMessage)
    {
        if (statusMessage.Contains("GetPersonViewModel")) return ConsoleColor.Yellow;
        if (statusMessage.Contains("Report")) return ConsoleColor.Yellow;
        if (statusMessage.Contains("GenerateDocx")) return ConsoleColor.Cyan;
        if (statusMessage.Contains("ConvertToPdf")) return ConsoleColor.DarkGreen;
        if (statusMessage.Contains("Print")) return ConsoleColor.Blue;
        if (statusMessage.Contains("Failed")) return ConsoleColor.Red;
        if (statusMessage.Contains("Completed")) return ConsoleColor.Green;
        return ConsoleColor.White;
    }
}