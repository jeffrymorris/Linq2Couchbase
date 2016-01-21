﻿using System;

namespace Couchbase.Linq.Filters
{
    /// <summary>
    /// Abstract base class for attribute-based <see cref="IDocumentFilter{T}">IDocumentFilter</see> implementations
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DocumentFilterAttribute : Attribute
    {
        /// <summary>
        /// Priority of this filter compared to other filters against the same type.  Lower priorities execute first.
        /// </summary>
        public int Priority { get; set; }

        public abstract IDocumentFilter<T> GetFilter<T>(); 

    }
}
