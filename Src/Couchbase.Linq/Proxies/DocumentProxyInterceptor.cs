﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Couchbase.Linq.Utils;

namespace Couchbase.Linq.Proxies
{
    internal class DocumentProxyInterceptor : IInterceptor
    {
        private readonly DocumentNode _documentNode = new DocumentNode();

        public  DocumentProxyInterceptor(IChangeTrackableContext context)
        {
            _documentNode.Context = context;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.InvocationTarget == null)
            {
                // This is a call against an ITrackedDocumentNode member
                // So redirect to the members on the DocumentNode

                invocation.ReturnValue = invocation.Method.Invoke(_documentNode, invocation.Arguments);
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            var property = invocation.Method.DeclaringType.GetProperties()
                .FirstOrDefault(prop => prop.GetSetMethod() == invocation.Method);

            if (property == null)
            {
                // Not a property, don't intercept
                invocation.Proceed();
                return;
            }

            var getMethod = property.GetGetMethod();

            if (getMethod == null)
            {
                // Not a property with a getter and setter, don't intercept
                invocation.Proceed();
                return;
            }

            // Save the previous value
            object initialValue = getMethod.Invoke(invocation.InvocationTarget, null);

            invocation.Proceed();

            // Get the new value
            object finalValue = getMethod.Invoke(invocation.InvocationTarget, null);

            if ((initialValue == null) && (finalValue != null) ||
                (initialValue != null && !initialValue.Equals(finalValue)))
            {
                // Value was changed

                var status = initialValue as ITrackedDocumentNode;
                if (status != null)
                {
                    _documentNode.RemoveChild(status);
                }

                status = finalValue as ITrackedDocumentNode;
                if (status != null)
                {
                    _documentNode.AddChild(status);
                }

                _documentNode.DocumentModified();

                //modified
                if (!_documentNode.IsDeserializing)
                {
                    _documentNode.Context.Modified(invocation.InvocationTarget);
                }
            }
        }
    }
}
