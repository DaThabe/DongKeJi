namespace DongKeJi.Common.Exceptions;

/// <summary>
/// 实体已存在
/// </summary>
public class EntityAlreadyExistsException(string message) : Exception(message);