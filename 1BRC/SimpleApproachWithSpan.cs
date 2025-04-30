using System.Text;
using System.Threading.Tasks;

public static class SimpleApproachWithSpan
{
    public static async Task Run(Dictionary<string, Station> values, int bufferSize, string filePath)
    {
        Console.WriteLine("Running...");
        using (var stream = File.OpenRead(filePath))
        {
            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, bufferSize))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var stationName = line.AsSpan(0, line.IndexOf(';')).ToString();
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
                }
            }
        }
    }
}
/** Results
============ 1M lines ==========
47499 ms
Gen 2: 4
Gen 1: 27
Gen 0: 65

---
43471 ms
Gen 2: 5
Gen 1: 15
Gen 0: 33

---
42275 ms
Gen 2: 5
Gen 1: 18
Gen 0: 51
----

976 ms
Gen 2: 5
Gen 1: 32
Gen 0: 49

=====================
 */
