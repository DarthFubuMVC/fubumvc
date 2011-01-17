using System;

namespace FubuMVC.Tests.Behaviors
{
    public interface ITargetController
    {
        Output ZeroInOneOut();
        Output OneInOneOut(Input input);
        void ZeroInZeroOut();
        void OneInZeroOut(Input input);
    }

    public class Input
    {
    }

    public class Output
    {
    }
}