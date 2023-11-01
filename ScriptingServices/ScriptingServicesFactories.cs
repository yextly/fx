// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using Yextly.Scripting.Abstractions;

namespace Yextly.Scripting.Services
{
    /// <summary>
    /// Collects various factories.
    /// </summary>
    public static class ScriptingServicesFactories
    {
        /// <summary>
        /// Collects all the factories into a unique instance.
        /// </summary>
        public static IScriptingEngineFactories ServiceFactories { get; } = new ScriptingEngineFactories();
    }
}
