using System;
using System.Linq;
using FubuCore;

namespace Fubu.Running
{
    public class FubuRegistryChooser
    {
        private readonly IFubuRegistryFinder _finder;
        private readonly IMessaging _messaging;

        public FubuRegistryChooser(IFubuRegistryFinder finder, IMessaging messaging)
        {
            _finder = finder;
            _messaging = messaging;
        }

        public void Find(StartApplication message, Action<Type> onFound)
        {
            var applicationTypes = _finder.Find();

            Type applicationType = null;


            if (!applicationTypes.Any())
            {
                _messaging.Send(new InvalidApplication
                {
                    Message = "Could not find any instance of FubuRegistry in any assembly in this directory"
                });

                return;
            }

            if (message.ApplicationName.IsNotEmpty())
            {
                applicationType = applicationTypes.FirstOrDefault(x => x.Name.EqualsIgnoreCase(message.ApplicationName));


                if (applicationType == null)
                {
                    _messaging.Send(new InvalidApplication
                    {
                        Message = "Could not find an application named '{0}'".ToFormat(message.ApplicationName),
                        Applications = applicationTypes.Select(x => x.Name).ToArray()
                    });

                    return;
                }
            }

            if (applicationType == null && applicationTypes.Count() == 1)
            {
                applicationType = applicationTypes.Single();
            }

            if (applicationType == null)
            {
                _messaging.Send(new InvalidApplication
                {
                    Applications = applicationTypes.Select(x => x.Name).ToArray(),
                    Message = "Unable to determine the FubuMVC Application"
                });
            }
            else
            {
                onFound(applicationType);
            }
        }
    }
}