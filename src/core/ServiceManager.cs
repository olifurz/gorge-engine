using Gorge.Services;

namespace Gorge.Core;

public static class ServiceManager
{
    private static readonly Dictionary<Type, BaseService> _services = new();

    public static void Initialise()
    {
        Log.LogInfo("Initialising services");
        // Order matters here !!
        RegisterService(new ResourceService());
        RegisterService(new WorkspaceService());
        RegisterService(new PhysicsService());
        RegisterService(new RenderService());

        foreach (var service in _services.Values)
        {
            service.Start();
        }
        Log.LogInfo("Services initialised");
    }

    public static void Update()
    {
        foreach (var service in _services.Values)
        {
            service.Update();
        }
    }

    public static void Deinitialise()
    {
        foreach (var service in _services.Values)
        {
            service.Stop();
            Log.LogInfo($"Stopped {service}");
        }
    }

    public static void RegisterService<T>(T service) where T : BaseService
    {
        _services[typeof(T)] = service;
    }

    public static T GetService<T>() where T : BaseService
    {
        return (T)_services[typeof(T)];
    }
}