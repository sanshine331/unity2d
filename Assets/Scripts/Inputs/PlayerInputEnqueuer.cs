﻿using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(PlayerInputEnqueuer))]
public class PlayerInputEnqueuerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Release actor"))
        {
            var enqueuer = (PlayerInputEnqueuer)target;
            var dequeuer = enqueuer.Actor.GetComponent<AInputDequeuer>();
            PlayerInputEnqueuer.Remove(ref dequeuer);
        }

        if (GUILayout.Button("Control actor"))
        {
            var enqueuer = (PlayerInputEnqueuer)target;
            var dequeuer = enqueuer.Actor.GetComponent<AInputDequeuer>();
            PlayerInputEnqueuer.Add(enqueuer.Actor, ref dequeuer);
        }
    }
}

#endif

public class PlayerInputEnqueuer : AInputEnqueuer
{
    [SerializeField]
    private AActor actor;

    public AActor Actor { get { return actor; } }

    public static PlayerInputEnqueuer Instance
    {
        get
        {
            return FindObjectOfType<PlayerInputEnqueuer>();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        gameObject.isStatic = true;
    }

    protected override void EnqueueInputs()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Enqueue(KeyCode.UpArrow);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Enqueue(KeyCode.DownArrow);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Enqueue(KeyCode.LeftArrow);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Enqueue(KeyCode.RightArrow);
            }
            return;
        }
        Enqueue(KeyCode.None);
        return;
    }

    public static void Add(AActor actor, ref AInputDequeuer dequeuer)
    {
        var instance = Instance as AInputEnqueuer;
        instance.Add(ref instance, ref dequeuer);

        Instance.actor = actor;
    }

    public static void Remove(ref AInputDequeuer dequeuer)
    {
        var instance = Instance as AInputEnqueuer;
        instance.Remove(ref instance, ref dequeuer);

        Instance.actor = null;
    }

    protected override void OnDequeuerDestroyed(IDestroyable destroyedComponent)
    {
        if (!Instance)
        {
            return;
        }

        var instance = Instance as AInputEnqueuer;
        var dequeuer = destroyedComponent as AInputDequeuer;
        instance.Remove(ref instance, ref dequeuer);
    }

    public override void Dispose()
    {
        if (!Instance)
        {
            return;
        }

        base.Dispose();
    }

    protected override void OnInputsDequeued(Vector2 direction)
    {
    }
}