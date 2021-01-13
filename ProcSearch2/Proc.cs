using System.Text.RegularExpressions;

namespace ProcSearch2
{
    public class Proc
    {
        private Alias _a;
        public Proc()
        {
        }

        public Proc(Alias a)
        {
            _a = a;
        }

        private string _name,_description,_fileName;
        private bool _isDrwaft,_isSub;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                string[] v = Regex.Split(_name, "   *");
                _fileName = v[0];
                _description = v[1].ToUpper();

                if (_a != null)
                {
                    foreach (var de in _a.Entries)
                    {
                        if (_description.Contains(de.Key.ToUpper()))
                        {
                            _description += ", "+de.Value.ToUpper();
                        }
                    }
                }
                if (_description.ToUpper().StartsWith("F:"))
                {
                    _isDrwaft = false;
                } else if (_description.ToUpper().StartsWith("E:"))
                {
                    _isDrwaft = true;
                }

                if (_description.ToUpper().StartsWith("SUB"))
                {
                    _isSub = true;
                }
                else
                {
                    _isSub = false;
                }
            }
        }


        public string Path
        {
            get;set;
        }

        public long Serial
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

        public bool IsDraft
        {
            get
            {
                return _isDrwaft;
            }
        }

        public bool IsSub
        {
            get
            {
                return _isSub;
            }
        }


        public override string ToString()
        {
            return Path + " XX " + Name;
        }


    }
}
