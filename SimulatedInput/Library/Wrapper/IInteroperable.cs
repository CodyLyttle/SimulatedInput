namespace SimulatedInput.Library.Wrapper;

public interface IInteroperable<out T>
{
    T ToInteropStruct();
}