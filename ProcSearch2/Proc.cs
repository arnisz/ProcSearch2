using System.Dynamic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ProcSearch2
{
    public class Proc
    {
        public Proc()
        {
        }

        private string _Name,_Description,_FileName;
        private bool _IsDrwaft,_IsSub;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                string[] v = Regex.Split(_Name, "   *");
                _FileName = v[0];
                _Description = v[1].ToUpper();
                ///
                /// Hard Kodierte Herstellerbezeichnungen und Aliase
                ///
                
                if (_Description.ToUpper().StartsWith("F:"))
                {
                    _IsDrwaft = false;
                } else if (_Description.ToUpper().StartsWith("E:"))
                {
                    _IsDrwaft = true;
                }

                if (_Description.ToUpper().StartsWith("SUB"))
                {
                    _IsSub = true;
                }
                else
                {
                    _IsSub = false;
                }
            }
        }


        public string Path
        {
            get;set;
        }

        public string Description
        {
            get
            {
                return _Description;
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
        }

        public bool IsDraft
        {
            get
            {
                return _IsDrwaft;
            }
        }

        public bool IsSub
        {
            get
            {
                return _IsSub;
            }
        }


        public override string ToString()
        {
            return Path + " XX " + Name;
        }


    }
}
