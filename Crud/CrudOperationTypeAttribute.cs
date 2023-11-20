// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.ServiceFabric.Mvc.Crud
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal sealed class CrudOperationTypeAttribute(OperationType operationType) : Attribute
    {
        public OperationType OperationType { get; } = operationType;
    }
}
