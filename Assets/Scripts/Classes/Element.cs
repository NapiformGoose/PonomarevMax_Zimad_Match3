using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Объектная модель игровых элементов
/// С объектами данного класса работает вся логика проверок
/// </summary>
public enum Type { Rectangle, Circle, Empty}
public enum Color { Green, Red, Blue, Purple, Pink }

public struct Element : IElement
{
    public int Xpos { get; set; }
    public int Ypos { get; set; }
    public string ID { get; set; }
    public Color ElementColor { get; set; }
    public Type ElementType { get; set; }
}
