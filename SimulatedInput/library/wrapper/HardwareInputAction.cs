using SimulatedInput.library.native.structs;
using SimulatedInput.library.native.unions;

namespace SimulatedInput.library.wrapper;

public class HardwareInputAction : IInputAction, IInteroperable<HARDWAREINPUT>
{
    public InputType Type => InputType.Hardware;
    public uint Message { get; set; }
    public ushort LowParam { get; set; }
    public ushort HighParam { get; set; }
    
    public HARDWAREINPUT ToInteropStruct()
    {
        return new HARDWAREINPUT
        {
            uMsg = Message,
            wParamL = LowParam,
            wParamH = HighParam
        };
    }
    
    public INPUT ToInputStruct()
    {
        return new INPUT
        {
            type = (int)InputType.Keyboard,
            args = new InputArgumentsUnion(ToInteropStruct())
        };
    }
}