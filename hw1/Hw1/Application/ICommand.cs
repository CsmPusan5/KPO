using Hw1.Domain;

namespace Hw1.Application;

public interface ICommand
{
    bool Execute(Park park, IReport report);
}
