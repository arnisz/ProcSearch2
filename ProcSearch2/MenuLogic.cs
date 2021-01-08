using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcSearch2
{

    static class MenuLogic
    {

        public async static Task FileOpen()
        {

            string path = "";  //Specify initial or root path
            IEnumerable<DirectoryInfo> r; // placeholder for TraverseRtee the list of paths
            List<ProcDB> rr = new List<ProcDB>(10); //placeholder
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                path = fb.SelectedPath;
            }



            Action getDir = () =>
            {
                ProcDB instance=ProcDB.GetInstace;

                r = GetDirRecursive(path);

                foreach (DirectoryInfo p in r)
                { 
                    instance.AddDirectory(p);
                }
            };
            Task t = new Task(getDir);

            t.Start();
        }

        public static IEnumerable<DirectoryInfo> GetDirRecursive(string path)
        {

            var directoryStack = new Stack<DirectoryInfo>();
            var returnStack = new Stack<DirectoryInfo>();
            directoryStack.Push(new DirectoryInfo(path));   //Push Root Directory

            while (directoryStack.Count > 0)
            {
                var dir = directoryStack.Pop();
                returnStack.Push(dir);
                try
                {
                    foreach (var i in dir.GetDirectories())
                    {
                        directoryStack.Push(i);
                        returnStack.Push(i);
                    }

                }
                catch (UnauthorizedAccessException)
                {

                }
            }
            return returnStack;

        }
    }


}
