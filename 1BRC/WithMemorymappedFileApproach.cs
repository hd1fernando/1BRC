using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;
using System.Text;

public static class WithMemorymappedFileApproach
{
    public static void Run(ConcurrentDictionary<string, decimal[]> values, int buffersize, string filePath)
    {
        Console.WriteLine("Running...");

        using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        var length = new FileInfo(filePath).Length;
        var buffer = new byte[buffersize];
        long position = 0;

        (int start, int end) stationNamePostions = (0, 0);
        (int start, int end) temperaturePostions = (0, 0);

        byte[] lineByte = new byte[100];
        int lineBytePointer = 0;

        while (position < length)
        {
            var bytesToRead = Math.Min(buffer.Length, length - position);
            accessor.ReadArray(position, buffer, 0, (int)bytesToRead);

            for (int i = 0; i < bytesToRead; i++)
            {
                var eol = (char)buffer[i];
                if (eol == '\n')
                {
                    var line = new Span<byte>(lineByte, 0, lineBytePointer + 1);
                    var indexOfComma = line.IndexOf((byte)';');
                    var stationName = Encoding.UTF8.GetString(line.Slice(0, indexOfComma));
                    var lengthOfTemperature = lineBytePointer - indexOfComma - 1;
                    var temperature = decimal.Parse(line.Slice(indexOfComma + 1, lengthOfTemperature));
                    ComputeTemperature(values, stationName, temperature);
                    lineBytePointer = 0;
                }
                else
                {
                    lineByte[lineBytePointer++] = buffer[i];
                }
            }
            position += bytesToRead;
        }
    }

    private static void ComputeTemperature(ConcurrentDictionary<string, decimal[]> values, string stationName, decimal temperature)
    {

        if (!values.ContainsKey(stationName))
        {
            var station = new decimal[4];
            // min
            station[0] = temperature;
            // max
            station[1] = temperature;
            // numOfValues
            station[2] = 1;
            // sum
            station[3] = temperature;

            var result = values.TryAdd(stationName, station);
            if (result == false)
                throw new InvalidOperationException();
        }
        else
        {
            var station = values[stationName];
            // min
            station[0] = Math.Min(temperature, station[0]);
            // max
            station[1] = Math.Max(temperature, station[1]);
            // numOfValues
            station[2] += 1;
            // sum
            station[3] += temperature;

            values[stationName] = station;
        }
    }
}

/* Results:
============ 1M lines ==========
Processing file time: 904 ms
Gen 2: 0
Gen 1: 1
Gen 0: 33

Calculate and print time: 1006 ms
Gen 2: 0
Gen 1: 0
Gen 0: 1
Total is row: 1000000
----------------------------
Processing file time: 896 ms
Gen 2: 0
Gen 1: 1
Gen 0: 18

Calculate and print time: 1074 ms
Gen 2: 0
Gen 1: 0
Gen 0: 0
Total is row: 1000000

===============1BR===============

Processing file time: 534472 ms
Gen 2: 6
Gen 1: 90
Gen 0: 36348

Calculate and print time: 1047 ms
Gen 2: 0
Gen 1: 0
Gen 0: 1
Total is row: 1_000_000_000

 */