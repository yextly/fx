// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Yextly.ServiceFabric.WellKnownIdentifiers;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Contains <see cref="IServiceProvider"/> extensions.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets the metadata necessary to register the current CRUD resources with the Fabric.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Incorrect metadata or resource declaration.</exception>
        public static ImmutableArray<CrudResourceDescriptor> GetRegisteredRestResources(this IServiceProvider serviceProvider)
        {
            var ret = ImmutableArray.CreateBuilder<CrudResourceDescriptor>();

            var actions = serviceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();

            var list = new List<(Guid Id, ControllerActionDescriptor Descriptor, CrudResourceAttribute Resource, RoleBasedAuthorizationAttribute Authorization)>();

            foreach (var item in actions.ActionDescriptors.Items)
            {
                if (item is not ControllerActionDescriptor descriptor)
                    continue;

                var authorization = FindAttribute<RoleBasedAuthorizationAttribute>(item.EndpointMetadata);
                var resource = FindAttribute<CrudResourceAttribute>(item.EndpointMetadata);

                if (authorization != null)
                {
                    if (resource == null)
                        throw new InvalidOperationException($"A CRUD resource is not properly defined: the controller {descriptor.ControllerTypeInfo.FullName} specifies {nameof(RoleBasedAuthorizationAttribute)}, but not {nameof(CrudResourceAttribute)}.");

                    if (!Guid.TryParse(resource.ResourceId, out var resourceId))
                        throw new InvalidOperationException($"A CRUD resource is not properly defined: the controller {descriptor.ControllerTypeInfo.FullName} specifies {nameof(CrudResourceAttribute)} with an invalid resource identifier");

                    list.Add((resourceId, descriptor, resource, authorization));
                }
            }

            foreach (var item in list.GroupBy(x => x.Id))
            {
                var resource = item.First().Resource;
                var descriptor = item.First().Descriptor;

                var id = item.Key.ToString();
                var name = descriptor.AttributeRouteInfo?.Name ?? ToCamelCase(descriptor.ControllerName);
                var description = resource.Description;

                var r = new CrudResourceDescriptor(id, name, description)
                {
                    CreateRoles = CreateRoles(item, x => x.Authorization.CreateRoleName).ToImmutableArray(),
                    ReadRoles = CreateRoles(item, x => x.Authorization.ReadRoleName).ToImmutableArray(),
                    UpdateRoles = CreateRoles(item, x => x.Authorization.UpdateRoleName).ToImmutableArray(),
                    DeleteRoles = CreateRoles(item, x => x.Authorization.DeleteRoleName).ToImmutableArray(),
                };
                ret.Add(r);
            }

            return ret.ToImmutableArray();
        }

        private static IEnumerable<string> CreateRoles<T>(IEnumerable<T> items, Func<T, string> roleGetter)
        {
            var roles = new HashSet<Guid>();
            foreach (var item in items)
            {
                var allRoles = roleGetter(item)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Guid.TryParse(x, out var ret) ? ret : KnownRoles.NoRole);

                foreach (var r in allRoles)
                {
                    roles.Add(r);
                }
            }

            roles.Remove(KnownRoles.NoRole);

            return roles.Select(x => x.ToString());
        }

        private static T? FindAttribute<T>(IList<object> list) => (T?)list.FirstOrDefault(x => x is T);

        private static string ToCamelCase(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return char.ToLowerInvariant(value[0]) + value[1..];
        }
    }
}
