using FubuMVC.Core.View;
using FubuMVC.Core.View.WebForms;

namespace FubuMVC.HelloWorld.Controllers.OutputModels
{
    public class OutputModelController
    {
        public OutputModelWithSettings Index(IndexInputModel model)
        {
            return new OutputModelWithSettings();
        }
    }

    public class IndexInputModel
    {
        
    }

    public class OutputModelSettings
    {
        public string FavoriteAnimalName { get; set; }
        public string BestSimpsonsCharacter { get; set; }
    }

    public class OutputModelWithSettings
    {
        public string SomeOtherProperty { get; set; }
        public OutputModelSettings Settings { get; set; }
    }

    public class Index : FubuPage<OutputModelWithSettings>
    {
    }
}