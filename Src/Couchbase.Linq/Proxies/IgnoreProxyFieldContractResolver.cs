using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Couchbase.Linq.Proxies
{
    /// <summary>
    /// A contract resolver which ignores fields used for proxy state. Additional fields to ignore can be added
    /// and the default ignored fields can be cleared (__interceptors, context, isDirty, isDeserializing, isDeleted).
    /// <remarks>Derives from <see cref="CamelCasePropertyNamesContractResolver"/>.</remarks>
    /// </summary>
    public class IgnoreProxyFieldContractResolver : CamelCasePropertyNamesContractResolver
    {
        //the default ignored fields
        private readonly List<string> _ignoredFields = new List<string>
        {
            "__id"
        };

        /// <summary>
        /// Adds a range of fields to ignore.
        /// </summary>
        /// <param name="fields"></param>
        public void AddFields(params string[] fields)
        {
            _ignoredFields.AddRange(fields);
        }

        /// <summary>
        /// Adds a field to ignore.
        /// </summary>
        /// <param name="field"></param>
        public void AddField(string field)
        {
            _ignoredFields.Add(field);
        }

        /// <summary>
        /// Clears the ignored fields list.
        /// </summary>
        public void ClearFields()
        {
            _ignoredFields.Clear();
        }

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract"/> and ignores any properties provided passed into the
        /// constructor and by default ignores the <see cref="ITrackedDocumentNode.__id"/> property.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>/// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>
        /// Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract"/>.
        /// </returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(x => !_ignoredFields.Contains(x.PropertyName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
    }
}
