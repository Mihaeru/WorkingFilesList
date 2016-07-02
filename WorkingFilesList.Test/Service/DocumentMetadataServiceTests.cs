﻿// WorkingFilesList
// Visual Studio extension tool window that shows a selectable list of files
// that are open in the editor
// Copyright(C) 2016 Anthony Fung

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see<http://www.gnu.org/licenses/>.

using EnvDTE;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkingFilesList.Model;
using WorkingFilesList.Test.TestingInfrastructure;

namespace WorkingFilesList.Test.Service
{
    [TestFixture]
    public class DocumentMetadataServiceTests
    {
        private static Document CreateDocument(string fullName)
        {
            var documentMock = new Mock<Document>();
            documentMock.Setup(d => d.FullName).Returns(fullName);
            return documentMock.Object;
        }

        private static Documents CreateDocuments(List<Document> documentsToReturn)
        {
            var documentsMock = new Mock<Documents>();

            documentsMock.Setup(d => d.GetEnumerator())
                .Returns(documentsToReturn.GetEnumerator());

            return documentsMock.Object;
        }

        [Test]
        public void UpsertAddsDocumentMetadataToListIfFullPathDoesNotExist()
        {
            // Arrange

            const string documentName = "DocumentName";

            var builder = new DocumentMetadataServiceBuilder();
            var service = builder.CreateDocumentMetadataService();

            // Act

            service.Upsert(documentName);

            // Assert

            Assert.That(service.ActiveDocumentMetadata.Count, Is.EqualTo(1));
            Assert.That(
                ((DocumentMetadata) service.ActiveDocumentMetadata.GetItemAt(0)).FullName,
                Is.EqualTo(documentName));
        }

        [Test]
        public void UpsertDoesNotAddDocumentMetadataToListIfFullPathAlreadyExist()
        {
            // Arrange

            const string documentName = "DocumentName";

            var builder = new DocumentMetadataServiceBuilder();
            var service = builder.CreateDocumentMetadataService();

            // Act

            service.Upsert(documentName);
            service.Upsert(documentName);

            // Assert

            Assert.That(service.ActiveDocumentMetadata.Count, Is.EqualTo(1));
            Assert.That(
                ((DocumentMetadata) service.ActiveDocumentMetadata.GetItemAt(0)).FullName,
                Is.EqualTo(documentName));
        }

        [Test]
        public void SynchronizeAddsDocumentsMissingInTarget()
        {
            // Arrange

            const string document1Name = "Document1Name";
            const string document2Name = "Document2Name";

            var documentMockList = new List<Document>
            {
                CreateDocument(document1Name),
                CreateDocument(document2Name)
            };

            var builder = new DocumentMetadataServiceBuilder();
            var service = builder.CreateDocumentMetadataService();
            var documents = CreateDocuments(documentMockList);

            // Act

            service.Synchronize(documents);

            // Assert

            Assert.That(service.ActiveDocumentMetadata.Count, Is.EqualTo(2));

            var collection =
                (IList<DocumentMetadata>) service.ActiveDocumentMetadata.SourceCollection;

            var document1 = collection.SingleOrDefault(m => m.FullName == document1Name);
            var document2 = collection.SingleOrDefault(m => m.FullName == document2Name);

            Assert.That(document1, Is.Not.Null);
            Assert.That(document2, Is.Not.Null);
        }

        [Test]
        public void SynchronizeRemovesDocumentMissingInSource()
        {
            // Arrange

            const string documentToRemove = "DocumentToRemove";
            const string documentToRetain = "DocumentToRetain";

            var documentMockList = new List<Document>
            {
                CreateDocument(documentToRemove),
                CreateDocument(documentToRetain)
            };

            var builder = new DocumentMetadataServiceBuilder();
            var service = builder.CreateDocumentMetadataService();
            var documents = CreateDocuments(documentMockList);

            // Synchronize to set two items in the document metadata service
            // metadata list

            service.Synchronize(documents);

            var updatedDocumentMockList = new List<Document>
            {
                CreateDocument(documentToRetain)
            };

            // Synchronizing with the updated list should remove one item

            var updatedDocuments = CreateDocuments(updatedDocumentMockList);

            // Act

            service.Synchronize(updatedDocuments);

            // Assert

            Assert.That(service.ActiveDocumentMetadata.Count, Is.EqualTo(1));

            var collection =
                (IList<DocumentMetadata>)service.ActiveDocumentMetadata.SourceCollection;

            var remove = collection.SingleOrDefault(m => m.FullName == documentToRemove);
            var retain = collection.SingleOrDefault(m => m.FullName == documentToRetain);

            Assert.That(remove, Is.Null);
            Assert.That(retain, Is.Not.Null);
        }

        [Test]
        public void DocumentsAddedBySynchronizeSetActivatedAt()
        {
            // Arrange

            var activatedAt = DateTime.UtcNow;

            var documentMockList = new List<Document>
            {
                CreateDocument(string.Empty)
            };
            
            var builder = new DocumentMetadataServiceBuilder();
            builder.TimeProviderMock.Setup(t => t.UtcNow)
                .Returns(activatedAt);

            var service = builder.CreateDocumentMetadataService();
            var documents = CreateDocuments(documentMockList);

            // Act

            service.Synchronize(documents);

            // Assert

            var collection =
                (IList<DocumentMetadata>)service.ActiveDocumentMetadata.SourceCollection;

            var document = collection.Single();

            Assert.That(document.ActivatedAt, Is.EqualTo(activatedAt));
        }
    }
}
