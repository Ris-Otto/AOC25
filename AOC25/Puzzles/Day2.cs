using System.Diagnostics;
using Serilog;

namespace AOC25.Puzzles;

public class Day2
{
    private readonly string _path;
    private readonly Stopwatch _stopwatch = new ();
    public Day2(bool test)
    {

        if (test)
        {
            _path = "test-input/day2.txt";
            return;
        }
        _path = "input/day2.txt";
    }

    public void Run()
    {
        var lines = File.ReadAllLines(_path);
        long res = 0;
        long res2 = 0;
        _stopwatch.Start();
        foreach (var line in lines)
        {
            var split = line.Split(',');
            foreach (var idRange in split)
            {
                res += PartOne(idRange);
                res2 += PartTwo(idRange);

            }
        }
        
        _stopwatch.Stop();
        Log.Logger.Information("Result: {res}", res);
        Log.Logger.Information("Result2: {res}", res2);
        Log.Logger.Information("Day2: {ms} ms", _stopwatch.Elapsed.TotalMilliseconds);
    }

    private static long PartOne(string line)
    {
        var a = new IdRange(line);
        var sum = a.InvalidValue();
        Log.Logger.Debug("ID {id} - sum of invalid IDs: {sum}", line,sum);
        return sum;
    }

    private static long PartTwo(string line)
    {
        var a = new IdRange(line);
        var sum =  a.Enumerate(minChunkSize: 1);
        Log.Logger.Debug("ID {id} - sum of invalid IDs: {sum}", line,sum);
        return sum;
    }
}

public readonly struct IdRange
{
    private readonly string _first;
    private readonly string _second;

    private readonly long _start;
    private readonly long _end;

    public IdRange(string separated)
    {
        var  split = separated.Split('-');
        _first = split[0];
        _second = split[1];
        _start = long.Parse(split[0]);
        _end = long.Parse(split[1]);
    }
    
    //Special case
    public long InvalidValue()
    {
        long result = 0;
        if (_first.Length % 2 != 0)
        {
            if (_second.Length == _first.Length) return result;
            var log = Math.Log(_start, 10);
                
            var firstRelevant = (int)Math.Pow(10, Math.Ceiling(log));
            result += Enumerate(firstRelevant, _end);
        }
        else
        {
            if (_second.Length != _first.Length)
            {
                var log = Math.Log(_end, 10);
                var lastRelevant = (int)Math.Pow(10, Math.Floor(log));
                result += Enumerate(_start, lastRelevant);

                return result;
            }
            //First and second are equal in length, and both are divisible by 2 -> can enumerate without special cases
            result = Enumerate(_start, _end);
        }
        return result;
    }

    private static long Enumerate(long start, long end)
    {
        long result = 0;

        for (var i = start; i <= end; i++)
        {
            var a = i.ToString();
            var len = a.Length;
            if (len == 1) continue;

            List<int> sizes = [a.Length / 2];

            result = Result(sizes, a.Length / 2, a, result, i);
        }

        return result;
    }
    
    public long Enumerate(int minChunkSize)
    {
        long result = 0;

        for (var i = _start; i <= _end; i++)
        {
            var a = i.ToString();
            var len = a.Length;
            if (len == 1) continue;

            var sizes = GetChunkSizes(len);

            result = Result(sizes, minChunkSize, a, result, i);
        }

        return result;
    }

    private static long Result(List<int> sizes, int minChunk, string a, long result, long i)
    {
        int index = 0;
        while (index < sizes.Count && sizes[index] >= minChunk)
        {
            var chunkSize = sizes[index];
            index++;

            var span = a.AsSpan();

            var allEqual = true;
            for (var k = chunkSize; k < span.Length; k += chunkSize)
            {
                var prev = span.Slice(k - chunkSize, chunkSize);
                var curr = span.Slice(k, chunkSize);

                if (prev.SequenceEqual(curr)) continue;
                allEqual = false;
                break;
            }

            if (!allEqual || span.Length <= chunkSize) continue;
            result += i;
            break;
        }

        return result;
    }
    
    private static readonly Dictionary<int, List<int>> ChunkSizeCache = new();

    private static List<int> GetChunkSizes(int length)
    {
        if (ChunkSizeCache.TryGetValue(length, out var sizes))
            return sizes;
        
        sizes = [length];

        // Half
        var half = length / 2;
        if (length % half == 0)
            sizes.Add(half);

        // Remaining divisors
        for (var s = half - 1; s >= 1; s--)
            if (length % s == 0)
                sizes.Add(s);

        ChunkSizeCache[length] = sizes;
        return sizes;
    }
}