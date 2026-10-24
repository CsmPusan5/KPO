using System;
using Hw1.Domain;

namespace Hw1.Application
{
    public class App(Park park, IParser parser, IReport report)
    {
        private readonly Park _park = park;
        private readonly IParser _parser = parser;
        private readonly IReport _report = report;

        public void Run()
        {
            Console.WriteLine("=== Сервис кикшеринга 'ВышКат' ===");
            Console.WriteLine("Введите 'help' для списка доступных команд.\n");

            bool isRunning = true;

            while (isRunning)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (input is null)
                {
                    Console.WriteLine("\nЗавершение работы...");
                    break;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                try
                {
                    ICommand command = _parser.Parse(input);
                    isRunning = command.Execute(_park, _report);
                }
                catch (ParseException ex)
                {
                    Console.WriteLine($"Ошибка ввода: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
                }
            }
        }
    }
}