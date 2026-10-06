namespace BackEnd.Refactoring.Application.Abstration.Srlztn;

public interface ISerializer
{
    string Serialize<T> (T value);
    T Deserialize<T> (string json);
}
