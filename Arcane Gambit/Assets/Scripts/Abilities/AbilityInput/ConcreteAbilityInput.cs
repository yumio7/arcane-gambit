using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IAbilityInput<T> : IEnumerable<T>
{
    void LinkSequence(IAbilityInput<T> sequencedInput);
    T GetInput();
    void SetInput(T input);
    void RequestInput();
    void RequestNextNonReadyInput();
    bool IsInputSequenceReady();
    public IAbilityInput<T> NextInput();
    List<T> ToList();
}

public abstract class AbstractAbilityInput<T> : IAbilityInput<T>
{
    public event Action<T> OnInputReceived = delegate { };
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
        OnInputReceived(Input);
    }

    public void RequestInput()
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

    public void RequestNextNonReadyInput()
    {
        if (InputReady)
        {
            RequestInput();
        }
        else
        {
            SequencedInput.RequestNextNonReadyInput();
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

    public List<T> ToList()
    {
        return new List<T>() { Input }.Concat(SequencedInput.ToList()).ToList();
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