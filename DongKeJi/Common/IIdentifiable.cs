namespace DongKeJi.Common;


/// <summary>
/// 表示具有Id属性
/// </summary>
public interface IIdentifiable
{
    /// <summary>
    /// 
    /// </summary>
    Guid Id { get; }
}


public class Identifiable(Guid guid) : IIdentifiable
{
    public static Identifiable Empty { get; } = new(Guid.Empty);

    public Guid Id { get; } = guid;



    public static Identifiable Create(Guid guid)
    {
        return new Identifiable(guid);
    }

    public static Identifiable Create()
    {
        return new Identifiable(Guid.NewGuid());
    }
}
