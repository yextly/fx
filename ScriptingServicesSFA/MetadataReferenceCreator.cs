// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;

namespace Yextly.Scripting.Services.SingleFileApplication
{
    /// <summary>
    /// Provides metadata creation utilities to work around the current limitation that the metadata is probed via Location instead.
    /// </summary>
    /// <remarks>Portion of this class comes from https://github.com/dotnet/runtime/issues/36590#issuecomment-689883856</remarks>
    public static class MetadataReferenceCreator
    {
        /// <summary>
        /// Creates a <see cref="MetadataReference"/> from the provided <see cref="Assembly"/> without using the problematic APIs when in SFA.
        /// </summary>
        /// <param name="assembly">The assembly whose metadata must be created from.</param>
        /// <returns></returns>
        /// <remarks>This method behaves differently depending on how the application is compiled: when not in SFA, you can reference every file-system backed assembly.
        /// However, when in SFA, referencing an assembly not contained in the bundle throws an exception.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The metadata store does not contain the provided assembly.</exception>
        public static MetadataReference CreateReference(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var location = GetLocation(assembly);

            if (location != null)
            {
                // Debugging, testing or normal publishing
                return MetadataReference.CreateFromFile(location);
            }

            // This is the SFA execution flow
            unsafe
            {
                if (!assembly.TryGetRawMetadata(out byte* blob, out int length))
                    throw new InvalidOperationException($"Failed to access the metadata store for {assembly.FullName}.");

#pragma warning disable CA2000 // Dispose objects before losing scope
                // We suppress disposable warnings since we load these metadata only once
                // and it is not clear what happens when the disposal occurs: are the inner pointers
                // still pointing to what they should?
                // The benefits of leaking them is clear in this case, therefore, at least for now, we leak them.

                var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
#pragma warning restore CA2000 // Dispose objects before losing scope

                var metadataReference = assemblyMetadata.GetReference();
                return metadataReference;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "This is here for a reason: the suppression is enforced when publishing as a single file application.")]
        internal static string? GetLocation(Assembly assembly)
        {
            // We use a side effect here: when in SFA, Location is null. This tells us that we are likely in this scenario.
#pragma warning disable IL3000
            ArgumentNullException.ThrowIfNull(assembly);

            if (string.IsNullOrEmpty(assembly.Location))
                return null;
            else
                return assembly.Location;
#pragma warning restore IL3000
        }
    }
}
