using System.Web;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SelfHost.Testing
{
    [TestFixture]
    public class debugging
    {
        [Test]
        public void decode()
        {
            var cookie =
            "ga7M%2bfHYF%2bYCbREh00%2b9jFcq6PQtRsmxBAlyA7tXb6l69uulrFw5kZS%2bNay8a9ElijomPLTexlCsx%2fqwAzZgOcGsw8KNxSm0eKJ1xhIWvIpwRitA425O%2fVnKrKOpUdZMu3EVyaRWAbC5hsVpAEjgO%2frztAf0n6U6Orj3NAY%2bw2smvjLJTu5nOjJNj8dR9hroOvOOZd%2bbaVX5BqXnqBjY%2fu%2bOvn3ajTZyeP6QYIGvXUFXbz8I%2bp8pmI8SmHzlqFb8FLNEfwIvurVqg92C8G1RwK6FI6M8nevTtFNpNHVrt%2fNe%2fYhX6IwC%2bNZUYODMra6Z";

            var ticket =
                "ga7M+fHYF+YCbREh00+9jFcq6PQtRsmxBAlyA7tXb6l69uulrFw5kZS+Nay8a9ElijomPLTexlCsx/qwAzZgOcGsw8KNxSm0eKJ1xhIWvIpwRitA425O/VnKrKOpUdZMu3EVyaRWAbC5hsVpAEjgO/rztAf0n6U6Orj3NAY+w2smvjLJTu5nOjJNj8dR9hroOvOOZd+baVX5BqXnqBjY/u+Ovn3ajTZyeP6QYIGvXUFXbz8I+p8pmI8SmHzlqFb8FLNEfwIvurVqg92C8G1RwK6FI6M8nevTtFNpNHVrt/Ne/YhX6IwC+NZUYODMra6Z";



            HttpUtility.UrlDecode(cookie).ShouldEqual(ticket);


        }

        


    }
}