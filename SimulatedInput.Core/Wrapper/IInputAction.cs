using SimulatedInput.Core.Native.Structs;

namespace SimulatedInput.Core.Wrapper;

public interface IInputAction
{
    InputType Type { get; }
    INPUT ToInputStruct();
}