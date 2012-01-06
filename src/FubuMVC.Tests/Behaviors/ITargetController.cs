using System;
using System.Threading.Tasks;

namespace FubuMVC.Tests.Behaviors
{
    public interface ITargetController
    {
        Output ZeroInOneOut();
        Task<Output> ZeroInOneOutAsync();
        Output OneInOneOut(Input input);
        void ZeroInZeroOut();
        void OneInZeroOut(Input input);
        Task OneInZeroOutAsync(Input input);
    }

    public class Input
    {
    }

    public class Output
    {
    }
}