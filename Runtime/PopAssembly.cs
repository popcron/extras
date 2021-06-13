using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Popcron.Extras
{
    [Serializable]
    public class PopAssembly
    {
        private static Dictionary<string, PopAssembly> fullNameToAssembly = null;
        private static Dictionary<Assembly, PopAssembly> systemAssemblyToAssembly = null;
        private static ReadOnlyCollection<PopAssembly> all = null;

        /// <summary>
        /// List of all types from all assemblies.
        /// </summary>
        public static ReadOnlyCollection<PopAssembly> All
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

        private Assembly assembly;
        private readonly string location;
        private readonly PopType[] types;

        private Assembly SystemAssembly
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

        protected PopAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            fullName = assembly.FullName;

            try
            {
                location = assembly.Location;
            }
            catch
            {

            }

            Type[] systemTypes = assembly.GetTypes();
            types = new PopType[systemTypes.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = systemTypes[i];
            }
        }

        public PopType[] GetTypes() => types;
        public override int GetHashCode() => SystemAssembly.GetHashCode();

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            return Equals(other as PopAssembly);
        }

        public bool Equals(PopAssembly other)
        {
            if (other is null)
            {
                return false;
            }

            return SystemAssembly.Equals(other.SystemAssembly);
        }

        private static void FindAll()
        {
            fullNameToAssembly = new Dictionary<string, PopAssembly>();
            systemAssemblyToAssembly = new Dictionary<Assembly, PopAssembly>();

            List<PopAssembly> all = new List<PopAssembly>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                PopAssembly newType = new PopAssembly(assembly);
                all.Add(newType);
                fullNameToAssembly[assembly.FullName] = newType;
                systemAssemblyToAssembly[assembly] = newType;
            }

            PopAssembly.all = all.AsReadOnly();
        }

        /// <summary>
        /// Returns a type using this full name.
        /// </summary>
        public static PopAssembly GetAssembly(string fullName)
        {
            if (fullNameToAssembly is null)
            {
                FindAll();
            }

            if (fullNameToAssembly.TryGetValue(fullName, out PopAssembly assembly))
            {
                return assembly;
            }
            else
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Assembly(PopAssembly assembly) => assembly?.SystemAssembly;
        public static implicit operator PopAssembly(Assembly assembly)
        {
            if (systemAssemblyToAssembly is null)
            {
                FindAll();
            }

            return systemAssemblyToAssembly[assembly];
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PopAssembly left, PopAssembly right)
        {
            return left.SystemAssembly == right.SystemAssembly;
        }

        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PopAssembly left, PopAssembly right)
        {
            return left.SystemAssembly != right.SystemAssembly;
        }
    }
}