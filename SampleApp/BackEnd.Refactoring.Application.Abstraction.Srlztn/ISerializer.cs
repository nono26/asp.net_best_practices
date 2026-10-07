namespace BackEnd.Refactoring.Application.Abstraction.Srlztn;

public interface ISerializer
{
    string Serialize<T> (T value);
    T Deserialize<T> (string json);
}
