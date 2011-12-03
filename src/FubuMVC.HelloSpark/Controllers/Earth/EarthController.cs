using System.Threading;
using System.Threading.Tasks;

namespace FubuMVC.HelloSpark.Controllers.Earth
{
    public class EarthController
    {
        public EarthViewModel Rock(EarthViewModel whereAreWe)
        {
            return whereAreWe;
        }

        public Task<EarthViewModelAsync> RockAsync(EarthAsyncInputModel whereAreWe)
        {
            return Task<EarthViewModelAsync>.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                return new EarthViewModelAsync{RawUrl = whereAreWe.RawUrl};
            }, TaskCreationOptions.AttachedToParent);
        }
    }

    public class EarthViewModel
    {
        public string RawUrl { get; set; }
    }

    public class EarthViewModelAsync
    {
        public string RawUrl { get; set; }
    }

    public class EarthAsyncInputModel
    {
        public string RawUrl { get; set; }
    }
}