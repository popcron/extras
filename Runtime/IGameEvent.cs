using Popcron.Extras;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public interface IGameEvent
{

}

public static class PopEvents
{
    private static List<Subscriber> subscribers = null;
    private static object[] parameters = new object[] { null };
    public delegate void OnEventDelegate(ref IGameEvent gameEvent);

    /// <summary>
    /// Happens when any event is dispatched.
    /// </summary>
    public static OnEventDelegate OnEvent { get; set; }

    /// <summary>
    /// Subscribes to a specific event of this kind.
    /// </summary>
    public static void Subscribe<T>(Action<T> callback) where T : IGameEvent
    {
        if (subscribers is null)
        {
            FindEventListenerMethods();
        }

        Subscriber subscriber = new Subscriber(callback.Method, callback.Target, typeof(T));
        subscribers.Add(subscriber);
    }

    /// <summary>
    /// Dispatch this specific event out.
    /// </summary>
    public static void Dispatch(this IGameEvent gameEvent)
    {
        //dispatch to mono behaviours
        int i = 0;
        List<PopBehaviour> all = PopBehaviour.All;
        while (i < all.Count)
        {
            try
            {
                all[i].OnEvent(ref gameEvent);
            }
            catch (Exception e)
            {
                HandleException(e);
            }

            i++;
        }

        //dispatch to static delegate
        try
        {
            OnEvent?.Invoke(ref gameEvent);
        }
        catch (Exception e)
        {
            HandleException(e);
        }

        //dispatch to methods with attributes
        if (subscribers is null)
        {
            FindEventListenerMethods();
        }

        i = 0;
        int count = subscribers.Count;
        parameters[0] = gameEvent;
        Type gameEventType = gameEvent.GetType();
        while (i < count)
        {
            Subscriber subscriber = subscribers[i];
            if (subscriber.eventType == gameEventType)
            {
                subscriber.method.Invoke(subscriber.target, parameters);
            }

            i++;
        }
    }

    private static void FindEventListenerMethods()
    {
        const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        subscribers = new List<Subscriber>();
        Type gameEventType = typeof(IGameEvent);
        foreach (PopType type in PopType.All)
        {
            MethodInfo[] methods = type.GetMethods(Flags);
            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] methodParameters = method.GetParameters();
                if (methodParameters.Length == 1)
                {
                    if (method.GetCustomAttribute<PopEventListenerAttribute>() != null)
                    {
                        Type parameterType = methodParameters[0].ParameterType;
                        if (parameterType.IsByRef)
                        {
                            parameterType = parameterType.GetElementType();
                        }

                        if (gameEventType.IsAssignableFrom(parameterType))
                        {
                            Subscriber subscriber = new Subscriber(method, null, parameterType);
                            subscribers.Add(subscriber);
                        }
                    }
                }
            }
        }
    }

    private static void HandleException(Exception e)
    {
        Debug.LogError(e);
    }

    public class Subscriber
    {
        public MethodInfo method;
        public object target;
        public Type eventType;

        public Subscriber(MethodInfo method, object target, Type eventType)
        {
            this.method = method;
            this.target = target;
            this.eventType = eventType;
        }
    }
}