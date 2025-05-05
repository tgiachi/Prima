namespace Prima.Core.Server.Types;

[Flags]
public enum CommandPermissionType
{
    None = 0,
    Player = 1,
    GM = 2,
    Admin = 4,
    Developer = 8,
    Console = 16,
    All = Player | GM | Admin | Developer | Console,
    AllInGame = Player | GM | Admin | Developer
}
