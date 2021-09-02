using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    public List<IObserver> observers = new List<IObserver>();

    public virtual void Notify()
    {
        for (int i = 0; i < observers.Count; i++)
        {
            observers[i].ObserverUpdate();
        }
    }

    public virtual void Attach(IObserver observer)
    {
        observers.Add(observer);
    }
}