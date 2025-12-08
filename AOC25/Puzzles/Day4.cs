using System.Diagnostics;
using System.Text;
using Serilog;
using Serilog.Events;

namespace AOC25.Puzzles;

public class Day4
{
    private readonly string _path;
    private readonly Stopwatch _stopwatch = new ();
    private const char At = '@';
    public Day4(bool test)
    {
        if (test)
        {
            _path = "test-input/day4.txt";
            return;
        }
        _path = "input/day4.txt";
    }

    public void Run()
    {
        var lines =  File.ReadAllLines(_path);
        long result = 0;

        _stopwatch.Start();
        var temp = CalcAdjacency(lines);
        result += temp.Length;
        
        _stopwatch.Stop();
        Log.Logger.Information("Part1: {rolls}", result);
        Log.Logger.Information("Executed in {ms} ms", _stopwatch.Elapsed.TotalMilliseconds);

        _stopwatch.Restart();
        var next = RemoveToiletPaper(temp, lines);
        long result2 = result;
        while (temp.Length > 0)
        {
            if(Log.IsEnabled(LogEventLevel.Debug))
            {
                foreach (var se in next)
                {
                    Log.Logger.Debug("{a}", se);
                }

                Log.Logger.Debug("*****************");
            }
            temp = CalcAdjacency(next);
            result2 += temp.Length;
            next = RemoveToiletPaper(temp, next);
        }
        _stopwatch.Stop();
        Log.Logger.Information("Part2: {rolls}", result2);
        Log.Logger.Information("Executed in {ms} ms", _stopwatch.Elapsed.TotalMilliseconds);
    }

    static string[] RemoveToiletPaper(IntVector2[] paperLocations, string[] lines)
    {
        foreach (var p in paperLocations)
        {
            StringBuilder sb = new (lines[p.Y])
            {
                [p.X] = 'x'
            };
            lines[p.Y] = sb.ToString();
        }
        return lines;
    }

    IntVector2[] CalcAdjacency(string[] lines)
    {
        List<IntVector2> ret = [];
        for (int y = 0; y < lines.Length; y++)
        {
            string? linea = null;
            string? lineb = null;
            string line = lines[y];
            
            if (y != 0)
            {
                linea = lines[y - 1];
            }
            if(y != lines.Length-1)
            {
                lineb =  lines[y + 1];
            }
            for(int x = 0; x < line.Length; x++)
            {
                var c = line[x];
                if (c != At) continue;
                
                var below = lineb?[x]??'.';
                var above = linea?[x]??'.';
                char diagla = '.';
                char diaglb = '.';
                char l = '.';
                if (x != 0)
                {
                    diagla = linea?[x - 1]??'.';
                    diaglb = lineb?[x - 1]??'.';
                    l = line[x - 1];
                }

                char diagra = '.';
                char diagrb = '.';
                char r = '.';
                if (x != lines[y].Length - 1)
                {
                    diagra = linea?[x + 1]??'.';
                    diagrb = lineb?[x + 1]??'.';
                    r = lines[y][x +1];
                }
                int adjs = Convert.ToInt32(below == At)
                          + Convert.ToInt32(above == At)
                          + Convert.ToInt32(diagla == At)
                          + Convert.ToInt32(diagra == At)
                          + Convert.ToInt32(diaglb == At)
                          + Convert.ToInt32(diagrb == At)
                          + Convert.ToInt32(l == At)
                          + Convert.ToInt32(r == At);
                if(Log.Logger.IsEnabled(LogEventLevel.Verbose))
                {
                    Log.Logger.Verbose("\n{0}{1}{2}\n{3}{4}{5}\n{6}{7}{8}", diagla, above, diagra, l, c, r, diaglb, below,
                        diagrb);
                    Log.Logger.Verbose("Adjacent = {a}", adjs);
                }
                if (adjs < 4)
                {
                    ret.Add(new IntVector2(x, y));
                }
            }
        }
        return ret.ToArray();
    }
}

internal struct IntVector2(int x, int y)
{
    public int X = x;
    public int Y = y;
}