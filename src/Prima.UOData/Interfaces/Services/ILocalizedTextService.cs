using Orion.Core.Server.Interfaces.Services.Base;

namespace Prima.UOData.Interfaces.Services;

public interface ILocalizedTextService : IOrionService, IOrionStartService
{

    /// <summary>
    ///     Get the localized text for the specified ID.
    /// </summary>
    /// <param name="id">The ID of the localized text.</param>
    /// <returns>The localized text.</returns>
    string GetLocalizedText(int id);

    /// <summary>
    ///     Get the localized text for the specified ID and format it with the specified arguments.
    /// </summary>
    /// <param name="id">The ID of the localized text.</param>
    /// <param name="args">The arguments to format the localized text with.</param>
    /// <returns>The formatted localized text.</returns>
    string GetLocalizedText(int id, params object[] args);
}
