﻿namespace DongKeJi.Extensions;


public static class ExceptionExtensions
{
    /// <summary>
    /// 格式化异常信息
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string FormatAllMessage(this Exception? exception)
    {
        if (exception is not null)
        {
            var childMessage = exception.InnerException.FormatAllMessage();

            if (string.IsNullOrWhiteSpace(childMessage))
            {
                return $"{exception.Message}";
            }

            return $"{exception.Message}\n{childMessage}";
        }

        return string.Empty;
    }
}