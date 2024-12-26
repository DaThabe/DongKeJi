namespace DongKeJi.Common.ViewModel;

/// <summary>
/// 实体 VM
/// </summary>
public class IdentifiableViewModel : ViewModelBase, IIdentifiable
{
    /// <summary>
    /// id
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    public override string ToString()
    {
        return Id.ToString("N");
    }
}