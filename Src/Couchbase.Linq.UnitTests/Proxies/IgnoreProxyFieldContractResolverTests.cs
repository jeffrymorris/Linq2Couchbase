using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Linq.Proxies;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Couchbase.Linq.UnitTests.Proxies
{
    [TestFixture]
    public class IgnoreProxyFieldContractResolverTests
    {
        [Test]
        public void SerializeObject_Ignores_Proxy_Class_Members()
        {
            var poco = new PocoWithProxyProperties
            {
                __id = "thekey",
                context = new object(),
                IsDirty = false,
                IsDeserializing = true,
                IsDeleted = true,
                Good = "All good!"
            };

            var actual = JsonConvert.SerializeObject(poco, new JsonSerializerSettings
            {
                ContractResolver = new IgnoreProxyFieldContractResolver()
            });

            var expected = "{\"good\":\"All good!\"}";
            Assert.AreEqual(expected, actual);
        }

        public class PocoWithProxyProperties
        {
            public object __id { get; set; }
            [IgnoreDataMember]
            public object context { get; set; }
            [IgnoreDataMember]
            public bool IsDirty { get; set; }
            [IgnoreDataMember]
            public bool IsDeserializing { get; set; }
            [IgnoreDataMember]
            public bool IsDeleted { get; set; }
            public string Good { get; set; }
        }
    }
}
