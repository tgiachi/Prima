using Prima.Core.Server.Types;

namespace Prima.Core.Server.Data.Commands;

public class CommandResult
{
    public CommandResultType ResultType { get; set; }

    public Exception? Exception { get; set; }
    public string Message { get; set; }

    public static CommandResult Success(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Success,
            Message = message
        };
    }

    public static CommandResult Error(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Error,
            Message = message
        };
    }

    public static CommandResult Warning(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Warning,
            Message = message
        };
    }

    public static CommandResult Info(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Info,
            Message = message
        };
    }

    public static CommandResult Debug(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Debug,
            Message = message
        };
    }

    public static CommandResult EmitException(Exception ex)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.Exception,
            Exception = ex,
            Message = ex.Message
        };
    }

    public static CommandResult NotFound(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.NotFound,
            Message = message
        };
    }



    public static CommandResult PermissionDenied(string message)
    {
        return new CommandResult
        {
            ResultType = CommandResultType.PermissionDenied,
            Message = message
        };
    }

}
