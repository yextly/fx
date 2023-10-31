// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents an exception thrown when a script cannot be run.
    /// </summary>
    [Serializable]
    public sealed class RuntimeExecutionException : ScriptingEngineException
    {
        private const string DefaultExceptionMessage = "The script cannot be run.";

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public RuntimeExecutionException() : base(DefaultExceptionMessage)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public RuntimeExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public RuntimeExecutionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        private RuntimeExecutionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
