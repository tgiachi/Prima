using Orion.Core.Server.Attributes.Scripts;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Interfaces.Services.System;

namespace Prima.Core.Server.Modules.Scripts;

[ScriptModule("files")]
public class FileScriptModule
{
    private readonly DirectoriesConfig _directoriesConfig;

    private readonly IScriptEngineService _scriptEngineService;

    public FileScriptModule(DirectoriesConfig directoriesConfig, IScriptEngineService scriptEngineService)
    {
        _directoriesConfig = directoriesConfig;
        _scriptEngineService = scriptEngineService;
    }


    [ScriptFunction("Include a script file")]
    public void IncludeScript(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty");
        }

        var filePath = Path.Combine(_directoriesConfig.Root, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Script file '{fileName}' not found in the scripts directory.");
        }

        _scriptEngineService.ExecuteScriptFile(filePath);

    }
}
