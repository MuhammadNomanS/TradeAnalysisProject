// See https://aka.ms/new-console-template for more information
using TradeAnalysisProject;
using System.Threading;

//Console.WriteLine("Hello, World!");

var name = typeof(System.Collections.Generic.List<string>).AssemblyQualifiedName;
string configKey = null;
DateOnly startDate, endDate;
if (args.Length > 0)
{
    configKey = args[0].ToString().ToLower();
    if (args.Length > 1)
    {
        startDate = DateOnly.ParseExact(args[1], "yyyyMMdd");
        if (args.Length > 2)
            endDate = DateOnly.ParseExact(args[2], "yyyyMMdd");
        else
            endDate = startDate.AddDays(1);
    }
    else
    {
        startDate = DateOnly.FromDateTime(DateTime.Now);
        endDate = startDate.AddDays(1);
    }

    
}

var dataConfigs = new DataConfigurations();
dataConfigs.LoadConfigs(out DataConfigurations dataConfigurations);

foreach (var dataConfig in dataConfigurations.DataConfigurationCollection)
{
    if (dataConfig.Key == configKey)
    {
        LoadData(dataConfig, startDate, endDate);
    }
}


static void LoadData(DataConfiguration _dataConfig, DateOnly startDate, DateOnly endDate)
{
    //var currentDate = startDate;
    var days = Enumerable
    .Range(0, (endDate.DayNumber - startDate.DayNumber)) // check the rounding
    .Select(i => startDate.AddDays(i));

    Parallel.ForEach(days, new ParallelOptions { MaxDegreeOfParallelism= _dataConfig.degreeOfParellelism}, currentDate =>
    {
        for (int i = 0; i < _dataConfig.fixInFiles.Count(); i++)
        {
            FIXLogFileTransformer.FIXCleanup(_dataConfig, currentDate, i);
        }
    });
}