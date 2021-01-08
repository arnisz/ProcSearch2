using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace ProcSearch2
{

    public sealed class ProcDB
    {
        public static event EventHandler RRChanged;
        private readonly List<DirectoryInfo> _directories= new List<DirectoryInfo>();
        private readonly List<Proc> _procedureList = new List<Proc>(1024);
        private readonly static object lockObject = new object();
        private static volatile ProcDB _instance;

        private ProcDB()
        {
        }

        public static ProcDB GetInstace
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ProcDB();
                        }
                    }
                }

                return _instance;
            }
        }

 


        public void AddDirectory(DirectoryInfo directoryInfo)
        {
            _directories.Add(directoryInfo);
            DirectoryChangedEventArgs m = new DirectoryChangedEventArgs()
            {
                Counter = this.Count
            };
            if (RRChanged != null) RRChanged(null, m);
            ReadAll(directoryInfo);
        }

        public List<DirectoryInfo> Directories => _directories;
        public List<Proc> ProcedureList => _procedureList;

        public int Count => _directories.Count;

        private void ReadAll(DirectoryInfo directoryInfo)
        {
            //  FileStream datei = new FileStream(pathname, FileMode.Open);
            string pathname = directoryInfo.FullName+"\\proc.dir";
            if (File.Exists(pathname))
            {
                using (StreamReader r = new StreamReader(pathname,Encoding.GetEncoding(codepage:1252)))
                {
                    lock (r)
                    {
                        while (r.EndOfStream == false)
                        {
                            Proc p = new Proc
                            {
                                Path = directoryInfo.FullName,
                                Name = r.ReadLine()
                            };
                            _procedureList.Add(p);
                        }
                        r.Close();
                    }
                }
            }
        }
    }
}
