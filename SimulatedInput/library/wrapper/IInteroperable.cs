namespace SimulatedInput.library.wrapper;

public interface IInteroperable<out T>
{
    T ToInteropStruct();
}