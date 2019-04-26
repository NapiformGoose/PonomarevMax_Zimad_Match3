using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicManager 
{
    void LoadResources();
    void CreateNewElements();
    void CreateElements();
    void Preparation();
    void CheckLine();
    void Swap();
    void Shift();
}
