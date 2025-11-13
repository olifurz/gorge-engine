using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Gorge.Core;

namespace Gorge.Services;

public class ResourceService : BaseService
{
    Assembly assembly = Assembly.GetExecutingAssembly();
    private string? assemblyName;
    public override void Start()
    {
        base.Start();
        assemblyName = assembly.GetName().Name?.Replace("-", "_"); ;

        Log.LogInfo($"Assembly v{assembly.GetName().Version}");
    }

    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Fetches an EmbeddedResource
    /// </summary>
    /// <param name="resourcePath">Resource path ** NOT including the namespace</param>
    /// <returns>Byte array of resource</returns>
    public byte[] GetResource(string resourcePath)
    {
        using var rs = assembly.GetManifestResourceStream(assemblyName + "." + resourcePath);
        if (rs == null)
        {
            Log.LogError("Resource not found " + assemblyName + "." + resourcePath);
            throw new FileNotFoundException(resourcePath);
        }
        using (var ms = new MemoryStream())
        {
            rs.CopyTo(ms);
            return ms.ToArray();
        }
    }


    public override void Stop()
    {
        base.Stop();
    }
}