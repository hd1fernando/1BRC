using System.Text;

public static class MinGCAllocApproach
{
    public static void Run(Dictionary<string, Station> values, int bufferSize, string filePath)
    {
        Console.WriteLine("Running...");
        var rawBuffer = new byte[bufferSize];
        using (var fs = File.OpenRead(filePath))
        {
            var bytesBuffered = 0;
            var bytesConsumed = 0;
            while (true)
            {
                var count = rawBuffer.Length - bytesBuffered;
                var bytesRead = fs.Read(rawBuffer, bytesBuffered, count);
                if (bytesRead == 0)
                    break;

                bytesBuffered += bytesRead;
                int linePosition;

                do
                {
                    linePosition = Array.IndexOf(rawBuffer, (byte)'\n', bytesConsumed, (bytesBuffered - bytesConsumed));
                    if (linePosition >= 0)
                    {
                        var lineLenght = linePosition - bytesConsumed;
                        var line = new Span<byte>(rawBuffer, bytesConsumed, lineLenght);
                        bytesConsumed += lineLenght + 1;

                        string? stationName = Encoding.UTF8.GetString(line.Slice(0, line.IndexOf((byte)';')));
                        var temperature = decimal.Parse(line.Slice(line.IndexOf((byte)';') + 1));

                        if (values.ContainsKey(stationName))
                        {
                            Station station = values[stationName];
                            station.AddValue(temperature);
                        }
                        else
                        {
                            Station station = new Station();
                            station.Name = stationName;
                            station.AddValue(temperature);
                            values[stationName] = station;
                        }

                    }
                } while (linePosition >= 0);

                Array.Copy(rawBuffer, bytesConsumed, rawBuffer, 0, (bytesBuffered - bytesConsumed));
                bytesBuffered -= bytesConsumed;
                bytesConsumed = 0;
            }
        }
    }
}

/* Results:
============ 1M lines ==========

42728 ms
Gen 2: 4
Gen 1: 14
Gen 0: 33
------------
42742 ms
Gen 2: 4
Gen 1: 15
Gen 0: 33

------------
901 ms
Gen 2: 1
Gen 1: 3
Gen 0: 9
-----------

Processing file time: 2184 ms
Gen 2: 0
Gen 1: 1
Gen 0: 10
Calculate and print time: 978 ms

================================

 */