namespace SampleApp.BackEnd.Domain;
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IEnumerable<Role> Roles { get; set; }
}


public class NullUser : User
{
    public NullUser()
    {
        Id = -1;
        Name = "Null";
        Email = "Null";
        Roles = new List<Role>();
    }
}