using System.Reflection;
using Jint;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Extensions;
using Orion.JavaScript.Engine.Data.Internal;
using Prima.Core.Server.Attributes.Scripts;
using Prima.Core.Server.Interfaces.Services;
using Prima.JavaScript.Engine.Data.Configs;
using Prima.JavaScript.Engine.Utils.Scripts;

namespace Prima.JavaScript.Engine.Services;

public class ScriptEngineService : IScriptEngineService, IEventBusListener<ServerReadyEvent>
{
    private readonly ILogger _logger;

    private readonly List<string> _initScripts;

    private readonly DirectoriesConfig _directoriesConfig;
    private readonly Jint.Engine _jsEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ScriptModuleData> _scriptModules;

    private readonly Dictionary<string, Action<object[]>> _callbacks = new();
    private readonly Dictionary<string, object> _constants = new();

    private readonly ScriptEngineConfig _scriptEngineConfig;

    private readonly IVersionService _versionService;

    private readonly AppNameData _appNameData;

    private readonly IEventBusService _eventBusService;

    private Func<string, string> _nameResolver;

    public ScriptEngineService(
        ILogger<ScriptEngineService> logger, DirectoriesConfig directoriesConfig, IServiceProvider serviceProvider,
        List<ScriptModuleData> scriptModules, IEventBusService eventBusService, ScriptEngineConfig scriptEngineConfig,
        IVersionService versionService, AppNameData appNameData
    )
    {
        _logger = logger;
        _directoriesConfig = directoriesConfig;
        _serviceProvider = serviceProvider;
        _scriptModules = scriptModules;

        _eventBusService = eventBusService;
        _scriptEngineConfig = scriptEngineConfig;
        _versionService = versionService;
        _appNameData = appNameData;

        _initScripts = _scriptEngineConfig.InitScriptsFileNames;


        CreateNameResolver();

        var typeResolver = TypeResolver.Default;

        typeResolver.MemberNameCreator = MemberNameCreator;
        _jsEngine = new Jint.Engine(options =>
            {
                options.EnableModules(directoriesConfig["Scripts"]);
                options.AllowClr(GetType().Assembly);
                options.SetTypeResolver(typeResolver);
            }
        );

        _eventBusService.Subscribe(this);
    }

    private void CreateNameResolver()
    {
        _nameResolver = name => name.ToSnakeCase();

        _nameResolver = _scriptEngineConfig.NamingConvention switch
        {
            ScriptNameConversion.CamelCase  => name => name.ToCamelCase(),
            ScriptNameConversion.PascalCase => name => name.ToPascalCase(),
            ScriptNameConversion.SnakeCase  => name => name.ToSnakeCase(),
            _                               => _nameResolver
        };
    }

    private IEnumerable<string> MemberNameCreator(MemberInfo memberInfo)
    {
        var memberType = _nameResolver(memberInfo.Name);
        _logger.LogTrace("[JS] Creating member name  {MemberInfo}", memberType);
        yield return memberType;
    }


    private void ExecuteBootstrap()
    {
        foreach (var file in _initScripts.Select(s => Path.Combine(_directoriesConfig["Scripts"], s)))
        {
            if (File.Exists(file))
            {
                var fileName = Path.GetFileName(file);
                _logger.LogInformation("Executing {FileName}  script", fileName);
                ExecuteScriptFile(file);
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var module in _scriptModules)
        {
            var scriptModuleAttribute = module.ModuleType.GetCustomAttribute<ScriptModuleAttribute>();
            var instance = _serviceProvider.GetService(module.ModuleType);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Unable to create instance of script module {module.ModuleType.Name}"
                );
            }

            _logger.LogDebug("Registering script module {Name}", scriptModuleAttribute.Name);

            _jsEngine.SetValue(scriptModuleAttribute.Name, instance);
        }

        AddConstant("appName", _appNameData.AppName);
        AddConstant("version", _versionService.GetVersionInfo().Version);

        _logger.LogDebug("Generating scripts documentation in scripts directory named 'index.d.ts'");
        var documentation = TypeScriptDocumentationGenerator.GenerateDocumentation(
            _appNameData.AppName,
            _versionService.GetVersionInfo().Version,
            _scriptModules,
            _constants,
            _nameResolver
        );

        File.WriteAllText(Path.Combine(_directoriesConfig["Scripts"], "index.d.ts"), documentation);


        ExecuteBootstrap();


        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void AddInitScript(string script)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        _initScripts.Add(script);
    }

    public void ExecuteScript(string script)
    {
        try
        {
            _jsEngine.Execute(script);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error executing script");
        }
    }

    public void ExecuteScriptFile(string scriptFile)
    {
        var content = File.ReadAllText(scriptFile);

        ExecuteScript(content);
    }

    public void AddCallback(string name, Action<object[]> callback)
    {
        _callbacks[name] = callback;
    }

    public void AddConstant(string name, object value)
    {
        _constants[name.ToSnakeCaseUpper()] = value;
        _jsEngine.SetValue(name.ToSnakeCaseUpper(), value);
    }

    public void ExecuteCallback(string name, params object[] args)
    {
        if (_callbacks.TryGetValue(name, out var callback))
        {
            _logger.LogDebug("Executing callback {Name}", name);
            callback(args);
        }
        else
        {
            _logger.LogWarning("Callback {Name} not found", name);
        }
    }


    public async Task HandleAsync(ServerReadyEvent @event, CancellationToken cancellationToken = default)
    {
        if (_callbacks.TryGetValue("onStarted", out var callback))
        {
            _logger.LogInformation("Executing onStarted");
            callback(null);
        }
        else
        {
            _logger.LogInformation("function {OnStartFunction} not found", _nameResolver("onStarted"));
        }
    }
}
