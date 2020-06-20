using LogglyAPI.Configuration;
using LogglyAPI.Contracts;
using LogglyAPI.Services;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace LogglyAPI.Tests
{
    public abstract class BaseTest
    {
        protected readonly LogglyConfig _logglyConfig;
        protected readonly ILogglyClient _logglyClient;

        protected BaseTest()
        {
            _logglyConfig = ReadConfigFromFile();
            _logglyClient = new LogglyClient(_logglyConfig);
        }

        protected LogglyConfig ReadConfigFromFile()
        {
            var currentAssembly = GetType().GetTypeInfo().Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(currentAssembly);
            var fileName = Path.Combine(assemblyDirectory, @"Files\config.json");
            var json = File.ReadAllText(fileName);

            return JsonConvert.DeserializeObject<LogglyConfig>(json);
        }
    }
}
