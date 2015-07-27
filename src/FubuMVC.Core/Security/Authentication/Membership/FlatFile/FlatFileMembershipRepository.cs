using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using HtmlTags;

namespace FubuMVC.Core.Security.Authentication.Membership.FlatFile
{
    public class FlatFileMembershipRepository : IMembershipRepository
    {
        private readonly string _passwordConfigFile;
        private readonly Lazy<IList<UserInfo>> _users;

        // TODO -- if anyone starts to care, we'll make the location be variable
        public FlatFileMembershipRepository(IFubuApplicationFiles files)
        {
            _passwordConfigFile = files.RootPath.AppendPath("fubu.auth.config");
            _users = new Lazy<IList<UserInfo>>(() =>
            {
                return ReadFromFile(_passwordConfigFile);
            });
        }

        public string PasswordConfigFile
        {
            get { return _passwordConfigFile; }
        }

        public static IList<UserInfo> ReadFromFile(string location)
        {
            var list = new List<UserInfo>();

            new FileSystem().ReadTextFile(location, line => {
                if (line.IsEmpty()) return;

                var user = JsonUtil.Get<UserInfo>(line);
                list.Add(user);
            });


            return list;
        } 

        public void Write(IEnumerable<UserInfo> users)
        {
            var existing = _users.Value;
            existing.RemoveAll(x => users.Contains(x));
            existing.AddRange(users);

            new FileSystem().WriteToFlatFile(_passwordConfigFile, x => {
                existing.Each(u => {
                    var json = JsonUtil.ToJson(u);
                    x.WriteLine(json);
                });
            });
        }

        public bool MatchesCredentials(LoginRequest request)
        {
            return _users.Value.Any(x => x.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase) && x.Password == request.Password);
        }

        public IUserInfo FindByName(string username)
        {
            return _users.Value.FirstOrDefault(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}