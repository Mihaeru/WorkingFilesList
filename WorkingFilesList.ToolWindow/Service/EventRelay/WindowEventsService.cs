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
using WorkingFilesList.ToolWindow.Interface;
using WorkingFilesList.ToolWindow.Model;

namespace WorkingFilesList.ToolWindow.Service.EventRelay
{
    /// <summary>
    /// Contains methods for processing event handler parameters for relevant
    /// <see cref="WindowEvents"/> events, and invoking appropriate methods on
    /// other services to respond to such events
    /// </summary>
    public class WindowEventsService : IWindowEventsService
    {
        private readonly IDocumentMetadataManager _documentMetadataManager;

        public WindowEventsService(IDocumentMetadataManager documentMetadataManager)
        {
            _documentMetadataManager = documentMetadataManager;
        }

        public void WindowActivated(Window gotFocus, Window lostFocus)
        {
            if (gotFocus.Type == vsWindowType.vsWindowTypeDocument &&
                gotFocus.Document.ActiveWindow != null)
            {
                _documentMetadataManager.Activate(
                    gotFocus.Document.FullName);
            }
        }

        public void WindowClosing(Window window)
        {
            if (window.Type == vsWindowType.vsWindowTypeDocument)
            {
                _documentMetadataManager.Synchronize(window.DTE.Documents, true);
            }
        }

        public void WindowCreated(Window window)
        {
            if (window.Type == vsWindowType.vsWindowTypeDocument &&
                window.Document.ActiveWindow != null)
            {
                if (_documentMetadataManager.ActiveDocumentMetadata.IsEmpty)
                {
                    _documentMetadataManager.Synchronize(window.DTE.Documents, false);

                    _documentMetadataManager.Activate(
                        window.Document.FullName);
                }
                else
                {
                    var info = new DocumentMetadataInfo
                    {
                        FullName = window.Document.FullName,

                        ProjectDisplayName =
                            window.Document.ProjectItem.ContainingProject.Name,

                        ProjectUniqueName =
                            window.Document.ProjectItem.ContainingProject.UniqueName
                    };

                    _documentMetadataManager.Add(info);
                }
            }
        }
    }
}