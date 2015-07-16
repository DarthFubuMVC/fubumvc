using System.Diagnostics;

namespace Bottles
{
    [DebuggerDisplay("_value")]
    public class CompileTarget
    {
        private string _value;

        private CompileTarget(string value)
        {
            _value = value;
        }

        public CompileTarget Build(string value)
        {
            return new CompileTarget(value);
        }

        public static readonly CompileTarget Release = new CompileTarget("Release");
        public static readonly CompileTarget Debug = new CompileTarget("Debug");


        public override string ToString()
        {
            return _value;
        }

        public bool Equals(CompileTarget other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._value, _value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CompileTarget)) return false;
            return Equals((CompileTarget) obj);
        }

        public override int GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }
    }
}