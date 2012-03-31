.. _how-to-use-fubu-mvc-with-ravendb

===============================
How To Use FubuMVC with RavenDB.
===============================

This section is a brief introduction of how to setup your FubuMVC application
with RavedDB.



Installation
____________
To get started, you will need a FubuMVC project :doc:`getting-started`, add
RavenDB to your references.  You can download RavenDB from: 
`RavenDB Download Page`_, or simply install it via the nuget package manager::
    Install-Package RavenDB

.. _`RavenDB Download Page`: http://ravendb.net/download



Bootstraping
------------
In order to get our application working we are going to need to setup our
bootstrapping code in the Global.asax.cs file :doc:`bootstrap`. For this example
we will write this code directly in the Application_Start() method::
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

This code will call our RavenDBRegistry, which we will talk about later.



Setup Connection
________________
There are a few ways to connect to your RavenDB Database (`Connecting to a RavenDB data store`_),
in this example will use the connection string, add this code to your
Web.config::

  <connectionStrings>
    <add name="MyConnection" connectionString="Url=http://localhost:8080"/>
  </connectionStrings>

.. _`Connecting to a RavenDB data store`: http://ravendb.net/docs/client-api/connecting-to-a-ravendb-datastore

We will provide RavenDB with this connection string name in our registry code.



Setup RavenDB
_____________
The setup code for RavenDB itself is quite simple::

    public class RavenDBRegistry : Registry
    {
        public RavenDBRegistry()
        {
            var documentStore = new DocumentStore
            {
                ConnectionStringName = "MyConnection"
            }.Initialize();


            Func<IDocumentSession> getSession = documentStore.OpenSession;

            For<IDocumentSession>().Use(getSession);
            For<IDocumentStore>().Singleton().Use(documentStore);
        }
    }

We need to initialize the DocumentStore as a singleton with our connection string name.

Since we want a new IDocumentSession resolved for each web request we register
the document stores OpenSession method as our IDocumentSession in the form of a
Func<IDocumentSession>.


Example Usage
_____________
To query the DocumentDatabase you can simply inject the IDocumentSession into
your Controller/Handler::

  public class GetHandler
  {
    private readonly IDocumentSession _session;

    public GetHandler(IDocumentSession session)
    {
      _session = session;
    }

    public dynamic Execute(InputModel inputModel)
    {
      //use _session to pull in data
    }
  }


Unit Of Work
____________
One common practice is to save or update your changes at the end of each
request.  This can be easily achieved with FubuMVC Behavior Chains
:doc:`behavior-chains`.  Here is an example of a Behavior that calls SaveChanges
for you::

    public class RavenDbBehavior : IActionBehavior
    {
        private readonly IDocumentSession _session;
        public IActionBehavior InsideBehavior { get; set; }

        public RavenDbBehavior(IDocumentSession session)
        {
            _session = session;
        }

        public void Invoke()
        {
            using(_session)
            {
                InsideBehavior.Invoke();
                _session.SaveChanges();
            }
        }

        public void InvokePartial()
        {
            InsideBehavior.InvokePartial();
        }
    }

In order to get this behavior into your Behavior Chain you will need to
register it within your FubuRegistry.

One way to do this is by using Policies.WrapBehaviorChainsWith, example::

  public class AFubuRegistry : FubuRegistry {
    public BlogRegistry()
    {
      Policies.WrapBehaviorChainsWith<RavenDbBehavior>()
    }
  }



