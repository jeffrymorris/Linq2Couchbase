﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq.UnitTests.Documents;
using Moq;
using NUnit.Framework;

namespace Couchbase.Linq.UnitTests.QueryGeneration
{
    [TestFixture]
    class NullHandlingTests : N1QLTestBase
    {
        
        [Test]
        public void Test_HasValue()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Class1>(mockBucket.Object)
                    .Where(e => e.Updated.HasValue)
                    .Select(e => new { e.Updated});

            const string expected =
                "SELECT `Extent1`.`Updated` as `Updated` FROM `default` as `Extent1` WHERE (`Extent1`.`Updated` IS NOT NULL)";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_NotHasValue()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Class1>(mockBucket.Object)
                    .Where(e => !e.Updated.HasValue)
                    .Select(e => new { e.Updated });

            const string expected =
                "SELECT `Extent1`.`Updated` as `Updated` FROM `default` as `Extent1` WHERE NOT (`Extent1`.`Updated` IS NOT NULL)";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        [Test]
        public void Test_Value()
        {
            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");

            var query =
                QueryFactory.Queryable<Class1>(mockBucket.Object)
                    .Where(e => e.Updated.Value < new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .Select(e => new { e.Updated });

            const string expected =
                "SELECT `Extent1`.`Updated` as `Updated` FROM `default` as `Extent1` " + 
                "WHERE (STR_TO_MILLIS(`Extent1`.`Updated`) < STR_TO_MILLIS(\"2000-01-01T00:00:00Z\"))";

            var n1QlQuery = CreateN1QlQuery(mockBucket.Object, query.Expression);

            Assert.AreEqual(expected, n1QlQuery);
        }

        #region Helpers

        private class Class1
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public DateTime? Updated { get; set; }
        }

        #endregion

    }
}
