namespace Hw1.Application;

public interface IParser
{
    ICommand Parse(string input);
}