using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Основной класс, отвечающий за запуск базового функционала 
/// </summary>

public class Bootstrapper : MonoBehaviour
{
    IObjectsStorage _objectsStorage;
    IUpdateManager _updateManager;
    ILevelManager _levelManager;
    IElementsCreator _elementsCreator;
    ILogicManager _logicManager;

    void Start()
    {
        var updateManagerObject = new GameObject("UpdateManager");
        _updateManager = updateManagerObject.AddComponent<UpdateManager>();

        _objectsStorage = new ObjectsStorage();
        _elementsCreator = new ElementsCreator(_objectsStorage);
        _levelManager = new LevelManager(_elementsCreator);
        _logicManager = new LogicManager(_objectsStorage, _updateManager, _levelManager, _elementsCreator);
        _logicManager.CreateNewElements();


        _updateManager.Start(); //начало игры
    }
}
