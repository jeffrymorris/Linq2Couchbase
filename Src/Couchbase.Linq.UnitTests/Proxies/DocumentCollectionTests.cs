using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Linq.Proxies;
using Moq;
using NUnit.Framework;

namespace Couchbase.Linq.UnitTests.Proxies
{
    [TestFixture]
    public class DocumentCollectionTests
    {
        #region IsDirty/IsDeserializing/ClearStatus

        [Test]
        public void NoAction_IsNotDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object);

            // Act

            var result = collection.IsDirty;

            // Assert

            Assert.IsFalse(result);
        }

        [Test]
        public void ClearStatus_IsNotDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                new SubDocument()
            };


            // Act

            collection.ClearStatus();

            var result = collection.IsDirty;

            // Assert

            Assert.IsFalse(result);
        }

        [Test]
        public void IsDeserializingAndMakeChanges_IsNotDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                IsDeserializing = true
            };


            // Act

            collection.Add(new SubDocument());

            var result = collection.IsDirty;

            // Assert

            Assert.IsFalse(result);
        }

        #endregion

        #region List Modifications

        [Test]
        public void AddElement_IsDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                new SubDocument()
            };

            // Act

            var result = collection.IsDirty;

            // Assert

            Assert.IsTrue(result);
        }

        [Test]
        public void RemoveElement_IsDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                new SubDocument()
            };

            collection.ClearStatus();

            // Act

            collection.RemoveAt(0);

            var result = collection.IsDirty;

            // Assert

            Assert.IsTrue(result);
        }

        [Test]
        public void SetElement_IsDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                new SubDocument()
            };

            collection.ClearStatus();

            // Act

            collection[0] = new SubDocument();

            var result = collection.IsDirty;

            // Assert

            Assert.IsTrue(result);
        }

        [Test]
        public void Clear_IsDirty()
        {
            // Arrange

            var collection = new DocumentCollection<SubDocument>(new Mock<IChangeTrackableContext>().Object)
            {
                new SubDocument()
            };

            collection.ClearStatus();

            // Act

            collection.Clear();

            var result = collection.IsDirty;

            // Assert

            Assert.IsTrue(result);
        }

        #endregion

        #region SubDocument

        [Test]
        public void SetProperty_OnSubDocument_IsDirty()
        {
            // Arrange
            var context = new Mock<IChangeTrackableContext>().Object;

            var collection = new DocumentCollection<SubDocument>(context)
            {
                (SubDocument)DocumentProxyManager.Default.CreateProxy(typeof (SubDocument), context)
            };

            collection.ClearStatus();

            // Act

            collection[0].IntegerProperty = 1;

            var result = collection.IsDirty;

            // Assert

            Assert.IsTrue(result);
        }

        [Test]
        public void SetProperty_OnRemovedSubDocument_IsNotDirty()
        {
            // Arrange

            var context = new Mock<IChangeTrackableContext>().Object;

            var originalDocument = (SubDocument) DocumentProxyManager.Default.CreateProxy(typeof (SubDocument), context);

            var collection = new DocumentCollection<SubDocument>(context)
            {
                originalDocument
            };

            collection.RemoveAt(0);
            collection.Add((SubDocument) DocumentProxyManager.Default.CreateProxy(typeof (SubDocument), context));

            collection.ClearStatus();

            // Act

            originalDocument.IntegerProperty = 1;

            var result = collection.IsDirty;

            // Assert

            Assert.IsFalse(result);
        }

        [Test]
        public void ClearStatus_SubDocument_IsNotDirty()
        {
            // Arrange

            var context = new Mock<IChangeTrackableContext>().Object;

            var collection = new DocumentCollection<SubDocument>(context)
            {
                (SubDocument)DocumentProxyManager.Default.CreateProxy(typeof (SubDocument), context)
            };

            collection[0].IntegerProperty = 1;

            collection.ClearStatus();

            // Act

            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = ((ITrackedDocumentNode)collection[0]).IsDirty;

            // Assert

            Assert.IsFalse(result);
        }

        [Test]
        public void ClearStatus_SubDocument_IsNotDeserializing()
        {
            // Arrange

            var context = new Mock<IChangeTrackableContext>().Object;

            var collection = new DocumentCollection<SubDocument>(context)
            {
                (SubDocument)DocumentProxyManager.Default.CreateProxy(typeof (SubDocument), context)
            };

            // ReSharper disable once SuspiciousTypeConversion.Global
            ((ITrackedDocumentNode)collection[0]).IsDeserializing = true;

            // Act

            collection.ClearStatus();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = ((ITrackedDocumentNode)collection[0]).IsDeserializing;

            // Assert

            Assert.IsFalse(result);
        }

        #endregion

        #region Helpers

        public class SubDocument
        {
            public virtual int IntegerProperty { get; set; }
        }

        #endregion
    }
}
