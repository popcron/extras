using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;
using SystemAssembly = System.Reflection.Assembly;

namespace Popcron.Extras
{
    [Serializable]
    public class PopType
    {
        private static Dictionary<string, PopType> fullNameToType = null;
        private static Dictionary<string, Type> fullNameToSystemType = null;
        private static Dictionary<Type, PopType> systemTypeToType = null;
        private static ReadOnlyCollection<PopType> all = null;

        /// <summary>
        /// List of all types from all assemblies.
        /// </summary>
        public static ReadOnlyCollection<PopType> All
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

        private Type type;
        private readonly string name;
        private readonly string nameSpace;
        private readonly bool isVisible;
        private readonly bool isNotPublic;
        private readonly bool isNested;
        private readonly string baseTypeFullName;
        private readonly string assemblyQualifiedName;
        private readonly string assemblyFullName;
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
        private readonly bool isGenericType;

        private Type SystemType
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
        public PopType BaseType => GetType(baseTypeFullName);
        public PopAssembly Assembly => PopAssembly.GetAssembly(assemblyFullName);
        public string AssemblyQualifiedName => assemblyQualifiedName;
        public bool IsArray => isArray;
        public bool IsSerialzable => isSerialzable;
        public bool IsSealed => isSealed;
        public bool IsEnum => isEnum;
        public bool IsClass => isClass;
        public bool IsInterface => isInterface;
        public bool IsValueType => isValueType;
        public bool IsAbstract => isAbstract;
        public TypeAttributes Attributes => attributes;
        public PopType UnderlyingSystemType => GetType(underlyingSystemTypeFullName);
        public bool IsGenericType => isGenericType;

        protected PopType()
        {

        }

        protected PopType(Type type)
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
            isGenericType = type.IsGenericType;
        }

        public override string ToString() => SystemType.ToString();
        public override int GetHashCode() => SystemType.GetHashCode();
        public PopType GetGenericTypeDefinition() => SystemType.GetGenericTypeDefinition();
        public IEnumerable<Attribute> GetCustomAttributes() => SystemType.GetCustomAttributes();
        public T GetCustomAttribute<T>() where T : Attribute => SystemType.GetCustomAttribute<T>();
        public object[] GetCustomAttributes(Type attributeType, bool inherit) => SystemType.GetCustomAttributes(attributeType, inherit);
        public FieldInfo GetField(string name, BindingFlags flags = BindingFlags.Default) => SystemType.GetField(name, flags);
        public bool IsAssignableFrom(Type type) => SystemType.IsAssignableFrom(type);
        public FieldInfo[] GetFields(BindingFlags flags = BindingFlags.Default) => SystemType.GetFields(flags);
        public PropertyInfo[] GetProperties(BindingFlags flags = BindingFlags.Default) => SystemType.GetProperties(flags);
        public MethodInfo[] GetMethods(BindingFlags flags = BindingFlags.Default) => SystemType.GetMethods(flags);
        public MemberInfo[] GetMembers(BindingFlags flags = BindingFlags.Default) => SystemType.GetMembers(flags);
        public PopType GetInterface(string name) => SystemType.GetInterface(name);
        public PopType GetInterface(string name, bool ignoreCase) => SystemType.GetInterface(name, ignoreCase);
        public Type[] GetInterfaces() => SystemType.GetInterfaces();

