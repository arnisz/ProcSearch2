using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace ProcSearch2
{
    [Serializable]
    public sealed class ProcDb
    {
        public static event EventHandler RrChanged;
        private readonly List<DirectoryInfo> _directories= new List<DirectoryInfo>();
        private List<Proc> _procedureList = new List<Proc>();
        private readonly static object LockObject = new object();
        private static volatile ProcDb _instance;
        private long _serial;

        private ProcDb()
        {
        }

        public static ProcDb GetInstace
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ProcDb();
                            _instance._serial = 0;
                        }
                    }
                }

                return _instance;
            }
        }

        public void SaveToFile(string fileName)
        {
            using (Stream stream = File.Open(fileName,FileMode.Create,FileAccess.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Proc>));
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default);
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer,_procedureList);
                writer.Close();
            }
        }

        public void ReadFromFile(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Proc>));
            using (Stream stream = File.Open(fileName, FileMode.Open,FileAccess.Read))
            {
                _procedureList = (List<Proc>) serializer.Deserialize(stream);
            }

            var m = new DirectoryChangedEventArgs()
            {
                Counter = _procedureList.Count
            };

            if (RrChanged != null) RrChanged(null, m);
        }

        public void Clear()
        {
            _directories.Clear();
        }


        public void AddDirectory(DirectoryInfo directoryInfo)
        {
            _directories.Add(directoryInfo);
            ReadAll(directoryInfo);
            var m = new DirectoryChangedEventArgs()
            {
                Counter = this.Count
            };
            if (RrChanged != null) RrChanged(null, m);
            ReadAll(directoryInfo);
        }

        public List<DirectoryInfo> Directories => _directories;
        public List<Proc> ProcedureList => _procedureList;

        public long Count => _procedureList.Count;

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
                        if (Alias.GetInstance.AliasReadyToUse)
                        {
                            while (r.EndOfStream == false)
                            {
                                try
                                {
                                    Proc p = new Proc(Alias.GetInstance)
                                    {
                                        Path = directoryInfo.FullName,
                                        Name = r.ReadLine(),
                                        Serial = _serial
                                    };
                                    if (p.Name != null && p.Name.Length > 3 && p.Name.Length < 90)
                                    {
                                        _procedureList.Add(p);
                                        _serial++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e+ @" could'nt read th Line, ignore!");
                                }


                            }
                        }
                        else
                        {
                            while (r.EndOfStream == false)
                            {
                                try
                                {
                                    Proc p = new Proc()
                                    {
                                        Path = directoryInfo.FullName,
                                        Name = r.ReadLine(),
                                        Serial = _serial
                                    };
                                    if (p.Name != null && p.Name.Length > 3 && p.Name.Length < 90)
                                    {
                                        _procedureList.Add(p);
                                        _serial++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e + @" could'nt read th Line, ignore!");
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
