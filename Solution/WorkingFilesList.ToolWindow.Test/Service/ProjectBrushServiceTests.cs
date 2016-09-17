﻿// Working Files List
// Visual Studio extension tool window that shows a selectable list of files
// that are open in the editor
// Copyright © 2016 Anthony Fung

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using Moq;
using NUnit.Framework;
using System.Linq;
using System.Windows.Media;
using WorkingFilesList.ToolWindow.Interface;
using WorkingFilesList.ToolWindow.Service;

namespace WorkingFilesList.ToolWindow.Test.Service
{
    [TestFixture]
    public class ProjectBrushServiceTests
    {
        private static IProjectBrushes CreateProjectBrushes()
        {
            var brushes = Mock.Of<IProjectBrushes>(p =>
                p.GenericBrush == Brushes.Orange &&
                p.ProjectSpecificBrushes == new[]
                {
                    Brushes.AliceBlue,
                    Brushes.Brown,
                    Brushes.CornflowerBlue
                });

            return brushes;
        }

        [Test]
        public void GetBrushReturnsNonNullWhenAssigningProjectColours()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush = service.GetBrush("Id", userPreferences);

            // Assert

            Assert.That(brush, Is.Not.Null);
        }

        [Test]
        public void SameBrushReturnedForSameIdWhenAssigningProjectColours()
        {
            // Arrange

            const string id = "Id";

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush1 = service.GetBrush(id, userPreferences);
            var brush2 = service.GetBrush(id, userPreferences);

            // Assert

            Assert.That(brush1, Is.EqualTo(brush2));
        }

        [Test]
        public void DifferentBrushReturnedForDifferentIdsWhenAssigningProjectColours()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush1 = service.GetBrush("Id1", userPreferences);
            var brush2 = service.GetBrush("Id2", userPreferences);

            // Assert

            Assert.That(brush1, Is.Not.EqualTo(brush2));
        }

        [Test]
        public void ProjectBrushesAreAssignedInDeclaredOrderWhenAssigningProjectColours()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush1 = service.GetBrush("Id1", userPreferences);
            var brush2 = service.GetBrush("Id2", userPreferences);
            var brush3 = service.GetBrush("Id3", userPreferences);

            // Assert

            Assert.That(brush1, Is.EqualTo(brushes.ProjectSpecificBrushes[0]));
            Assert.That(brush2, Is.EqualTo(brushes.ProjectSpecificBrushes[1]));
            Assert.That(brush3, Is.EqualTo(brushes.ProjectSpecificBrushes[2]));
        }

        [Test]
        public void Brush1ReusedForId4When3UniqueBrushesAvailableAndAssigningProjectColours()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush1 = service.GetBrush("Id1", userPreferences);
            service.GetBrush("Id2", userPreferences);
            service.GetBrush("Id3", userPreferences);
            var brush4 = service.GetBrush("Id4", userPreferences);

            // Assert

            Assert.That(brush4, Is.EqualTo(brush1));
        }

        [Test]
        public void GenericBrushReturnedWhenNotAssigningProjectColoursAndShowingRecentUsage()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                !u.AssignProjectColours &&
                u.ShowRecentUsage);

            // Act

            var brush = service.GetBrush("Id", userPreferences);

            // Assert

            Assert.That(brush, Is.EqualTo(brushes.GenericBrush));
        }

        [Test]
        public void TransparentReturnedWhenNotAssigningProjectColoursAndNotShowingRecentUsage()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                !u.AssignProjectColours &&
                !u.ShowRecentUsage);

            // Act

            var brush = service.GetBrush("Id", userPreferences);

            // Assert

            Assert.That(brush, Is.EqualTo(Brushes.Transparent));
        }

        [Test]
        public void SecondIdReturnsSameBrushAsFirstIdAfterClearingIdCollection()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var brush1 = service.GetBrush("Id1", userPreferences);
            service.ClearBrushIdCollection();
            var brush2 = service.GetBrush("Id2", userPreferences);

            // Assert

            var firstProjectBrush = brushes.ProjectSpecificBrushes[0];

            Assert.That(brush1, Is.EqualTo(firstProjectBrush));
            Assert.That(brush2, Is.EqualTo(firstProjectBrush));

            Assert.That(brushes.ProjectSpecificBrushes.Length, Is.GreaterThan(1));

            var isUnique = brushes
                .ProjectSpecificBrushes
                .Count(b => b.Equals(firstProjectBrush)) == 1;

            Assert.IsTrue(isUnique);
        }

        [Test]
        public void NewIdReturnsBrushOfOldIdAfterUpdatingBrushId()
        {
            // Arrange

            const string newId = "NewId";
            const string oldId = "OldId";

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            // Act

            var oldIdBrush = service.GetBrush(oldId, userPreferences);
            service.UpdateBrushId(oldId, newId);
            var newIdBrush = service.GetBrush(newId, userPreferences);

            // Assert

            Assert.That(oldIdBrush, Is.EqualTo(newIdBrush));
        }

        [Test]
        public void UpdatingNonExistentIdDoesNotThrowException()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            // Assert

            Assert.DoesNotThrow(() =>
                service.UpdateBrushId("NonExistentId", "NewId"));
        }

        [Test]
        public void UpdateDoesNotThrowExceptionWhenNewIdIsNull()
        {
            // Arrange

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            // Assert

            Assert.DoesNotThrow(() =>
                service.UpdateBrushId("OldId", null));
        }

        [Test]
        public void UpdatingToAlreadyExistingIdReturnsBrushForExistingId()
        {
            // Arrange

            const string id1 = "Id1";
            const string id2 = "Id2";

            var brushes = CreateProjectBrushes();
            var service = new ProjectBrushService(brushes);

            var userPreferences = Mock.Of<IUserPreferences>(u =>
                u.AssignProjectColours);

            var id1Brush = service.GetBrush(id1, userPreferences);
            var id2Brush = service.GetBrush(id2, userPreferences);

            // Act

            service.UpdateBrushId(id2, id1);
            var brush = service.GetBrush(id1, userPreferences);

            // Assert

            Assert.That(brush, Is.EqualTo(id1Brush));
            Assert.That(brush, Is.Not.EqualTo(id2Brush));
        }
    }
}