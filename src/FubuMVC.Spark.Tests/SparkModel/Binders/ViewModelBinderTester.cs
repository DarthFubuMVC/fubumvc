
namespace FubuMVC.Spark.Tests.SparkModel.Binders
{
    // TODO : NEXT UP FIXING THIS.

    //[TestFixture]
    //public class ViewModelBinderTester : InteractionContext<ViewModelBinder>
    //{
    //    private BindContext _context;
    //    private SparkItem _sparkItem;

    //    protected override void beforeEach()
    //    {
    //        _sparkItem = new SparkItem("", "", "");
    //        _context = new BindContext
    //        {
    //            ViewModelType = "FubuMVC.Spark.Tests.SparkModel.Binders.Bar"
    //        };
    //    }

    //    [Test]
    //    public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
    //    {
    //        duplicateFullnamePool();
    //        ClassUnderTest.Bind(_sparkItem, _context);
    //        _sparkItem.ViewModelType.ShouldBeNull();
    //    }

    //    // TODO: Test that ambiguity is logged

    //    [Test]
    //    public void if_view_model_type_exists_it_is_assigned_on_item()
    //    {
    //        _context.TypePool = new TypePool(GetType().Assembly);
    //        _context.TypePool.AddType(typeof(Bar));

    //        ClassUnderTest.Bind(_sparkItem, _context);
    //        _sparkItem.ViewModelType.ShouldEqual(typeof(Bar));
    //    }

    //    [Test]
    //    public void if_view_model_type_does_not_exist_nothing_is_assigned()
    //    {
    //        _context.TypePool = new TypePool(GetType().Assembly);

    //        ClassUnderTest.Bind(_sparkItem, _context);
    //        _sparkItem.ViewModelType.ShouldBeNull();
    //    }

    //    private TypePool duplicateFullnamePool()
    //    {
    //        var pool = new TypePool(GetType().Assembly);
    //        pool.AddType(generateType("namespace FubuMVC.Spark.Tests.SparkModel.Binders{public class Bar{}}", "FubuMVC.Spark.Tests.SparkModel.Binders.Bar"));
    //        pool.AddType(typeof(Bar));
    //        return pool;
    //    }
    //    private static Type generateType(string source, string fullName)
    //    {
    //        var parms = new CompilerParameters
    //        {
    //            GenerateExecutable = false,
    //            GenerateInMemory = true,
    //            IncludeDebugInformation = false
    //        };

    //        return CodeDomProvider
    //            .CreateProvider("CSharp")
    //            .CompileAssemblyFromSource(parms, source)
    //            .CompiledAssembly
    //            .GetType(fullName);
    //    } 
    //}

    public class Bar { }
}