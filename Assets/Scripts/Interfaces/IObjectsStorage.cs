using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Хранение основных геймплейных объектов
/// </summary>

public interface IObjectsStorage
{
    Element[,] Elements { get; set; }

    void AddItem(Element element, int x, int y);
    void RemoveGameObject();
}
