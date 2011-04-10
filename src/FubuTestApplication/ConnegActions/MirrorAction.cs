using FubuCore;

namespace FubuTestApplication.ConnegActions
{
    public class Input
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Output
    {
        public string FullName { get; set; }
    }

    public class MirrorAction
    {
        public Output Return(Input input)
        {
            return new Output(){
                FullName = "{0} {1}".ToFormat(input.FirstName, input.LastName)
            };
        }    
    
        public Output BuckRogers()
        {
            return new Output(){FullName = "Buck Rogers"};
        }
    }
}