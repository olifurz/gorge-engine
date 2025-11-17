using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Gorge.Core;
using Gorge.Services;

namespace Gorge.Game;

/// <summary>
/// Root game object
/// </summary>
public class GameObject
{
    public int Id;
    public string Name = "Unnamed Object";
    public GameObject? Parent { get; internal set; }
    public List<GameObject> Children { get; internal set; } = [];

    /// <summary>
    /// Get the parent GameObject
    /// </summary>
    public GameObject? GetParent()
    {
        return Parent;
    }

    /// <summary>
    /// Get the ID of the parent GameObject
    /// </summary>
    public int? GetParentId()
    {
        return Parent?.Id;
    }


    /// <summary>
    /// Set the parent GameObject
    /// </summary>
    /// <param name="obj">GameObject to set</param>
    public void SetParent(GameObject obj)
    {
        Parent = obj;
        obj.AddChild(this);
    }

    /// <summary>
    /// Set the parent GameObject via ID
    /// </summary>
    /// <param name="id">ID to search for</param>
    public void SetParent(int id)
    {
        var obj = ServiceManager.GetService<WorkspaceService>().GetObject(id);
        if (obj != null)
        {
            Parent = obj;
            obj.AddChild(this);
        }
        else throw Log.Error($"{Name} tried to parent to invalid id ({id})");
    }

    /// <summary>
    /// Get array of children of this GameObject
    /// </summary>
    /// <param name="recursive">Whether to recursively traverse through children</param>
    /// <returns>Array of children</returns>
    public GameObject[] GetChildren(bool recursive = false)
    {
        List<GameObject> result = [];

        result.AddRange(Children);

        if (recursive)
        {
            collectChildren(this, result);
        }
        return result.ToArray();
    }

    /// <summary>
    /// Helper to recursively collect all children for GetChildren()
    /// </summary>
    private static void collectChildren(GameObject gameObject, List<GameObject> allChildren)
    {
        foreach (var child in gameObject.Children)
        {
            allChildren.Add(child);

            collectChildren(child, allChildren);
        }
    }

    public void AddChild(GameObject child)
    {
        Children.Add(child);
    }
}