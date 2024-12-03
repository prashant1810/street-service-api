using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WeightCalculator.JsonConfigData
{
    public static class JsonConfigLoader
    {

        public static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var jsonConfigFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonConfigData", "Json");

            var configClasses = assembly.GetTypes()
                .Where(t => t.IsClass && t.GetCustomAttributes(typeof(JsonConfigAttribute), true).Length > 0);

            foreach (var configClass in configClasses)
            {
                string filePath = Path.Combine(jsonConfigFolderPath, $"{configClass.Name}.json");
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"The file {filePath} has not been found.");
                }

                string json = File.ReadAllText(filePath);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                foreach (var property in configClass.GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    if (dictionary.TryGetValue(property.Name, out var value))
                    {
                        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        var safeValue = (value == null) ? null : Convert.ChangeType(value, targetType);
                        property.SetValue(null, safeValue);
                    }
                }
            }
        }
    }

}
