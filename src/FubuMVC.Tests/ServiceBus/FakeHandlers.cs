using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace FubuMVC.Tests.ServiceBus
{

    public interface ITargetHandler
    {
        string Message { get; set; }
        Output OneInOneOut(Input input);
        void OneInZeroOut(Input input);
        object OneInManyOut(Input input);
        void ZeroInZeroOut();

        void ManyIn(Input i1, Input i2);

        IImmediateContinuation ReturnsInterface(Input input);
        bool ReturnsValueType(Input input);
    }

    public class Input
    {
    }

    public class DifferentInput
    {
        
    }

    public class SpecialInput : Input
    {

    }

    public class Output
    {
    }
    
    public interface IInput { }
    public abstract class InputBase { }
    public class Input1 : InputBase, IInput { }
    public class Input2 { }

    public class SomeHandler
    {
        public void Interface(IInput input) { }
        public void BaseClass(InputBase input) { }
    }
}