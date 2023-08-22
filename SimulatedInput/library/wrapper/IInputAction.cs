using SimulatedInput.library.native.structs;

namespace SimulatedInput.library.wrapper;

public interface IInputAction
{
    InputType Type { get; }
    INPUT ToInputStruct();
}