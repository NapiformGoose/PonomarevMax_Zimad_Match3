using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Хранение основных геймплейных объектов
/// </summary>

public class ObjectsStorage : IObjectsStorage
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public Element[,] Elements { get; set; }
    //public IList<Element> Elements { get; set; }

    public ObjectsStorage() 
    {
        Rows = 12;
        Columns = 12;
        Elements = new Element[Rows, Columns];
    }

    public void AddItem(Element element, int x, int y) 
    {
       // Element element = item as Element; //подумать
        //if(element != null)
        //{
            Elements[x, y] = element;
            //Elements.Add(element);
        //}
    }

    public void RemoveGameObject()
    {

    }
}
