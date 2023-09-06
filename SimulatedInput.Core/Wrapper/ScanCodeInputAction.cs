using SimulatedInput.Core.Native;
using SimulatedInput.Core.Native.Structs;

namespace SimulatedInput.Core.Wrapper;

public sealed class ScanCodeInputAction : IInputAction, IInteroperable<KEYBDINPUT>
{
    private static class Flag
    {
        public const uint ExtendedKey = 0x0001;
        public const uint KeyUp = 0x0002;
        public const uint ScanCode = 0x0008;
    }

    public InputType Type => InputType.Keyboard;

    public ushort ScanCode { get; }
    public bool IsExtendedKey { get; }
    public bool IsKeyUp { get; }
    public uint Timestamp { get; set; } = 0;
    public UIntPtr ExtraInfo { get; set; } = UIntPtr.Zero;

    public ScanCodeInputAction(ushort scanCode, bool isKeyUp)
    {
        ScanCode = scanCode;
        IsExtendedKey = false;
        IsKeyUp = isKeyUp;
    }

    public ScanCodeInputAction(byte extendedScanCode, bool isKeyUp)
    {
        const ushort extendedScanCodeLowOrderByte = 0xE0;
        
        ScanCode = (ushort) (extendedScanCodeLowOrderByte << sizeof(byte) | extendedScanCode);
        IsExtendedKey = true;
        IsKeyUp = isKeyUp;
    }

    public KEYBDINPUT ToInteropStruct()
    {
        uint flags = Flag.ScanCode;
        if (IsExtendedKey)
            flags |= Flag.ExtendedKey;
        if (IsKeyUp)
            flags |= Flag.KeyUp;
        
        return new KEYBDINPUT
        {
            wVK = 0,
            wScan = ScanCode,
            dwFlags = flags,
            time = Timestamp,
            dwExtraInfo = ExtraInfo
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