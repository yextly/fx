// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Runtime.Versioning;

namespace System.Reflection
{
    /// <summary>
    /// Provides extensions for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the framework for which the assembly has been compiled against.
        /// </summary>
        /// <param name="instance">The assembly whose target framework needs to be retrieved.</param>
        /// <returns></returns>
        public static FrameworkName? GetTargetFramework(this Assembly instance)
        {
            ArgumentNullException.ThrowIfNull(instance);

            // The target framework moniker that this assembly was compiled against.
            // Use the FrameworkName class to interpret target framework monikers.

            var targetFrameworkAttribute = instance
                .GetCustomAttributes<TargetFrameworkAttribute>()
                .SingleOrDefault();

            var tfm = targetFrameworkAttribute?.FrameworkName;

            if (string.IsNullOrWhiteSpace(tfm))
                return null;
            else
                return new FrameworkName(tfm);
        }
    }
}
