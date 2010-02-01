namespace FubuMVC.HelloSpark.Controllers
{
    public class FireController
    {
        public FireViewModel Create(FireInputModel model)
        {
            return model.GotALight()
                       ? new FireViewModel {Text = "Light 'em up!"}
                       : new FireViewModel {Text = "Light 'em up anyway!"};
        }
    }

    public class FireInputModel
    {
        public bool GotALight()
        {
            return true;
        }
    }

    public class FireViewModel
    {
        public string Text { get; set; }
    }
}