using dnaborshchikova_github.Bea.Generator;
using dnaborshchikova_github.Bea.Generator.Validators;
using Microsoft.Extensions.Configuration;

Console.Error.WriteLine("Генератор запущен.");

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var settingsService = new GeneratorSettingsService(config);
var settings = settingsService.GetSettings();

var validator = new GeneratorSettingsValidator();
validator.ValidateGeneratorSettings(settings);

var runner = new AppRunner(settings);
var folderPath = AppContext.BaseDirectory;
var filePath = runner.Generate(folderPath);

Console.Error.WriteLine("Генерация завершена");
Console.Out.WriteLine(filePath);