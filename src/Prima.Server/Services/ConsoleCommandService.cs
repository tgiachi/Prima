using System.Text;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;
using Spectre.Console;

namespace Prima.Server.Services;

/// <summary>
/// Provides command line interface functionality for the Prima server.
/// </summary>
public class ConsoleCommandService : IHostedService, IDisposable
{
    private readonly string _prompt;
    private readonly CancellationTokenSource _cts = new();
    private Task _inputTask;
    private readonly Action<string> _commandHandler;
    private bool _isDisposed;
    private readonly Action<ConsoleKeyInfo> _tabHandler;

    private readonly ICommandSystemService _commandSystemService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleCommandService"/> class.
    /// </summary>
    /// <param name="prompt">The prompt to display before each command input.</param>
    /// <param name="commandHandler">The function to handle command input.</param>
    public ConsoleCommandService(ICommandSystemService commandSystemService, string prompt = "prima> ")
    {
        _commandSystemService = commandSystemService;
        _prompt = prompt;
        _commandHandler = DefaultCommandHandler;
        _tabHandler = info =>
        {
            _commandSystemService.AutoComplete(info.KeyChar.ToString())
                .ToList()
                .ForEach(s => { AnsiConsole.MarkupLine($"[green]{s}[/]"); });
        };
    }

    /// <summary>
    /// Default command handler implementation.
    /// </summary>
    /// <param name="command">The command to process.</param>
    private async void DefaultCommandHandler(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return;

        var result = await _commandSystemService.ExecuteCommandAsync(
            command,
            CommandType.Console,
            CommandPermissionType.All
        );

        if (result.ResultType == CommandResultType.Success)
        {
            AnsiConsole.MarkupLine($"[green]Command executed successfully: {command}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error executing command: {command} - {result.Message}[/]");
        }
    }

    /// <summary>
    /// Continuously processes user input from the console.
    /// </summary>
    private async Task ProcessInputAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            // Write prompt
            Console.Write(_prompt);

            var input = ReadLineWithTabSupport(out bool wasCancelled);

            if (wasCancelled)
                break;

            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    // Process the command
                    _commandHandler(input);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red on yellow]Error processing command: {input}: {ex.Message}[/]");
                }
            }
        }
    }

    /// <summary>
    /// Reads a line from the console with support for special keys like TAB.
    /// </summary>
    /// <param name="wasCancelled">Indicates if the read operation was cancelled.</param>
    /// <returns>The input line.</returns>
    private string ReadLineWithTabSupport(out bool wasCancelled)
    {
        var inputBuffer = new StringBuilder();
        int cursorPosition = 0;
        wasCancelled = false;

        while (true)
        {
            if (_cts.Token.IsCancellationRequested)
            {
                wasCancelled = true;
                return string.Empty;
            }

            // Read key without displaying it
            ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    System.Console.WriteLine(); // Move to next line
                    return inputBuffer.ToString();

                case ConsoleKey.Tab:
                    // Handle tab completion
                    _tabHandler(keyInfo);
                    break;

                case ConsoleKey.Backspace:
                    if (cursorPosition > 0)
                    {
                        inputBuffer.Remove(cursorPosition - 1, 1);
                        cursorPosition--;

                        // Redraw input line
                        RedrawInputLine(inputBuffer.ToString(), cursorPosition);
                    }

                    break;

                case ConsoleKey.Delete:
                    if (cursorPosition < inputBuffer.Length)
                    {
                        inputBuffer.Remove(cursorPosition, 1);

                        // Redraw input line
                        RedrawInputLine(inputBuffer.ToString(), cursorPosition);
                    }

                    break;

                case ConsoleKey.LeftArrow:
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                        System.Console.SetCursorPosition(_prompt.Length + cursorPosition, System.Console.CursorTop);
                    }

                    break;

                case ConsoleKey.RightArrow:
                    if (cursorPosition < inputBuffer.Length)
                    {
                        cursorPosition++;
                        System.Console.SetCursorPosition(_prompt.Length + cursorPosition, System.Console.CursorTop);
                    }

                    break;

                case ConsoleKey.Home:
                    cursorPosition = 0;
                    System.Console.SetCursorPosition(_prompt.Length + cursorPosition, System.Console.CursorTop);
                    break;

                case ConsoleKey.End:
                    cursorPosition = inputBuffer.Length;
                    System.Console.SetCursorPosition(_prompt.Length + cursorPosition, System.Console.CursorTop);
                    break;

                default:
                    // Handle normal key input
                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        if (cursorPosition == inputBuffer.Length)
                        {
                            inputBuffer.Append(keyInfo.KeyChar);
                            System.Console.Write(keyInfo.KeyChar);
                        }
                        else
                        {
                            inputBuffer.Insert(cursorPosition, keyInfo.KeyChar);
                            RedrawInputLine(inputBuffer.ToString(), cursorPosition + 1);
                        }

                        cursorPosition++;
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Redraws the input line with the current buffer and cursor position.
    /// </summary>
    /// <param name="input">The current input text.</param>
    /// <param name="cursorPosition">The desired cursor position.</param>
    private void RedrawInputLine(string input, int cursorPosition)
    {
        // Save cursor top position
        int currentTop = System.Console.CursorTop;

        // Clear the current line
        System.Console.SetCursorPosition(0, currentTop);
        System.Console.Write(new string(' ', _prompt.Length + Math.Max(input.Length + 1, 1)));

        // Redraw prompt and input
        System.Console.SetCursorPosition(0, currentTop);
        System.Console.Write(_prompt);
        System.Console.Write(input);

        // Restore cursor position
        System.Console.SetCursorPosition(_prompt.Length + cursorPosition, currentTop);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ConsoleCommandService"/>.
    /// </summary>
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _cts.Cancel();
            try
            {
                _inputTask.Wait(1000); // Give some time for clean termination
            }
            catch (AggregateException)
            {
                // Task was cancelled, which is expected
            }

            _cts.Dispose();
            _isDisposed = true;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Start input processing task
        _inputTask = Task.Run(ProcessInputAsync, _cts.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}
