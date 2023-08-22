using SimulatedInput.library.native.structs;

namespace SimulatedInput.library.wrapper;

    
// TODO: Implement UnicodeInput.
public class UnicodeInputAction : IInputAction, IInteroperable<KEYBDINPUT>
{
    public InputType Type { get; }

    public KEYBDINPUT ToInteropStruct()
    {
        throw new System.NotImplementedException();
    }
    
    public INPUT ToInputStruct()
    {
        throw new System.NotImplementedException();
    }
}