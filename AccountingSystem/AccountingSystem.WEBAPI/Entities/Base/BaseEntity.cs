namespace AccountingSystem.WEBAPI.Entities.Base;

public abstract class BaseEntity
{
    public BaseEntity()
    {
        Id = new int();
    }
    public int Id { get; set; }
}
