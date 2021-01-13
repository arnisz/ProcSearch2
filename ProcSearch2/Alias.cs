using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace ProcSearch2
{
    public class Alias
    {
        private static volatile Alias _instance;
        private Dictionary<string, string> _Entries = new Dictionary<string, string>(1024);
        private readonly static Object LockObject = new object();
        private bool _aliasReadyToUse;
         

        private Alias()
        {
        }

        public static Alias GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Alias();
                            _instance._aliasReadyToUse = false;
                        }
                    }
                }

                return _instance;
            }
        }

        public Dictionary<string,string> Entries=>_Entries;

        public bool AliasReadyToUse => _aliasReadyToUse;

        public void Load(string aliasFileName)
        {
            char[] separators = new []{':'};
            try
            {
                StreamReader streamReader = new StreamReader(aliasFileName, Encoding.GetEncoding(codepage: 1252));
                _aliasReadyToUse = true;
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line != null)
                    {
                        string[] vs = line.Split(separators);
                        string key = vs[0];
                        StringBuilder s = new StringBuilder();

                        for (int iterator = 1; iterator < vs.Length; iterator++)
                        {
                            s.Append(vs[iterator]);
                        }
                        _Entries.Add(key, s.ToString());
                    }
                }
                streamReader.Close();
            }
            catch (FileNotFoundException)
            {
                _aliasReadyToUse = false;
            }
        }

    }
}
