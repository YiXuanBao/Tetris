using System;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEntry : MonoBehaviour
{
    private static readonly GameFrameworkLinkedList<GameFrameworkComponent> _components = new GameFrameworkLinkedList<GameFrameworkComponent>();

    internal static void RegisterComponent(GameFrameworkComponent GameFrameworkComponent)
    {
        if (_components == null)
        {
            Debug.LogError("Game Framework component is invalid.");
            return;
        }

        Type type = GameFrameworkComponent.GetType();

        LinkedListNode<GameFrameworkComponent> current = _components.First;
        while (current != null)
        {
            if (current.Value.GetType() == type)
            {
                Debug.LogError("Game Framework component type '{0}' is already exist." + type.FullName);
                return;
            }

            current = current.Next;
        }

        _components.AddLast(GameFrameworkComponent);
    }

    public static T GetGameFrameworkComponent<T>() where T : GameFrameworkComponent
    {
        return GetGameFrameworkComponent(typeof(T)) as T;
    }

    public static GameFrameworkComponent GetGameFrameworkComponent(Type type)
    {
        LinkedListNode<GameFrameworkComponent> current = _components.First;
        while (current != null)
        {
            if (current.Value.GetType() == type)
            {
                return current.Value;
            }

            current = current.Next;
        }

        return null;
    }

    public static void Shutdown()
    {
        Utils.Log("Shutdown Game Framework...");

        _components.Clear();
    }
}
