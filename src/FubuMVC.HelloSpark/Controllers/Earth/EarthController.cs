using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core.Continuations;
using FubuMVC.HelloSpark.Controllers.Air;

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
                Thread.Sleep(1000);
                return new EarthViewModelAsync{RawUrl = whereAreWe.RawUrl};
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task RockAsyncNoResult(EarthAsyncNoResultInputModel input)
        {
            return Task.Factory.StartNew(() => Thread.Sleep(1000), 
                TaskCreationOptions.AttachedToParent);
        }

        public Task<FubuContinuation> RockAsyncRedirect()
        {
            return Task<FubuContinuation>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return FubuContinuation.RedirectTo<AirRequest>();
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task<EarthAsyncPartialViewModel> RockPartialAsync(EarthAsyncPartialInputModel input)
        {
            return Task<EarthAsyncPartialViewModel>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return new EarthAsyncPartialViewModel {Message = "Slept for a while and returned partial model"};
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

    public class EarthAsyncNoResultInputModel
    {
        public string RawUrl { get; set; }
    }

    public class EarthAsyncPartialViewModel
    {
        public string Message { get; set; }
    }

    public class EarthAsyncPartialInputModel
    {
    }
}