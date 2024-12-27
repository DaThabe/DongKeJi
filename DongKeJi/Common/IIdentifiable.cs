namespace DongKeJi.Common;

/// <summary>
///     表示具有Id属性
/// </summary>
public interface IIdentifiable
{
    /// <summary>
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


public static class IdentifiableExtensions
{
    /// <summary>
    /// 如果 <see cref="IIdentifiable"/> == (<see cref="Nullable"/> or <see cref="Identifiable.Empty"/>) or (<see cref="IIdentifiable.Id"/>==<see cref="Guid.Empty"/>)
    /// </summary>
    /// <param name="identifiable"></param>
    /// <returns></returns>
    public static bool IsEmpty(this IIdentifiable? identifiable)
    {
        if (identifiable == null)
        {
            return true;
        }

        if (identifiable == Identifiable.Empty)
        {
            return true;
        }

        if (identifiable.Id == Guid.Empty)
        {
            return true;
        }

        return false;
    }
}