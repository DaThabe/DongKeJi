namespace DongKeJi.Common.Model;

/// <summary>
/// 真假结果
/// </summary>
public class Result
{
    /// <summary>
    /// 异常状态
    /// </summary>
    private readonly Exception? _innerException;
    
    /// <summary>
    /// 是否成功
    /// </summary>
    private readonly bool _innerIsSuccess;

    /// <summary>
    /// 是否有值
    /// </summary>
    public bool IsSuccess => _innerIsSuccess;



    /// <summary>
    /// 成功状态结果
    /// </summary>
    private Result(bool success) => _innerIsSuccess = success;
    
    /// <summary>
    /// 异常结果
    /// </summary>
    /// <param name="exception"></param>
    public Result(Exception exception) => _innerException = exception;


    /// <summary>
    /// 成功状态转换
    /// </summary>
    /// <param name="success"></param>
    public static implicit operator Result(bool success)
    {
        return success ? Success : Fail;
    }

    /// <summary>
    /// 异常转换
    /// </summary>
    /// <param name="exception"></param>
    public static implicit operator Result(Exception exception)
    {
        return new Result(exception);
    }


    /// <summary>
    /// 成功结果
    /// </summary>
    public static Result Success { get; } = new(true);

    /// <summary>
    /// 成功结果
    /// </summary>
    public static Result Fail { get; } = new(false);


    public bool Match(Func<Exception, bool> exception)
    {
        if (_innerException != null)
        {
            return exception(_innerException);
        }

        return true;
    }

}

/// <summary>
/// 值结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    /// <summary>
    /// 值状态
    /// </summary>
    private readonly T? _innerValue;

    /// <summary>
    /// 异常状态
    /// </summary>
    private readonly Exception? _innerException;

    /// <summary>
    /// 是否有值
    /// </summary>
    public bool HasValue => _innerValue != null;

    /// <summary>
    ///值
    /// </summary>
    public T? Value => _innerValue;

    /// <summary>
    /// 异常
    /// </summary>
    public Exception? Exception => _innerException;


    /// <summary>
    /// 值结果
    /// </summary>
    /// <param name="value"></param>
    private Result(T value) => _innerValue = value;

    /// <summary>
    /// 异常结果
    /// </summary>
    /// <param name="exception"></param>
    private Result(Exception exception) => _innerException = exception;

    
    /// <summary>
    /// 值转换
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    /// <summary>
    /// 异常转换
    /// </summary>
    /// <param name="exception"></param>
    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception);
    }



    public T Match(T @default)
    {
        if (_innerValue == null)
        {
            return @default;
        }

        return _innerValue;
    }

    public TResult Match<TResult>(Func<T, TResult> select, TResult @default)
    {
        if (_innerValue == null)
        {
            return @default;
        }

        return select(_innerValue);
    }


    public T Match(Func<Exception, T> exception)
    {
        if (_innerException != null)
        {
            return exception(_innerException);
        }

        return _innerValue!;
    }

    public TResult Match<TResult>(Func<T, TResult> select, Func<Exception, TResult> exception)
    {
        if (_innerException != null)
        {
            return exception(_innerException);
        }

        return select(_innerValue!);
    }
}


/// <summary>
/// 扩展方法
/// </summary>
public static class ResultExtensions
{
    public static Result<T> Create<T>(Exception exception)
    {
        return exception;
    }

    public static Result<T> Create<T>(T value)
    {
        return value;
    }



    public static Result<T> Create<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<T> Create<T>(Func<Result<T>> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }



    public static async ValueTask<Result<T>> Create<T>(Func<ValueTask<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static async ValueTask<Result<T>> Create<T>(Func<ValueTask<Result<T>>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}