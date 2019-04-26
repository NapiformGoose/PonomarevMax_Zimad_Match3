using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateManager
{
    void AddUpdatable(IUpdatable updatable);
    void RemoveUpdatable(IUpdatable updatable);
    void Start();
    void Stop();
}
