using csdb.Database.Repositories;
using csdb.Database;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;

class Program
{
    private const int Iterations = 10; // Jumlah iterasi pengukuran
    private const int WarmUpIterations = 2; // Jumlah iterasi pemanasan

    // Struktur untuk menyimpan hasil benchmark
    class BenchmarkResult
    {
        public string Name { get; set; } = "";
        public long? TimeMs { get; set; }
        public long MemoryBytes { get; set; }
        public long MemoryDeltaBytes { get; set; }
    }

    static void Main()
    {
        DbConnector.Initialize();
        string tvfSqlScript = System.IO.File.ReadAllText("Database/tvf.sql");
        DbConnector.ExecuteSqlScript(tvfSqlScript);
        Console.WriteLine("== type '/exit' to exit ==");
        while (true)
        {
            Console.WriteLine(@"Menu
1. Basic Query (ORM vs ADO vs TVF)
2. Aggregate Query (ORM vs ADO vs TVF)
3. Detail Query (ORM vs ADO vs TVF)
4. Heavy Join Query (ORM vs ADO vs TVF)
5. Run All Benchmarks (ORM vs ADO vs TVF)"
            );

            Console.Write("Input: ");
            string? userInput = Console.ReadLine();

            if (userInput == null)
                continue;
            else if (userInput.Trim().ToLower() == "/exit")
                break;

            switch (userInput)
            {
                case "1":
                    RunComparison("Basic Query", OrmRepository.QueryBasic, AdoRepository.QueryBasic, TvfRepository.QueryBasic);
                    break;
                case "2":
                    RunComparison("Aggregate Query", OrmRepository.QueryAggregate, AdoRepository.QueryAggregate, TvfRepository.QueryAggregate);
                    break;
                case "3":
                    RunComparison("Detail Query", OrmRepository.QueryDetail, AdoRepository.QueryDetail, TvfRepository.QueryDetail);
                    break;
                case "4":
                    RunComparison("Heavy Join Query", OrmRepository.QueryHeavyJoin, AdoRepository.QueryHeavyJoin, TvfRepository.QueryHeavyJoin);
                    break;
                case "5":
                    RunAllBenchmarks();
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    static BenchmarkResult ExecuteAndBenchmark(string name, Func<int, int, long?> queryFunc)
    {
        // Force garbage collection to get a cleaner baseline for each run
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = Process.GetCurrentProcess().PrivateMemorySize64;

        long? time = queryFunc(Iterations, WarmUpIterations);
        long currentMemory = Process.GetCurrentProcess().PrivateMemorySize64;

        return new BenchmarkResult
        {
            Name = name,
            TimeMs = time,
            MemoryBytes = currentMemory,
            MemoryDeltaBytes = currentMemory - initialMemory
        };
    }

    static void PrintCombinedTable(BenchmarkResult ormResult, BenchmarkResult adoResult, BenchmarkResult tvfResult)
    {
        Console.WriteLine($"\n{"Metric",-20} | {"ORM",-15} | {"ADO.NET",-15} | {"TVF",-15}");
        Console.WriteLine($"--------------------|-----------------|-----------------|-----------------");

        // Time
        string ormTime = ormResult.TimeMs.HasValue ? $"{ormResult.TimeMs} ms" : "N/A";
        string adoTime = adoResult.TimeMs.HasValue ? $"{adoResult.TimeMs} ms" : "N/A";
        string tvfTime = tvfResult.TimeMs.HasValue ? $"{tvfResult.TimeMs} ms" : "N/A";
        Console.WriteLine($"{"Time (Avg)",-20} | {ormTime,-15} | {adoTime,-15} | {tvfTime,-15}");

        // Memory Delta
        string ormMemDelta = $"{ormResult.MemoryDeltaBytes / (1024.0 * 1024.0):F2} MB";
        string adoMemDelta = $"{adoResult.MemoryDeltaBytes / (1024.0 * 1024.0):F2} MB";
        string tvfMemDelta = $"{tvfResult.MemoryDeltaBytes / (1024.0 * 1024.0):F2} MB";
        Console.WriteLine($"{"Memory Delta" ,-20} | {ormMemDelta,-15} | {adoMemDelta,-15} | {tvfMemDelta,-15}");
    }

    static void PrintSummaryTable(List<BenchmarkResult> results)
    {
        Console.WriteLine();
        Console.WriteLine($"{"Query Type",-30} | {"Time (ms)",-15} | {"Memory (MB)",-15}");
        Console.WriteLine($"------------------------------|-----------------|-----------------");

        foreach (var result in results)
        {
            string time = result.TimeMs.HasValue ? $"{result.TimeMs}" : "N/A";
            string memory = $"{result.MemoryDeltaBytes / (1024.0 * 1024.0):F2}";
            Console.WriteLine($"{result.Name,-30} | {time,-15} | {memory,-15}");
        }
    }

    static void RunAllBenchmarks()
    {
        Console.WriteLine($"\n--- Running All Benchmarks (ORM, ADO.NET, TVF) ---");
        List<BenchmarkResult> results = new List<BenchmarkResult>();

        // Basic Query
        results.Add(ExecuteAndBenchmark("Basic Query (ORM)", OrmRepository.QueryBasic));
        results.Add(ExecuteAndBenchmark("Basic Query (ADO.NET)", AdoRepository.QueryBasic));
        results.Add(ExecuteAndBenchmark("Basic Query (TVF)", TvfRepository.QueryBasic));

        // Aggregate Query
        results.Add(ExecuteAndBenchmark("Aggregate Query (ORM)", OrmRepository.QueryAggregate));
        results.Add(ExecuteAndBenchmark("Aggregate Query (ADO.NET)", AdoRepository.QueryAggregate));
        results.Add(ExecuteAndBenchmark("Aggregate Query (TVF)", TvfRepository.QueryAggregate));

        // Detail Query
        results.Add(ExecuteAndBenchmark("Detail Query (ORM)", OrmRepository.QueryDetail));
        results.Add(ExecuteAndBenchmark("Detail Query (ADO.NET)", AdoRepository.QueryDetail));
        results.Add(ExecuteAndBenchmark("Detail Query (TVF)", TvfRepository.QueryDetail));

        // Heavy Join Query
        results.Add(ExecuteAndBenchmark("Heavy Join Query (ORM)", OrmRepository.QueryHeavyJoin));
        results.Add(ExecuteAndBenchmark("Heavy Join Query (ADO.NET)", AdoRepository.QueryHeavyJoin));
        results.Add(ExecuteAndBenchmark("Heavy Join Query (TVF)", TvfRepository.QueryHeavyJoin));

        Console.WriteLine($"\n--- All Benchmarks Complete ---");
        PrintSummaryTable(results);
    }

    static void RunComparison(string queryName, Func<int, int, long?> ormFunc, Func<int, int, long?> adoFunc, Func<int, int, long?> tvfFunc)
    {
        Console.WriteLine($"\n--- Benchmarking {queryName} ---");
        var ormResult = ExecuteAndBenchmark($"{queryName} (ORM)", ormFunc);
        var adoResult = ExecuteAndBenchmark($"{queryName} (ADO.NET)", adoFunc);
        var tvfResult = ExecuteAndBenchmark($"{queryName} (TVF)", tvfFunc);

        PrintCombinedTable(ormResult, adoResult, tvfResult);
    }
}