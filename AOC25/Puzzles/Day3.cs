using System.Diagnostics;
using Serilog;
using Serilog.Core;

namespace AOC25.Puzzles;

public class Day3
{
    private readonly string _path;
    private readonly Stopwatch _stopwatch = new ();
    public Day3(bool test)
    {

        if (test)
        {
            _path = "test-input/day3.txt";
            return;
        }
        _path = "input/day3.txt";
    }

    public void Run()
    {
        var lines = File.ReadAllLines(_path).AsSpan();
        long part1 = 0;
        long part2 = 0;
        _stopwatch.Start();

        foreach (var line in lines)
        {
            part1 += MaxJoltageNBatteries(line, 2);
        }
        
        _stopwatch.Stop();
        Log.Logger.Information("Part 1: {result}", part1);
        Log.Logger.Information("Executed in {ms} ms", _stopwatch.ElapsedMilliseconds);
        _stopwatch.Restart();
        foreach (var line in lines)
        {
            part2 += MaxJoltageNBatteries(line, 12);
        }
        _stopwatch.Stop();
        Log.Logger.Information("Part 2: {result}", part2);
        Log.Logger.Information("Executed in {ms} ms", _stopwatch.ElapsedMilliseconds);
    }
    
    private static long MaxJoltageNBatteries(string line, int nBatteries)
    {
        var ret = new List<char>(nBatteries);
        int start = 0;

        for (int pick = 0; pick < nBatteries; pick++)
        {
            int remaining = nBatteries - pick;
            int lastIndex = line.Length - remaining;
            
            var best = line
                .Skip(start)
                .Take(lastIndex - start + 1)
                .Select((c, idx) => new { c, idx = start + idx })
                .MaxBy(x => x.c);
            Log.Logger.Verbose("Best: {@best}", best);
            if (best is null)
            {
                throw new ArgumentException("lol");
            }
            ret.Add(best.c);
            start = best.idx + 1;
        }
        return long.Parse(ret.ToArray());
    }
}

/*
811111111111119
234234234234278
818181911112111
 * 
 */