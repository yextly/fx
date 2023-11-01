// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents a script.
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// The unique identifier of the item.
        /// </summary>
        Guid Id { get; }
    }
}
