// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using AOC25.Puzzles;
using Serilog;
using Serilog.Core;
using Serilog.Events;


var rootCommand = new RootCommand();

Option<string> verbosityOption = new("--verbosity", "-v")
{
    Description = "Verbosity level, Q[uiet], M[inimal], N[ormal], D[etailed], Diag[nostic]. Default is N[ormal]",
};
verbosityOption.AcceptOnlyFromAmong(
    "Q", "q",
    "M", "m",
    "N", "n",
    "D", "d",
    "Diag", "diag");

var day1command = new Command("day1");
var day2command = new Command("day2");

var testOption = new Option<bool>("--test", "-t");

day1command.Add(testOption);
day1command.Add(verbosityOption);

day2command.Add(testOption);
day2command.Add(verbosityOption);

rootCommand.Add(day1command);
rootCommand.Add(day2command);

rootCommand.Add(testOption);
rootCommand.Add(verbosityOption);

rootCommand.SetAction(async parseResult =>
{
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    var isTest = parseResult.GetValue(testOption);
    await RunAll(isTest);
});

day1command.SetAction(parseResult =>
{
    
    var isTest = parseResult.GetValue(testOption);
    var day1 = new Day1(isTest);
    day1.Run();
});

var parseResult = rootCommand.Parse(args);
Log.Logger = new LoggerConfiguration().WriteTo.Console()
    .MinimumLevel.ControlledBy(GetLogLevel(parseResult.GetValue(verbosityOption))).CreateLogger();
        
await parseResult.InvokeAsync();


Task RunAll(bool test)
{
    var day1 = new Day1(test);
    day1.Run();
    return Task.CompletedTask;
}


static LoggingLevelSwitch GetLogLevel(string? verbosity)
{
    return verbosity switch
    {
        "diag" or "Diag" => new LoggingLevelSwitch(LogEventLevel.Verbose),
        "d" or "D" => new LoggingLevelSwitch(LogEventLevel.Debug),
        "n" or "N" => new LoggingLevelSwitch(),
        "m" or "M" => new LoggingLevelSwitch(LogEventLevel.Warning),
        "q" or "Q" => new LoggingLevelSwitch(LogEventLevel.Fatal),
        _ => new LoggingLevelSwitch()
    };
}