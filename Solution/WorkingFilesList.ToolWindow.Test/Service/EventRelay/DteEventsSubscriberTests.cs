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

using EnvDTE;
using EnvDTE80;
using Moq;
using NUnit.Framework;
using WorkingFilesList.ToolWindow.Test.TestingInfrastructure;

namespace WorkingFilesList.ToolWindow.Test.Service.EventRelay
{
    [TestFixture]
    public class DteEventsSubscriberTests
    {
        private class TestingMocks
        {
            public Mock<Events2> Events2Mock { get; }
                = new Mock<Events2>();

            public Mock<ProjectItemsEvents> ProjectItemsEventsMock { get; }
                = new Mock<ProjectItemsEvents>();

            public Mock<SolutionEvents> SolutionEventsMock { get; }
                = new Mock<SolutionEvents>();

            public Mock<WindowEvents> WindowEventsMock { get; }
                = new Mock<WindowEvents>();

            public TestingMocks()
            {
                Events2Mock
                    .Setup(e => e.ProjectItemsEvents)
                    .Returns(ProjectItemsEventsMock.Object);

                Events2Mock
                    .Setup(e => e.SolutionEvents)
                    .Returns(SolutionEventsMock.Object);

                Events2Mock
                    .Setup(e => e.get_WindowEvents(It.IsAny<Window>()))
                    .Returns(WindowEventsMock.Object);
            }
        }

        [Test]
        public void SubscribesToWindowActivated()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            var gotFocus = Mock.Of<Window>();
            var lostFocus = Mock.Of<Window>();

            // Act

            mocks.WindowEventsMock.Raise(w =>
                w.WindowActivated += null,
                gotFocus,
                lostFocus);

            // Assert

            builder.WindowEventsServiceMock.Verify(w =>
                w.WindowActivated(
                    gotFocus,
                    lostFocus));
        }

        [Test]
        public void SubscribesToWindowCreated()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            var window = Mock.Of<Window>();

            // Act

            mocks.WindowEventsMock.Raise(w =>
                w.WindowCreated += null,
                window);

            // Assert

            builder.WindowEventsServiceMock.Verify(w =>
                w.WindowCreated(window));
        }

        [Test]
        public void SubscribesToWindowClosing()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            var window = Mock.Of<Window>();

            // Act

            mocks.WindowEventsMock.Raise(w =>
                w.WindowClosing += null,
                window);

            // Assert

            builder.WindowEventsServiceMock.Verify(w =>
                w.WindowClosing(window));
        }

        [Test]
        public void SubscribesToItemRenamed()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            var projectItem = Mock.Of<ProjectItem>();
            const string oldName = "OldName";

            // Act

            mocks.ProjectItemsEventsMock.Raise(w =>
                w.ItemRenamed += null,
                projectItem,
                oldName);

            // Assert

            builder.ProjectItemsEventsServiceMock.Verify(w =>
                w.ItemRenamed(
                    projectItem,
                    oldName));
        }

        [Test]
        public void SubscribesToAfterClosing()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            // Act

            mocks.SolutionEventsMock.Raise(s =>
                s.AfterClosing += null);

            // Assert

            builder.SolutionEventsServiceMock.Verify(w =>
                w.AfterClosing());
        }

        [Test]
        public void SubscribesToProjectRenamed()
        {
            // Arrange

            var mocks = new TestingMocks();
            var builder = new DteEventsSubscriberBuilder();

            var subscriber = builder.CreateDteEventsSubscriber();
            subscriber.SubscribeTo(mocks.Events2Mock.Object);

            var project = Mock.Of<Project>();
            const string oldName = "OldName";

            // Act

            mocks.SolutionEventsMock.Raise(s =>
                s.ProjectRenamed += null,
                project,
                oldName);

            // Assert

            builder.SolutionEventsServiceMock.Verify(w =>
                w.ProjectRenamed(project, oldName));
        }
    }
}