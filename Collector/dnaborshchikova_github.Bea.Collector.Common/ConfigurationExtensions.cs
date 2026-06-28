using Microsoft.Extensions.Configuration;

namespace dnaborshchikova_github.Bea.Collector.Common
{
    public static class ConfigurationExtensions
    {
        public static T GetRequired<T>(this IConfiguration config, string sectionName
            , params string[] requiredKeys) where T : new()
        {
            var section = config.GetSection(sectionName);

            if (!section.Exists())
                throw new InvalidOperationException($"Секция {sectionName} не найдена");

            foreach (var key in requiredKeys)
            {
                if (string.IsNullOrWhiteSpace(section[key]))
                    throw new InvalidOperationException($"{key} не указан в {sectionName}");
            }

            var result = new T();
            section.Bind(result);

            return result;
        }

        public static T GetRequired<T>(this ConfigurationManager config, string sectionName
            , params string[] requiredKeys) where T : new() 
            => ((IConfiguration)config).GetRequired<T>(sectionName, requiredKeys);
    }
}
