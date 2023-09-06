namespace SimulatedInput.Core.Wrapper;

public interface IInteroperable<out T>
{
    T ToInteropStruct();
}