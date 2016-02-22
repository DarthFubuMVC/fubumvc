namespace FubuMVC.Core.Validation
{
	public class ValidationMode
	{
		public static readonly ValidationMode Live = new ValidationMode("live");
		public static readonly ValidationMode Triggered = new ValidationMode("triggered");

		private readonly string _mode;

		public ValidationMode(string mode)
		{
			_mode = mode;
		}

		public string Mode
		{
			get { return _mode; }
		}

		public override string ToString()
		{
			return _mode;
		}

		protected bool Equals(ValidationMode other)
		{
			return string.Equals(_mode, other._mode);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ValidationMode) obj);
		}

		public override int GetHashCode()
		{
			return _mode.GetHashCode();
		}
	}
}