        public PopType GetGenericInterface(Type type)
        {
            Type[] interfaceTypes = GetInterfaces();
            int length = interfaceTypes.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Type interfaceType = interfaceTypes[i];
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type)
                {
                    Type genericType = interfaceType.GetGenericArguments()[0];
                    return genericType;
                }
            }

            return null;
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            return Equals(other as PopType);
        }

        public bool Equals(PopType other)
        {
            if (other is null)
            {
                return false;
            }

            return SystemType.Equals(other.SystemType);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (all is null)
            {
                FindAll();
            }
        }

        private static void FindAll()
        {
            fullNameToType = new Dictionary<string, PopType>();
            fullNameToSystemType = new Dictionary<string, Type>();
            systemTypeToType = new Dictionary<Type, PopType>();

            List<PopType> all = new List<PopType>();
            SystemAssembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (SystemAssembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type systemType in types)
                {
                    PopType type = new PopType(systemType);
                    all.Add(type);
                    fullNameToType[systemType.FullName] = type;
                    fullNameToSystemType[systemType.FullName] = systemType;
                    systemTypeToType[systemType] = type;
                }
            }

            PopType.all = all.AsReadOnly();
        }

        public static IEnumerable<PopType> GetAllAssignableFrom<T>() => GetAllAssignableFrom(typeof(T));

        public static IEnumerable<PopType> GetAllAssignableFrom(PopType baseType)
        {
            ReadOnlyCollection<PopType> all = All;
            foreach (PopType type in all)
            {
                if (baseType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<(T[], M)> GetMembersWithAttribute<T, M>(BindingFlags flags = BindingFlags.Default) where T : Attribute where M : MemberInfo
        {
            ReadOnlyCollection<PopType> all = All;
            foreach (PopType type in all)
            {
                MemberInfo[] members = type.GetMembers(flags);
                foreach (MemberInfo member in members)
                {
                    T[] attributes = member.GetCustomAttributes<T>().ToArray();
                    if (attributes != null && attributes.Length > 0)
                    {
                        if (member is M m)
                        {
                            yield return (attributes, m);
                        }
                    }
                }
            }
        }

        public static IEnumerable<(T[], MemberInfo)> GetMembersWithAttribute<T>(BindingFlags flags = BindingFlags.Default) where T : Attribute
        {
            ReadOnlyCollection<PopType> all = All;
            foreach (PopType type in all)
            {
                MemberInfo[] members = type.GetMembers(flags);
                foreach (MemberInfo member in members)
                {
                    T[] attributes = member.GetCustomAttributes<T>().ToArray();
                    if (attributes != null && attributes.Length > 0)
                    {
                        yield return (attributes, member);
                    }
                }
            }
        }

        public static IEnumerable<(object[], MemberInfo)> GetMembersWithAttribute(PopType attributeType, BindingFlags flags = BindingFlags.Public)
        {
            ReadOnlyCollection<PopType> all = All;
            foreach (PopType type in all)
            {
                MemberInfo[] members = type.GetMembers(flags);
                foreach (MemberInfo member in members)
                {
                    object[] attributes = member.GetCustomAttributes(attributeType);
                    if (attributes != null && attributes.Length > 0)
                    {
                        yield return (attributes, member);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a type using this full name.
        /// </summary>
        public static PopType GetType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }

            if (fullNameToType is null)
            {
                FindAll();
            }

            if (fullNameToType.TryGetValue(fullName, out PopType type))
            {
                return type;
            }
            else
            {
                return null;
            }
        }

        private static Type GetSystemType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }

            if (fullNameToSystemType is null)
            {
                FindAll();
            }

            if (fullNameToSystemType.TryGetValue(fullName, out Type type))
            {
                return type;
            }
            else
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Type(PopType type) => type?.SystemType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(PopType type)
        {
            if (type is null)
            {
                return false;
            }

            return type.SystemType != null;
        }

        public static implicit operator PopType(Type type)
        {
            if (systemTypeToType is null)
            {
                FindAll();
            }

            if (systemTypeToType.TryGetValue(type, out PopType returnType))
            {
                return returnType;
            }
            else
            {
                return null;
            }
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PopType left, PopType right)
        {
            return left?.SystemType == right?.SystemType;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PopType left, PopType right)
        {
            return left?.SystemType != right?.SystemType;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PopType left, Type right)
        {
            return left?.SystemType == right;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PopType left, Type right)
        {
            return left?.SystemType != right;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Type left, PopType right)
        {
            return left == right?.SystemType;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Type left, PopType right)
        {
            return left != right?.SystemType;
        }
    }
}