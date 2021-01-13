using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcSearch2
{

    static class MenuLogic
    {

        public static async Task FileOpen()
        {

            string path; //Specify initial or root path
            IEnumerable<DirectoryInfo> r; // placeholder for TraverseRtee the list of paths

            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                path = fb.SelectedPath;


                if (ProcDb.GetInstace.Count > 0)
                {
                    ProcDb.GetInstace.Clear();
                }

                Action getDir = () =>
                {
                    ProcDb instance = ProcDb.GetInstace;

                    r = GetDirRecursive(path, f => f.Exists == true);

                    foreach (DirectoryInfo p in r)
                    {
                        instance.AddDirectory(p);
                    }
                };
                Task t = new Task(getDir);

                t.Start();
            }
        }

        public static IEnumerable<DirectoryInfo> GetDirRecursive(string path, Func<DirectoryInfo, bool> pattern)
        {
            var directoryStack = new Stack<DirectoryInfo>();
            directoryStack.Push(new DirectoryInfo(path));
            while (directoryStack.Count > 0)
            {
                var dir = directoryStack.Pop();
                try
                {
                    foreach (var i in dir.GetDirectories())
                        directoryStack.Push(i);
                }
                catch (UnauthorizedAccessException)
                {
                    continue; // We don't have access to this directory, so skip it
                }

                foreach (var f in dir.GetDirectories().Where(pattern)) // "pattern" is a function
                    yield return f;
            }
        }

    }
}
