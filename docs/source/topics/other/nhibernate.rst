.. _how-to-use-fubu-mvc-with-nhibernate:

==================================
How To Use FubuMVC with NHibernate
==================================

Setting up NHibernate with FubuMVC is very similar to setting up RavenDB with
FubuMVC(:ref:`how-to-use-fubu-mvc-with-ravendb`). In this section we will go
over some of the same steps to get our application using NHibernate.

Installation
____________
You can find the latest version of NHibernate on the: `NHibernate Download Page`_,
or via the nuget package manager, by running::
  Install-Package NHibernate

.. _`NHibernate Download Page`: http://sourceforge.net/projects/nhibernate/

For help with NHibernate itself, it is recommended that you read the 
`Getting Started Guide`_. You will most likely want to bookmark the:
`NHibernate Reference Documentation`_, as it one of the most comprehensive
documents about NHibernate on the Internet.

  
.. _`Getting Started Guide`: http://nhforge.org/wikis/howtonh/your-first-nhibernate-based-application.aspx
.. _`NHibernate Reference Documentation`: http://www.nhforge.org/doc/nh/en/
  

Bootstraping
------------
To get our application started we to need to setup our bootstrapping code in the
Global.asax.cs file :ref:`setting-up-your-fuburegistry` . For this example we will write this code
directly in the Application_Start() method::

  FubuApplication
    .For<YourRegistry>()
    .StructureMap(() => new Container(x =>
      x.Scan(i =>
      {
        i.TheCallingAssembly();
        PackageRegistry.PackageAssemblies.Each(i.Assembly);
        i.LookForRegistries();
      })))
    .Bootstrap();

In this bootstrap code we are registering all StructureMap
Registries::
        i.LookForRegistries();

This code will call our NHibernateDBRegistry, which we will talk about next.


Setup NHibernate
________________

We will now dive into setting up our NHibernateRegistry code::

    public class NHibernateRegistry : Registry
    {
        public NHibernateRegistry()
        {
            var sessionFactory = new Configuration()
                //set connection properties: ConnectionDriver, ConnectionProvider, etc:
                .SetProperty(NHibernate.Cfg.Environment.ConnectionString, "")
                .BuildSessionFactory();

            For<ISession>().Use(() => sessionFactory.OpenSession());
            For<ISessionFactory>().Singleton().Use(sessionFactory);
        }
    }

Without going into too much detail, we are setting up the connection to our
database via the NHibernate.Cfg.Configuration class and calling the
BuildSessionFactory() on that Configuration to build our SessionFactory
(Singleton). Since we want a new instance of ISession resolved for each web
request we need to call the OpenSession method on the SessionFactory in the form
of a Func<ISession>.

**Q:** Using Fubu, how do make our NHibernate Session scoped one per web request?

**A:** For each new request there will be a new nested container spawned off from the
main Container. One of the traits of these nested containers is that each
instance resolved from them that would normally be Transient (ie, one instance
per resolve) now serves up as if it were a Singleton for the lifetime of that
nested container. Fubu will create the nested container at the beginning of the
request and dispose of the nested container for you at the end of your request.

Example Usage
_____________
To query the Database you can simply inject the ISession into your
Controller/Handler::

  public class GetPostsHandler
  {
    private readonly ISession _session;

    public GetPostsHandler(ISession session)
    {
      _session = session;
    }

    public PostsViewModel Execute(PostsInputModel inputModel)
    {
      //work with _session here.
    }
  }

Unit Of Work
____________
One common practice is to commit your changes at the end of each request.  This
can be easily achieved with FubuMVC Behavior Chains (:ref:`behavior`).  Here is an
example of a Behavior that wraps a new transaction around your InnerBehaviors and
commits that transaction for you when they are done executing::

    public class NHibernateBehavior : IActionBehavior
    {
        private readonly ISession _session;
        public IActionBehavior InsideBehavior { get; set; }

        public NHibernateBehavior(ISession session)
        {
            _session = session;
        }

        public void Invoke()
        {
            using (var tx = _session.BeginTransaction())
            {
                InsideBehavior.Invoke();
                tx.Commit();
            }
        }

        public void InvokePartial()
        {
            //Nothing to do here because we are already inside Invoke()
            InsideBehavior.InvokePartial();
        }
    }

In order to get this behavior into your Behavior Chain you will need to
register it within your FubuRegistry.

One way to do this is by using Policies.WrapBehaviorChainsWith, example::

  public class AFubuRegistry : FubuRegistry {
    public AFubuRegistry()
    {
      Policies.WrapBehaviorChainsWith<NHibernateBehavior>()
    }
  }



