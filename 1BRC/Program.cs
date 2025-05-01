using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

int concurrencyLevel = Environment.ProcessorCount - 1;
var values = new ConcurrentDictionary<string, decimal[]>(concurrencyLevel, 1_000);

var totalTime = new Stopwatch();
var gen2 = GC.CollectionCount(2);
var gen1 = GC.CollectionCount(1);
var gen0 = GC.CollectionCount(0);
totalTime.Start();

// processing text
int bufferSize = 1024 * 1024;

var filePath = "D:\\1brc\\1brc\\data\\measurements.txt";

MinGCAllocApproach.Run(values, bufferSize, filePath);

//await SimpleApproachWithSpan.Run(values, bufferSize, filePath);

// calculate
totalTime.Stop();
var gen2Count = GC.CollectionCount(2) - gen2;
var gen1Count = GC.CollectionCount(1) - gen1;
var gen0Count = GC.CollectionCount(0) - gen0;


var calculateTime = new Stopwatch();
var gen2Calculate = GC.CollectionCount(2);
var gen1Calculate = GC.CollectionCount(1);
var gen0Calculate = GC.CollectionCount(0);
calculateTime.Start();

var sortedResult = values.OrderBy(x => x.Key).ToList();
var sb = new StringBuilder();
foreach (var result in sortedResult)
{
    var mean = result.Value[3] / result.Value[2];
    var min = result.Value[0];
    var max = result.Value[1];
    sb.AppendLine($"{result.Key}={min}/{mean}/{max}");
}
Console.WriteLine(sb.ToString());

calculateTime.Stop();
var gen2CountCalculate = GC.CollectionCount(2) - gen2Calculate;
var gen1CountCalculate = GC.CollectionCount(1) - gen1Calculate;
var gen0CountCalculate = GC.CollectionCount(0) - gen0Calculate;

Console.WriteLine("Processing file time: " + totalTime.ElapsedMilliseconds + " ms");
Console.WriteLine("Gen 2: " + gen2Count);
Console.WriteLine("Gen 1: " + gen1Count);
Console.WriteLine("Gen 0: " + gen0Count);
Console.WriteLine();

Console.WriteLine("Calculate and print time: " + calculateTime.ElapsedMilliseconds + " ms");
Console.WriteLine("Gen 2: " + gen2CountCalculate);
Console.WriteLine("Gen 1: " + gen1CountCalculate);
Console.WriteLine("Gen 0: " + gen0CountCalculate);


var audit = values.Sum(_ => _.Value[2]);
Console.WriteLine($"Total is row: {audit}");

Console.ReadKey();