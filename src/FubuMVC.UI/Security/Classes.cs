using System;

namespace FubuMVC.UI.Security
{
    public class AccessRight
    {
        private readonly bool _read;
        private readonly bool _write;
        private readonly int _precedence;

        private AccessRight(bool read, bool write, int precedence)
        {
            _read = read;
            _write = write;
            _precedence = precedence;
        }

        public int Precedence
        {
            get { return _precedence; }
        }

        public bool Read
        {
            get { return _read; }
        }

        public bool Write
        {
            get { return _write; }
        }

        public static readonly AccessRight All = new AccessRight(true, true, 1);
        public static readonly AccessRight ReadOnly = new AccessRight(true, false, 2);
        public static readonly AccessRight None = new AccessRight(false, false, 3);

        public static AccessRight operator +(AccessRight first, AccessRight second)
        {
            return first.Precedence <= second.Precedence ? first : second;
        }
    }
}