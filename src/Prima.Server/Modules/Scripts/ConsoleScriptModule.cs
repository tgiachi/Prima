using Prima.Core.Server.Attributes.Scripts;
using Spectre.Console;

namespace Prima.Server.Modules.Scripts;

[ScriptModule("console")]
public class ConsoleScriptModule
{
    [ScriptFunction("Log a message to the console")]
    public void Log(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null or empty");
        }

        Console.WriteLine(message);
    }

    [ScriptFunction("Log an error message to the console")]
    public void LogError(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null or empty");
        }
        AnsiConsole.Markup($"[red]{message}[/]");
    }

    [ScriptFunction("Ask for user input")]
    public Task Prompt(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message), "Message cannot be null or empty");
        }

        return AnsiConsole.PromptAsync(
            new TextPrompt<string>(message)
                .PromptStyle("green")
                .ShowDefaultValue()
                .DefaultValue("default")
                .AllowEmpty()
        );
    }
}
