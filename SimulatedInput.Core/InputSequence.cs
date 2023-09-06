using System.Collections;
using SimulatedInput.Core.Wrapper;

namespace SimulatedInput.Core;

public class InputSequence : IEnumerable<IInputAction>
{
    private readonly List<IInputAction> _actions = new();

    public int Length => _actions.Count;
    
    public InputSequence Add(IInputAction action)
    {
        _actions.Add(action);
        return this;
    }
    
    public InputSequence Add(IEnumerable<IInputAction> actions)
    {
        foreach (IInputAction action in actions)
        {
            _actions.Add(action);
        }
        return this;
    }

    public void Clear()
    {
        _actions.Clear();
    }
    

    public IEnumerator<IInputAction> GetEnumerator()
    {
        return _actions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IInputAction[] ToArray()
    {
        return _actions.ToArray();
    }
}