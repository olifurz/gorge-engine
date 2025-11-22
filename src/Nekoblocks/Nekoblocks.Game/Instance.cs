using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Nekoblocks.Core;
using Nekoblocks.Services;

namespace Nekoblocks.Game;

/// <summary>
/// Root instance
/// </summary>
public class Instance
{
    public int Id { get; internal set; }
    public string Name { get; set; } = "Instance";
    public Instance? Parent { get; internal set; }
    public List<Instance> Children { get; internal set; } = [];

    public Instance()
    {
        Id = GameService.GetUniqueId();
    }

    /// <summary>
    /// Get the ID of the parent Instance
    /// </summary>
    public int? GetParentId()
    {
        return Parent?.Id;
    }


    /// <summary>
    /// Set the parent Instance
    /// </summary>
    /// <param name="obj">Instance to set</param>
    public void SetParent(Instance obj)
    {
        if (Parent != null)
            if (Parent.Children.Contains(this))
                Parent.Children.Remove(this); // Remove from parent's children list
        Parent = obj;
        obj.AddChild(this);
    }

    /// <summary>
    /// Set the parent Instance via ID
    /// </summary>
    /// <param name="id">ID to search for</param>
    public void SetParent(int id)
    {
        var obj = ServiceManager.GetService<GameService>().Root.GetChild(id);
        if (obj != null)
            SetParent(obj);
        else throw Log.Error($"{Name} tried to parent to invalid id ({id})");
    }

    /// <summary>
    /// Get a child of the Instance via ID
    /// </summary>
    /// <param name="id">ID to search for</param>
    /// <returns></returns>
    public Instance? GetChild(int id)
    {
        return Children?.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Get array of children of this Instance
    /// </summary>
    /// <param name="recursive">Whether to recursively traverse through children</param>
    /// <returns>Array of children</returns>
    public Instance[] GetChildren(bool recursive = false)
    {
        List<Instance> result = [];

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
    private static void collectChildren(Instance Instance, List<Instance> allChildren)
    {
        foreach (var child in Instance.Children)
        {
            allChildren.Add(child);
            collectChildren(child, allChildren);
        }
    }

    public void AddChild(Instance child)
    {
        Children.Add(child);
    }
}