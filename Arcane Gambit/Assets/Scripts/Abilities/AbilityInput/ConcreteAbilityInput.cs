using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IAbilityInput<T> : IEnumerable<T>
{
    public event Action OnInputReceived;
    void LinkSequence(IAbilityInput<T> sequencedInput);
    T GetInput();
    void SetInput(T input);
    void RequestInput();
    void RequestNextNonReadyInput();
    bool IsInputSequenceReady();
    public IAbilityInput<T> NextInput();
    public void Cleanup();
    List<T> ToList();

    public void SubscribeToSequence(Action action);
    public void UnsubscribeFromSequence(Action action);
}

public abstract class AbstractAbilityInput<T> : IAbilityInput<T>
{
    public event Action OnInputReceived = delegate {};
    public IAbilityInput<T> SequencedInput { get; protected set; }
    protected T Input = default;
    protected IAbilityInputRequest<T> InputRequest = null;
    protected bool InputReady = false;
    
    public AbstractAbilityInput() { }

    public AbstractAbilityInput(T input, IAbilityInput<T> sequencedInput = null)
    {
        Input = input;
        SequencedInput = sequencedInput;
    }
    
    public AbstractAbilityInput(T input, IAbilityInputRequest<T> inputRequest, IAbilityInput<T> sequencedInput = null)
    {
        Input = input;
        SequencedInput = sequencedInput;
        InputRequest = inputRequest;
    }


    public void LinkSequence(IAbilityInput<T> sequencedInput)
    {
        SequencedInput = sequencedInput;
    }

    public T GetInput()
    {
        return Input;
    }

    public void SetInput(T input)
    {
        Input = input;
        InputReady = true;
        RequestNextNonReadyInput();
    }

    public virtual void RequestInput()
    {
        if(InputRequest != null) 
        {
            InputRequest.Request(SetInput);
        } 
        else 
        {
            SetInput(Input);
        }
    }

    public virtual void RequestNextNonReadyInput()
    {
        if (InputReady == false)
        {
            RequestInput();
        }
        else if(SequencedInput != null)
        {
            SequencedInput.RequestNextNonReadyInput();
        } else
        {
            OnInputReceived?.Invoke();
        }
    }

    public bool IsInputSequenceReady()
    {
        return (SequencedInput != null) ? InputReady || SequencedInput.IsInputSequenceReady() : InputReady;
    }

    public IAbilityInput<T> NextInput()
    {
        return SequencedInput;
    }

    public void Cleanup()
    {
        InputRequest?.Cleanup();
        SequencedInput?.Cleanup();
        InputReady = false;
    }

    public List<T> ToList()
    {
        return new List<T>() { Input }.Concat(SequencedInput.ToList()).ToList();
    }

    public void SubscribeToSequence(Action action)
    {
        Debug.Log("subscribe");
        OnInputReceived += action;
        SequencedInput?.SubscribeToSequence(action);
    }

    public void UnsubscribeFromSequence(Action action)
    {
        OnInputReceived -= action;
        SequencedInput?.UnsubscribeFromSequence(action);
    }

    public IEnumerator<T> GetEnumerator()
    {
        yield return Input;
        if (SequencedInput != null)
        {
            foreach (var sequenced in SequencedInput)
            {
                yield return sequenced;
            }
        } 
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class ConcreteAbilityInput<T> : AbstractAbilityInput<T>
{
    public ConcreteAbilityInput(T input, IAbilityInput<T> sequencedInput = null) : base(input, sequencedInput)
    {
        
    }

    /*public override void RequestInput()
    {
        if (InputRequest != null)
        {
            InputRequest.Request(SetInput);
        }
        else
        {
            SetInput(Input);
        }
    }

    public override void RequestNextNonReadyInput()
    {
        if (InputReady == false)
        {
            RequestInput();
        }
        else
        {
            SequencedInput?.RequestNextNonReadyInput();
        }
    }*/
}

public class AbilityInput<T> : AbstractAbilityInput<T>
{
    public AbilityInput(){ }
    public AbilityInput(T input, IAbilityInput<T> sequencedInput = null) : base(input, sequencedInput) { }
    
    public AbilityInput(T input, IAbilityInputRequest<T> inputRequest, IAbilityInput<T> sequencedInput = null) : base(input, sequencedInput)
    {
        InputRequest = inputRequest;
    }
}

public class ListAbilityInput<T> : AbstractAbilityInput<T>
{
    
}