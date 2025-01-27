﻿// Working Files List
// Visual Studio extension tool window that shows a selectable list of files
// that are open in the editor
// Copyright © 2016 Anthony Fung

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace WorkingFilesList.Core.Interface
{
    public interface IStoredSettingsRepository
    {
        int GetPathSegmentCount();
        void SetPathSegmentCount(int count);

        int GetUnityRefreshDelay();
        void SetUnityRefreshDelay(int delayInMilliseconds);

        string GetDocumentSortOptionName();
        void SetDocumentSortOptionName(string name);

        string GetProjectSortOptionName();
        void SetProjectSortOptionName(string name);

        bool GetGroupByProject();
        void SetGroupByProject(bool value);

        bool GetHighlightFileName();
        void SetHighlightFileName(bool value);

        bool GetShowRecentUsage();
        void SetShowRecentUsage(bool value);

        bool GetAssignProjectColours();
        void SetAssignProjectColours(bool value);

        bool GetShowFileTypeIcons();
        void SetShowFileTypeIcons(bool value);

        void Reset();
    }
}