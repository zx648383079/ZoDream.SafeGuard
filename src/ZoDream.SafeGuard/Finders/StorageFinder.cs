﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Finders
{
    public abstract class StorageFinder
    {
        private CancellationTokenSource? _cancelTokenSource;

        public event FinderLogEventHandler? FileChanged;

        public event FinderEventHandler? FoundChanged;

        public event FinderFinishedEventHandler? Finished;


        public void Start(IEnumerable<string> folders)
        {
            Stop();
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            Task.Factory.StartNew(() => {
                CheckAnyFile(folders, token);
                Finished?.Invoke();
            }, token);
        }

        public void Stop()
        {
            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
                Finished?.Invoke();
            }
        }

        private void CheckAnyFile(IEnumerable<string> folders, CancellationToken token = default)
        {
            foreach (var item in folders)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                CheckFolderOrFile(item, token);
            }
        }

        private void CheckFolderOrFile(string fileName, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            var file = new FileInfo(fileName);
            if (file.Exists)
            {
                CheckFile(file, token);
                return;
            }
            EachFiles(fileName, items => {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                foreach (var item in items)
                {
                    CheckFile(item, token);
                }
            }, token);
        }

        private void CheckFile(string fileName, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            var file = new FileInfo(fileName);
            if (!file.Exists)
            {
                return;
            }
            CheckFile(file, token);
        }

        private void CheckFile(FileInfo file, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            FileChanged?.Invoke(file.FullName);
            if (!IsValidFile(file, token))
            {
                return;
            }
            FoundChanged?.Invoke(file);
            ProcessFile(file, token);
        }

        protected abstract bool IsValidFile(FileInfo fileInfo, CancellationToken token = default);

        protected virtual Task ProcessFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return Task.CompletedTask;
        } 

        private static void EachFiles(string folder, 
            Action<IEnumerable<string>> success,
            CancellationToken token = default)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Array.ForEach(Directory.GetDirectories(folder), fileName => {
                    EachFiles(fileName, success, token);
                });
                success.Invoke(Directory.GetFiles(folder));
            }
            catch (UnauthorizedAccessException)
            {

            }
        }
    }
}
