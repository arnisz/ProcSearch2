using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcSearch2
{
    public class Alias
    {
        private static volatile Alias _instance;
        private readonly static Object lockObject = new object();

         

        private Alias()
        {
        }

        public static Alias getInstace
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new Alias();
                        }
                    }
                }

                return _instance;
            }
        }

        public Dictionary<string,string> Entries;

        public void Load(string AliasFileName)
        {
            char[] Separators = new []{':'};
            StreamReader streamReader = new StreamReader(AliasFileName, Encoding.GetEncoding(codepage: 1252));
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line != null)
                {
                    string[] _vs = line.Split(Separators);
                    string key = _vs[0];
                    StringBuilder s = new StringBuilder();

                    for (int iterator = 1; iterator < _vs.Length; iterator++)
                    {
                        s.Append(_vs[iterator]);
                    }
                    Entries.Add(key,s.ToString());
                }
            }
            streamReader.Close();
        }

    }
}
