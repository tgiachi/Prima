namespace Prima.Core.Server.Types;

[Flags]
public enum CommandType
{
    Console,
    InGame,
    All = Console | InGame
}
