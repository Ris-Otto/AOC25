using System.Numerics;
using Serilog;

namespace AOC25.Puzzles;

public class Day1
{
    private readonly string _path;
    private const int InitialPointer = 50;
    public Day1(bool test)
    {
        if (test)
        {
            _path = "test-input/day1.txt";
            return;
        }
        _path = "input/day1.txt";
    }

    
    public void Run()
    {
        var lines =  File.ReadAllLines(_path);
        int p = InitialPointer;
        var zeroes1 = 0;
        var zeroes2 = 0;
        foreach (var line in lines)
        {
            var command = ParseCommand(line);
            var temp = CurrentPointer(p, command, out var hitZero);
            zeroes2 += hitZero;

            Log.Logger.Debug("Pointer moved from {last} to {current} by command {command} and moved past zero {zero} times"
                , p, temp, line, hitZero);
            if (temp % 100 == 0)
            {
                zeroes1++;
            }
            p = temp;
            
        }
        Log.Logger.Information("Password: {zeroes}", zeroes1);
        Log.Logger.Information("Password2: {zeroes2}", zeroes2);
        
    }

    private static int ParseCommand(string line)
    {
        var direction = line[..1];
        var magnitude = int.Parse(line[1..]);
        if (direction == "L")
        {
            magnitude *= -1;
        }
        return magnitude;
    }

    private static int CurrentPointer(int last, int command, out int hitZero)
    {
        hitZero = 0;
        var cSign = Math.Sign(command);
        var h = Math.Abs(command);
        var g = Math.Abs(last);
        (int thousands, int hundreds) = (h / 1000, h / 100 % 10);
        (int cte, int lte) = (h / 10 % 10, g / 10 % 10);
        (int co, int lo) = (h % 10, g % 10);
        
        hitZero += hundreds + thousands * 10;
        
        int result = (cSign *cte + lte) * 10 + cSign * co + lo;
        Log.Logger.Debug("Result: {result}", result);
        if (result > 99 || (result <= 0 && last != 0))
        {
            hitZero++;
        }
        if (result > 99)
        {
            result -= 100;
        }
        if(result < 0)
        {
            result += 100;
        }
        return result;
    }
}