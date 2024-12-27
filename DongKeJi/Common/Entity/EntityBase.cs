namespace DongKeJi.Common.Entity;

public class EntityBase : IIdentifiable
{
    /// <summary>
    ///     Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}