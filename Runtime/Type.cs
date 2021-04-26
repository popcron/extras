using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security;
using UnityEngine;
using SystemAssembly = System.Reflection.Assembly;
using SystemType = System.Type;

[Serializable]
public class Type
{
    private static Dictionary<string, Type> fullNameToType = null;
    private static Dictionary<string, SystemType> fullNameToSystemType = null;
    private static Dictionary<SystemType, Type> systemTypeToType = null;
    private static ReadOnlyCollection<Type> all = null;

    /// <summary>
    /// List of all types from all assemblies.
    /// </summary>
    public static ReadOnlyCollection<Type> All
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

    private SystemType type;
    private readonly string name;
    private readonly string nameSpace;
    private readonly bool isVisible;
    private readonly bool isNotPublic;
    private readonly bool isNested;
    private readonly string baseTypeFullName;
    private readonly string assemblyQualifiedName;
    private readonly string assemblyFullName;
    private readonly Guid guid;
    private readonly bool isArray;
    private readonly bool isSerialzable;
    private readonly bool isEnum;
    private readonly bool isSealed;
    private readonly bool isAbstract;
    private readonly TypeAttributes attributes;
    private readonly bool isClass;
    private readonly bool isInterface;
    private readonly bool isValueType;
    private readonly string underlyingSystemTypeFullName;

    private SystemType SystemType
    {
        get
        {
            if (type is null)
            {
                type = GetSystemType(fullName);
            }

            return type;
        }
    }

    public string FullName => fullName;
    public string Name => name;
    public string Namespace => nameSpace;
    public bool IsVisible => isVisible;
    public bool IsNotPublic => isNotPublic;
    public bool IsNested => isNested;
    public Type BaseType => GetType(baseTypeFullName);
    public Assembly Assembly => Assembly.GetAssembly(assemblyFullName);
    public string AssemblyQualifiedName => assemblyQualifiedName;
    public Guid GUID => guid;
    public bool IsArray => isArray;
    public bool IsSerialzable => isSerialzable;
    public bool IsSealed => isSealed;
    public bool IsEnum => isEnum;
    public bool IsClass => isClass;
    public bool IsInterface => isInterface;
    public bool IsValueType => isValueType;
    public bool IsAbstract => isAbstract;
    public TypeAttributes Attributes => attributes;
    public Type UnderlyingSystemType => GetType(underlyingSystemTypeFullName);

    protected Type(SystemType type)
    {
        this.type = type;
        name = type.Name;
        nameSpace = type.Namespace;
        fullName = type.FullName;
        isVisible = type.IsVisible;
        isNotPublic = type.IsNotPublic;
        isNested = type.IsNested;
        baseTypeFullName = type.BaseType?.FullName;
        assemblyFullName = type.Assembly.FullName;
        assemblyQualifiedName = type.AssemblyQualifiedName;
        guid = type.GUID;
        isArray = type.IsArray;
        isSerialzable = type.IsSerializable;
        isSealed = type.IsSealed;
        isEnum = type.IsEnum;
        isClass = type.IsClass;
        isInterface = type.IsInterface;
        isValueType = type.IsValueType;
        isAbstract = type.IsAbstract;
        attributes = type.Attributes;
        underlyingSystemTypeFullName = type.UnderlyingSystemType?.FullName;
    }

    public override string ToString() => SystemType.ToString();
    public override int GetHashCode() => SystemType.GetHashCode();
    public IEnumerable<Attribute> GetCustomAttributes() => SystemType.GetCustomAttributes();
    public object[] GetCustomAttributes(Type attributeType, bool inherit) => SystemType.GetCustomAttributes(attributeType, inherit);

    public override bool Equals(object other)
    {
        if (other is null)
        {
            return false;
        }

        return Equals(other as Type);
    }

    public bool Equals(Type other)
    {
        if (other is null)
        {
            return false;
        }

        return SystemType.Equals(other.SystemType);
    }

    private static void FindAll()
    {
        fullNameToType = new Dictionary<string, Type>();
        fullNameToSystemType = new Dictionary<string, SystemType>();
        systemTypeToType = new Dictionary<SystemType, Type>();

        List<Type> all = new List<Type>();
        SystemAssembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (SystemAssembly assembly in assemblies)
        {
            SystemType[] types = assembly.GetTypes();
            foreach (SystemType systemType in types)
            {
                Type type = new Type(systemType);
                all.Add(type);
                fullNameToType[systemType.FullName] = type;
                fullNameToSystemType[systemType.FullName] = systemType;
                systemTypeToType[systemType] = type;
            }
        }

        Type.all = all.AsReadOnly();
    }

    /// <summary>
    /// Returns a type using this full name.
    /// </summary>
    public static Type GetType(string fullName)
    {
        if (fullNameToType is null)
        {
            FindAll();
        }

        if (fullNameToType.TryGetValue(fullName, out Type type))
        {
            return type;
        }
        else
        {
            return null;
        }
    }

    private static SystemType GetSystemType(string fullName)
    {
        if (fullNameToSystemType is null)
        {
            FindAll();
        }

        if (fullNameToSystemType.TryGetValue(fullName, out SystemType type))
        {
            return type;
        }
        else
        {
            return null;
        }
    }

    public static implicit operator SystemType(Type type) => type.SystemType;
    public static implicit operator Type(SystemType type)
    {
        if (systemTypeToType is null)
        {
            FindAll();
        }

        if (systemTypeToType.TryGetValue(type, out Type returnType))
        {
            return returnType;
        }
        else
        {
            return null;
        }
    }

    [SecuritySafeCritical]
    public static bool operator ==(Type left, Type right)
    {
        return left?.SystemType == right?.SystemType;
    }

    [SecuritySafeCritical]
    public static bool operator !=(Type left, Type right)
    {
        return left?.SystemType != right?.SystemType;
    }
}