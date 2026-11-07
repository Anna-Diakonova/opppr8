using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AsyncConsoleVariant7
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Варіант 7: Асинхронна обробка рядків у консолі";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=============================================================");
                Console.WriteLine("  ВАРІАНТ 7: Асинхронна обробка консольного вводу (C#)");
                Console.WriteLine("=============================================================");
                Console.WriteLine("1. Інтерактивний режим (Нез блоковане асинхронне введення)");
                Console.WriteLine("2. Порівняльний аналіз ефективності (Sequential vs Async)");
                Console.WriteLine("0. Вихід");
                Console.Write("\nОберіть режим (0-2): ");

                string choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        await RunInteractiveAsyncMode();
                        break;
                    case "2":
                        await RunEfficiencyBenchmark();
                        break;
                    default:
                        Console.WriteLine("⚠️ Невірний вибір. Натисніть Enter...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        #region 1. Інтерактивний асинхронний режим
        private static async Task RunInteractiveAsyncMode()
        {
            Console.Clear();
            Console.WriteLine("=== 1. ІНТЕРАКТИВНИЙ АСИНХРОННИЙ РЕЖИМ ===");
            Console.WriteLine("• Вводьте будь-які рядки текста.");
            Console.WriteLine("• Обчислення виконуються в тлі; консоль НЕ блокується.");
            Console.WriteLine("• Введіть 'exit' для завершення.\n");

            int lineId = 0;
            List<Task> activeTasks = new List<Task>();

            while (true)
            {
                Console.Write($"[Рядок #{lineId + 1}] > ");
                string input = Console.ReadLine();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("⚠️ Порожній рядок пропущено.");
                    continue;
                }

                lineId++;

                Task task = ProcessLineAsync(input, lineId, showOutput: true);
                activeTasks.Add(task);
            }

            if (activeTasks.Count > 0)
            {
                Console.WriteLine("\n⏳ Очікування завершення всіх фонових обчислень...");
                await Task.WhenAll(activeTasks);
                Console.WriteLine("✅ Усі асинхронні завдання завершено!");
            }

            Console.WriteLine("\nНатисніть Enter для повернення в меню...");
            Console.ReadLine();
        }
        #endregion

        #region 2. Блок порівняльного аналізу (Sequential vs Async)
        private static async Task RunEfficiencyBenchmark()
        {
            Console.Clear();
            Console.WriteLine("=== 2. АНАЛІЗ ЕФЕКТИВНОСТІ ОБРОБКИ ===");

            List<string> testDataSet = new List<string>
            {
                "High Performance Computing in .NET",
                "Asynchronous Programming Model",
                "Non-blocking Console Input Handling",
                "Parallel Processing Benchmark Data",
                "System Threading Tasks TaskRun",
                "DotNet Core Async Standard Implementation",
                "Concurrency and Reactive Programming",
                "Multithreading vs Async Await Task"
            };

            Console.WriteLine($"Кількість тестових рядків: {testDataSet.Count}");
            Console.WriteLine("Навантаження: I/O затримка (300 мс) + CPU 100,000 ітерацій SHA-256 на рядок.\n");

            Console.WriteLine("▶ 1. Запуск ПОСЛІДОВНОЇ обробки...");
            Stopwatch swSeq = Stopwatch.StartNew();
            for (int i = 0; i < testDataSet.Count; i++)
            {
                await ProcessLineAsync(testDataSet[i], i + 1, showOutput: false);
            }
            swSeq.Stop();
            long seqTimeMs = swSeq.ElapsedMilliseconds;
            Console.WriteLine($"⏱ Час послідовної обробки: {seqTimeMs} мс");

            Console.WriteLine("\n▶ 2. Запуск АСИНХРОННОЇ (ПАРАЛЕЛЬНОЇ) обробки...");
            Stopwatch swAsync = Stopwatch.StartNew();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < testDataSet.Count; i++)
            {
                tasks.Add(ProcessLineAsync(testDataSet[i], i + 1, showOutput: false));
            }
            await Task.WhenAll(tasks);
            swAsync.Stop();
            long asyncTimeMs = swAsync.ElapsedMilliseconds;
            Console.WriteLine($"⏱ Час асинхронної обробки: {asyncTimeMs} мс");

            double speedup = (double)seqTimeMs / (asyncTimeMs > 0 ? asyncTimeMs : 1);
            int coreCount = Environment.ProcessorCount;
            double efficiency = (speedup / coreCount) * 100;

            Console.WriteLine("\n================ РЕЗУЛЬТАТИ АНАЛІЗУ ================");
            Console.WriteLine($"Послідовний час (T_seq):     {seqTimeMs,8} мс");
            Console.WriteLine($"Асинхронний час (T_async):   {asyncTimeMs,8} мс");
            Console.WriteLine($"Коефіцієнт прискорення (S):  {speedup,8:F2}x");
            Console.WriteLine($"Доступно ядер CPU (N):       {coreCount,8}");
            Console.WriteLine($"Відносна ефективність (E):   {efficiency,8:F1}%");
            Console.WriteLine("====================================================");

            Console.WriteLine("\nНатисніть Enter для повернення в меню...");
            Console.ReadLine();
        }
        #endregion

        #region Логіка обчислення рядка (Task Execution)
        private static async Task ProcessLineAsync(string line, int lineId, bool showOutput)
        {
            if (showOutput)
            {
                Console.WriteLine($"  [ID #{lineId}] 🚀 Запущено обробку: '{line}'");
            }

            await Task.Delay(300);

            string resultHash = await Task.Run(() => ComputeCpuIntenseHash(line));

            if (showOutput)
            {
                Console.WriteLine($"  [ID #{lineId}] ✅ Завершено! Хеш: {resultHash.Substring(0, 16)}... (Довжина: {line.Length})");
            }
        }

        private static string ComputeCpuIntenseHash(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                for (int i = 0; i < 100_000; i++)
                {
                    bytes = sha256.ComputeHash(bytes);
                }
                return Convert.ToHexString(bytes);
            }
        }
        #endregion
    }
}