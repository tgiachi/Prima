using Orion.Core.Server.Interfaces.Services.Base;

namespace Prima.Core.Server.Interfaces.Services;

public interface IScriptEngineService : IOrionStartService
{
    void AddInitScript(string script);
    void ExecuteScript(string script);
    void ExecuteScriptFile(string scriptFile);
    void AddCallback(string name, Action<object[]> callback);
    void AddConstant(string name, object value);
    void ExecuteCallback(string name, params object[] args);
}
