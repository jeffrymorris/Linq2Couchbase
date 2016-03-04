﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Couchbase.Linq.Proxies
{
    internal interface ITrackedDocumentNode
    {
        [IgnoreDataMember]
        bool IsDeleted { get; set; }

        [IgnoreDataMember]
        bool IsDeserializing { get; set; }

        [IgnoreDataMember]
        bool IsDirty { get; set; }

        /// <summary>
        /// If this is the root node in a document tree, this should contain the document ID.  Otherwise null.
        /// </summary>
        /// <remarks>
        /// The property name __id is important for compatibility with other JSON deserializers, since we can't rely
        /// on JsonProperty attributes for Newtonsoft.Json.
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        [IgnoreDataMember]
        string __id { get; set; }

        [IgnoreDataMember]
        IChangeTrackableContext Context { get; set; }

        void RegisterChangeTracking(ITrackedDocumentNodeCallback callback);

        void UnregisterChangeTracking(ITrackedDocumentNodeCallback callback);

        /// <summary>
        /// Clears IsDeserializing and IsDirty on this document and all child documents.
        /// Does nothing if IsDeserialization is already false to prevent accidental infinite recursion.
        /// </summary>
        void ClearStatus();
    }
}
