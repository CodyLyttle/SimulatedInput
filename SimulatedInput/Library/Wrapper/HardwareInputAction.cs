using SimulatedInput.Library.Native;
using SimulatedInput.Library.Native.Structs;

namespace SimulatedInput.Library.Wrapper;

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
            type = (uint)Type,
            args = new InputArgumentsUnion(ToInteropStruct())
        };
    }
}