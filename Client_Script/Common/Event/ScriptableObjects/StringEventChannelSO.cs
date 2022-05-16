using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Int Event Channel")]
public class StringEventChannelSO : EventChannelSO
{
    public UnityAction<string> OnEventRaised;
	
    public void RaiseEvent(string value)
    {
            OnEventRaised?.Invoke(value);
    }
}

