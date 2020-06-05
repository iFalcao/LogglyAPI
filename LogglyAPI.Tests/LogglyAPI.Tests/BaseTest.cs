using LogglyAPI.Models;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace LogglyAPI.Tests
{
    public abstract class BaseTest
    {
        protected readonly LogglyConfig _logglyConfig;
        protected BaseTest()
        {
            _logglyConfig = this.ReadConfigFromFile();
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
