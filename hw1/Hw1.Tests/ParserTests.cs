using Xunit;
using Hw1.Application;

namespace Hw1.Tests;

public class ParserTests
{
    private readonly Parser _parser = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Parse_EmptyInput_ShouldThrowParseException(string? input)
    {
        // Act & Assert
        Assert.Throws<ParseException>(() => _parser.Parse(input!));
    }

    [Theory]
    [InlineData("foobar")]
    [InlineData("add")]
    [InlineData("add scooter")]
    [InlineData("add scooter 001")]
    [InlineData("add unknown 001 Xiaomi 8")]
    public void Parse_InvalidInput_ShouldThrowParseException(string input)
    {
        // Act & Assert
        Assert.Throws<ParseException>(() => _parser.Parse(input));
    }

    [Fact]
    public void Parse_AddScooter_ShouldReturnAddItemCommand()
    {
        // Act
        var command = _parser.Parse("add scooter 001 Xiaomi 8 1.2");

        // Assert
        Assert.IsType<AddItemCommand>(command);
    }

    [Fact]
    public void Parse_AddScooterWithoutEnergy_ShouldThrowParseException()
    {
        // Act & Assert
        Assert.Throws<ParseException>(
            () => _parser.Parse("add scooter 001 Xiaomi 8"));
    }

    [Fact]
    public void Parse_AddBike_ShouldReturnAddItemCommand()
    {
        // Act
        var command = _parser.Parse("add bike 001 Bike 3");

        // Assert
        Assert.IsType<AddItemCommand>(command);
    }

    [Theory]
    [InlineData("report", typeof(ShowReportCommand))]
    [InlineData("easy",   typeof(ShowEasyCommand))]
    [InlineData("help",   typeof(HelpCommand))]
    [InlineData("exit",   typeof(ExitCommand))]
    public void Parse_KnownCommands_ShouldReturnExpectedCommandType(
        string input, Type expectedType)
    {
        // Act
        var command = _parser.Parse(input);

        // Assert
        Assert.IsType(expectedType, command);
    }
}