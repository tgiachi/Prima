using System.Reflection;
using System.Text.RegularExpressions;

namespace Prima.Core.Server.Utils;

/// <summary>
/// Utility class for managing embedded resources in the assembly
/// </summary>
public static partial class EmbeddedResourcesHelper
{
    /// <summary>
    /// Gets a list of all embedded resources that match a given pattern
    /// </summary>
    /// <param name="assembly">The assembly to search in (if null, uses current assembly)</param>
    /// <param name="directoryPath">Directory path to search (e.g. "Assets.Templates")</param>
    /// <returns>A list of resource names found</returns>
    public static IEnumerable<string> GetEmbeddedResourceNames(Assembly assembly = null, string directoryPath = null)
    {
        // If no assembly is specified, use the current one
        assembly ??= Assembly.GetExecutingAssembly();

        // Get all resources in the assembly
        var resourceNames = assembly.GetManifestResourceNames();

        // If no directory path is specified, return all resources
        if (string.IsNullOrEmpty(directoryPath))
        {
            return resourceNames;
        }

        // Replace any path separators with dots, as required by the embedded resource format
        string normalizedPath = directoryPath.Replace('/', '.').Replace('\\', '.');

        // If it doesn't end with a dot, add one to ensure we're looking for that specific path
        if (!normalizedPath.EndsWith("."))
        {
            normalizedPath += ".";
        }

        // Filter resources that contain the specified path
        return resourceNames.Where(name => name.Contains(normalizedPath));
    }

    /// <summary>
    /// Gets a list of all files in a specific embedded directory
    /// </summary>
    /// <param name="assembly">The assembly to search in (if null, uses current assembly)</param>
    /// <param name="directoryPath">Directory path to search (e.g. "Assets/Templates")</param>
    /// <returns>A list of file names (without the full path)</returns>
    public static IEnumerable<string> GetEmbeddedResourceFileNames(
        Assembly assembly = null, string directoryPath = "Assets/Templates"
    )
    {
        // Normalize the path for embedded resource format
        string normalizedPath = directoryPath.Replace('/', '.').Replace('\\', '.');

        // Get all resources in the specified path
        var resources = GetEmbeddedResourceNames(assembly, normalizedPath);

        // Extract file names from the full paths
        var fileNames = new List<string>();

        foreach (var resource in resources)
        {
            // Extract the final part of the resource name (file name with extension)
            string fileName = resource.Substring(resource.LastIndexOf('.') + 1);

            // If not empty, add it to the list
            if (!string.IsNullOrEmpty(fileName))
            {
                fileNames.Add(fileName);
            }
        }

        return fileNames;
    }

    /// <summary>
    /// Reads the content of an embedded resource as a string
    /// </summary>
    /// <param name="resourcePath">Resource path (e.g. "Assets/Templates/welcome.scriban")</param>
    /// <param name="assembly">The assembly to search in (if null, uses current assembly)</param>
    /// <returns>The content of the resource as a string</returns>
    public static string GetEmbeddedResourceContent(string resourcePath, Assembly assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();

        // Normalize the path for embedded resource format
        string normalizedPath = resourcePath.Replace('/', '.').Replace('\\', '.');

        // Get the full resource name
        string assemblyName = assembly.GetName().Name;
        string fullResourceName = $"{assemblyName}.{normalizedPath}";

        // Check if the resource exists
        if (!assembly.GetManifestResourceNames().Contains(fullResourceName))
        {
            // Try to find a partial match
            var resourceNames = assembly.GetManifestResourceNames();
            var matchingResource = resourceNames.FirstOrDefault(n => n.EndsWith(normalizedPath));

            if (matchingResource != null)
            {
                fullResourceName = matchingResource;
            }
            else
            {
                throw new FileNotFoundException($"Embedded resource not found: {resourcePath}");
            }
        }

        // Read the resource content
        using (var stream = assembly.GetManifestResourceStream(fullResourceName))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Unable to open resource: {fullResourceName}");
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Extracts the file name from an embedded resource path
    /// </summary>
    /// <param name="resourceName">Full resource name</param>
    /// <returns>File name without path</returns>
    public static string GetFileNameFromResourcePath(string resourceName)
    {
        // Use a regex to extract the file name
        Match match = FileNameRegex().Match(resourceName);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return resourceName; // If it fails to find a pattern, return the original name
    }

    [GeneratedRegex(@"\.([^\.]+)$")]
    private static partial Regex FileNameRegex();
}
