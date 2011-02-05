using System;
using System.Security.Principal;

namespace FubuMVC.Core.Security.AntiForgery
{
	public class AntiForgeryData
	{
		public AntiForgeryData() { }

		public AntiForgeryData(AntiForgeryData token) {
            CreationDate = token.CreationDate;
            Salt = token.Salt;
            Username = token.Username;
            Value = token.Value;
        }

		private string _salt;
		public string Salt
		{
			get { return _salt ?? string.Empty; }
			set { _salt = value; }
		}

		private string _value;
		public string Value
		{
			get { return _value ?? string.Empty; }
			set { _value = value; }
		}

		private string _username;
		public string Username
		{
			get { return _username ?? string.Empty; }
			set { _username = value; }
		}

		private DateTime _creationDate = DateTime.UtcNow;
		public DateTime CreationDate
		{
			get { return _creationDate; }
			set { _creationDate = value; }
		}


		public void SetUser(IPrincipal user)
		{
			Username = GetUsername(user);
		}

		public static string GetUsername(IPrincipal user)
		{
			
			if (user != null)
			{
				IIdentity identity = user.Identity;
				if (identity != null && identity.IsAuthenticated)
				{
				return identity.Name;
				}
			}

			return String.Empty;
		}
	}
}