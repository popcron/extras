using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using UnityEngine;
using SystemAssembly = System.Reflection.Assembly;

[Serializable]
public class Assembly
{
    private static Dictionary<string, Assembly> fullNameToAssembly = null;
    private static Dictionary<SystemAssembly, Assembly> systemAssemblyToAssembly = null;
    private static ReadOnlyCollection<Assembly> all = null;

    /// <summary>
    /// List of all types from all assemblies.
    /// </summary>
    public static ReadOnlyCollection<Assembly> All
    {
        get
        {
            if (all is null)
            {
                FindAll();
            }

            return all;
        }
    }

    [SerializeField]
    private string fullName;

    private SystemAssembly assembly;
    private readonly string location;

    private SystemAssembly SystemAssembly
    {
        get
        {
            if (assembly is null)
            {
                assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.FullName == fullName);
            }

            return assembly;
        }
    }

    public string Location => location;

    protected Assembly(SystemAssembly assembly)
    {
        this.assembly = assembly;
        fullName = assembly.FullName;
        location = assembly.Location;
    }

    public override int GetHashCode() => SystemAssembly.GetHashCode();

    public override bool Equals(object other)
    {
        if (other is null)
        {
            return false;
        }

        return Equals(other as Assembly);
    }

    public bool Equals(Assembly other)
    {
        if (other is null)
        {
            return false;
        }

        return SystemAssembly.Equals(other.SystemAssembly);
    }

    private static void FindAll()
    {
        fullNameToAssembly = new Dictionary<string, Assembly>();
        systemAssemblyToAssembly = new Dictionary<SystemAssembly, Assembly>();

        List<Assembly> all = new List<Assembly>();
        SystemAssembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (SystemAssembly assembly in assemblies)
        {
            Assembly newType = new Assembly(assembly);
            all.Add(newType);
            fullNameToAssembly[assembly.FullName] = newType;
            systemAssemblyToAssembly[assembly] = newType;
        }

        Assembly.all = all.AsReadOnly();
    }

    /// <summary>
    /// Returns a type using this full name.
    /// </summary>
    public static Assembly GetAssembly(string fullName)
    {
        if (fullNameToAssembly is null)
        {
            FindAll();
        }

        if (fullNameToAssembly.TryGetValue(fullName, out Assembly assembly))
        {
            return assembly;
        }
        else
        {
            return null;
        }
    }

    public static implicit operator SystemAssembly(Assembly assembly) => assembly.SystemAssembly;
    public static implicit operator Assembly(SystemAssembly assembly)
    {
        if (systemAssemblyToAssembly is null)
        {
            FindAll();
        }

        return systemAssemblyToAssembly[assembly];
    }

    [SecuritySafeCritical]
    public static bool operator ==(Assembly left, Assembly right)
    {
        return left.SystemAssembly == right.SystemAssembly;
    }

    [SecuritySafeCritical]
    public static bool operator !=(Assembly left, Assembly right)
    {
        return left.SystemAssembly != right.SystemAssembly;
    }
}
