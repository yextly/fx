// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents the base exception related to the scripting engine.
    /// </summary>
    [Serializable]
    public class ScriptingEngineException : Exception
    {
        private const string DefaultExceptionMessage = "A generic operation has failed.";

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ScriptingEngineException() : this(DefaultExceptionMessage)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ScriptingEngineException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public ScriptingEngineException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        protected ScriptingEngineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
