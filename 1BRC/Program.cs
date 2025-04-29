
using System.Diagnostics;
using System.Text;

var values = new Dictionary<string, Station>();

var totalTime = new Stopwatch();
totalTime.Start();

// processing text
using (var stream = File.OpenRead("D:\\1brc\\1brc\\data\\measurements.txt"))
{
    int bufferSize = 128;
    using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, bufferSize))
    {
        string line;
        int intLine = 1;
        while ((line = streamReader.ReadLine()) != null)
        {
            var stationName = line.AsSpan(0,line.IndexOf(';')).ToString();
            var temperature = float.Parse(line.AsSpan(line.IndexOf(';') + 1));
            if (values.ContainsKey(stationName))
            {
                Station station = values[stationName];
                station.Values.Add(temperature);
            }
            else
            {
                Station station = new Station();
                station.Name = stationName;
                station.Values.Add(temperature);
                values[stationName] = station;
            }

            Console.WriteLine(intLine + " " + line);
            intLine++;
        }
    }
}

// calculate
var sortedResult = values.OrderBy(x => x.Key).ToList();
foreach (var result in sortedResult)
{
    Console.WriteLine(result.Value.ToString());
}

totalTime.Stop();
Console.WriteLine(totalTime.ElapsedMilliseconds + " ms");
Console.ReadKey();
// 1M Rows - 43903 ms, 48649 ms, 43335 ms
