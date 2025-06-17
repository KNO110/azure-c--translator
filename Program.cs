using Microsoft.Extensions.Configuration;

var cfg = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false)
          .Build();

var translator = new TranslatorService(cfg);

Console.Write("Текст для перевода: ");
var text = Console.ReadLine();

Console.Write("На какой перевести?: (en, fr, de…): ");
var to = Console.ReadLine();

var result = await translator.TranslateAsync(text!, to!);
Console.WriteLine($"-> {result}");
