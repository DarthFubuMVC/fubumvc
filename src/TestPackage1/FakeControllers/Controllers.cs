namespace TestPackage1.FakeControllers
{
    public class ClassInAnotherAssembly
    {
    }

    public class SimpleInputModel
    {
    }

    public class SimpleOutputModel
    {
    }

    public class OneController
    {

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }
    }

    public class TwoController
    {

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }
    }

    public class ThreeController
    {

        public SimpleOutputModel Report()
        {
            return new SimpleOutputModel();
        }

        public SimpleOutputModel Query(SimpleInputModel model)
        {
            return new SimpleOutputModel();
        }
    }
}