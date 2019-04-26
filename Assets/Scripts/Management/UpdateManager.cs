using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour, IUpdateManager
{
    readonly IList<IUpdatable> _customUpdatables = new List<IUpdatable>();
    bool IsOpenUpdate = false;

    public void AddUpdatable(IUpdatable updatable)
    {
        _customUpdatables.Add(updatable);
    }

    public void RemoveUpdatable(IUpdatable updatable)
    {
        _customUpdatables.Remove(updatable);
    }

    public void Update()
    {
        if (IsOpenUpdate == false)
            return;
        for (int i = 0; i < _customUpdatables.Count; i++)
        {
            _customUpdatables[i].CustomUpdate();
        }
    }

    public void Start()
    {
        IsOpenUpdate = true;
    }

    public void Stop()
    {
        IsOpenUpdate = false;
    }
}
