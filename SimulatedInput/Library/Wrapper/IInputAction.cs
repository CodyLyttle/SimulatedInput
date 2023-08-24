using SimulatedInput.Library.Native.Structs;

namespace SimulatedInput.Library.Wrapper;

public interface IInputAction
{
    InputType Type { get; }
    INPUT ToInputStruct();
}