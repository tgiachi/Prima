namespace Prima.JavaScript.Engine.Data.Configs;

public class ScriptEngineConfig
{
    public List<string> InitScriptsFileNames { get; set; } = new() { "bootstrap.js", "index.js" };

    public ScriptNameConversion NamingConvention { get; set; } = ScriptNameConversion.PascalCase;
}

public enum ScriptNameConversion
{
    CamelCase,
    PascalCase,
    SnakeCase,
}
