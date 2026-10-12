public interface IEasyForBeginners
{
    int GetEasyScore();
    bool IsEasyForBeginners() => GetEasyScore() >= 6;
}