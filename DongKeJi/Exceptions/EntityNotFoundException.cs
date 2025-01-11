namespace DongKeJi.Exceptions;

/// <summary>
/// 实体不存在
/// </summary>
public class EntityNotFoundException(string message) : Exception(message);