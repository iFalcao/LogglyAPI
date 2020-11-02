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
            this._logglyConfig = this.ReadConfigFromFile();
            this._logglyClient = new LogglyClient(this._logglyConfig);
        }

        protected LogglyConfig ReadConfigFromFile()
        {
            var currentAssembly = this.GetType().GetTypeInfo().Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(currentAssembly);
            var fileName = Path.Combine(assemblyDirectory, @"Files\config.json");
            var json = File.ReadAllText(fileName);

            return JsonConvert.DeserializeObject<LogglyConfig>(json);
        }
    }
}
