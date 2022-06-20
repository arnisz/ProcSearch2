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
        private bool _isDraft,_isSub;
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
                    _isDraft = false;
                } else if (_description.ToUpper().StartsWith("E:"))
                {
                    _isDraft = true;
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
            get => _description;
        }

        public string FileName
        {
            get => _fileName;
        }

        public bool IsDraft
        {
            get => _isDraft;
        }

        public bool IsSub
        {
            get => _isSub;

        }


        public override string ToString()
        {
            return Path + " XX " + Name;
        }


    }
}
