using System.Globalization;

public class Parser : IParser
{
    public ICommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ParseException("Пустая команда");

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLowerInvariant();

        return command switch
        {
            "help"   => new HelpCommand(),
            "add"    => ParseAdd(parts),
            "report" => new ShowReportCommand(),
            "list"   => new ShowItemsCommand(),
            "easy"   => new ShowEasyCommand(),
            "exit"   => new ExitCommand(),
            _        => throw new ParseException($"Неизвестная команда: {command}")
        };
    }

    private static ICommand ParseAdd(string[] parts)
    {
        if (parts.Length < 5)
            throw new ParseException("Формат: add <type> <number> <name> <easyScore> [energy]");

        var type = parts[1].ToLowerInvariant();
        var number = parts[2];
        var name = parts[3];

        if (!int.TryParse(parts[4], out var easyScore))
            throw new ParseException("easyScore должен быть целым числом");

        return type switch
        {
            "scooter" => new AddItemCommand(new Scooter(
                number, name, easyScore, ParseEnergy(parts, 5))),

            "bike" => new AddItemCommand(new Bike(
                number, name, easyScore)),

            "ebike" => new AddItemCommand(new ElectricBike(
                number, name, easyScore, ParseEnergy(parts, 5))),

            "helmet" => new AddItemCommand(new Helmet(
                number, name, easyScore)),

            "charger" => new AddItemCommand(new ChargeBattery(
                number, name, easyScore, ParseEnergy(parts, 5))),

            _ => throw new ParseException($"Неизвестный тип: {type}")
        };
    }

    private static double ParseEnergy(string[] parts, int index)
    {
        if (parts.Length <= index)
            throw new ParseException("Не указан расход энергии");

        if (!double.TryParse(parts[index], NumberStyles.Float,
                             CultureInfo.InvariantCulture, out var value))
            throw new ParseException("Расход энергии должен быть числом");

        return value;
    }
}