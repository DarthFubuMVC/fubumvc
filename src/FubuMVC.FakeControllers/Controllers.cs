namespace FubuMVC.FakeControllers
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
        public void Go()
        {
        }

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
        public void Go()
        {
        }

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
        public void Go()
        {
        }

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