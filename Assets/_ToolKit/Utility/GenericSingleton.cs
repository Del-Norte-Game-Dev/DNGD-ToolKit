using System;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour, IInitializable where T : Component
{
    protected static bool isClosing = false;

    public bool IsReady { get; private set; } = false;
    public Action OnReady;

    protected static T instance;
    public static T Instance {
        get {
            if (!Application.isPlaying || isClosing) return null; // don't auto-create when closing
            if (instance == null) {
                instance = FindAnyObjectByType<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake() {
        Initialize();
        IsReady = true;
        OnReady?.Invoke();
    } 

    public virtual void Initialize(){
        if (!Application.isPlaying || isClosing) return;
        if (instance == null) {
            instance = this as T;
        } else { 
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnApplicationQuit()
    {
        isClosing = true;
    }
}
