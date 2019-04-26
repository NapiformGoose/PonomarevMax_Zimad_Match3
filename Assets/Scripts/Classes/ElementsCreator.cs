using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создание элементов и запись в ObjectsStorage
/// </summary>
public class ElementsCreator : IElementsCreator
{
    IObjectsStorage _objectsStorage;

    public ElementsCreator(IObjectsStorage objectsStorage)
    {
        _objectsStorage = objectsStorage;
    }

    public void CreateElement(int x, int y) 
    {          
        Element element = new Element();
        element.Xpos = x;
        element.Ypos = y;
        element.ID = x.ToString() + "-" + y.ToString();
;
        int i = Random.Range(0, 2);
        switch (i)
        {
            case 0:
                {
                    element.ElementType = Type.Circle;
                    break;
                }
            case 1:
                {
                    element.ElementType = Type.Rectangle;
                    break;
                }
        }
        switch (Random.Range(0, 5))
        {
            case 0:
                {
                    element.ElementColor = Color.Blue;
                    break;
                }
            case 1:
                {
                    element.ElementColor = Color.Green;
                    break;
                }
            case 2:
                {
                    element.ElementColor = Color.Red;
                    break;
                }
            case 3:
                {
                    element.ElementColor = Color.Pink;
                    break;
                }
            case 4:
                {
                    element.ElementColor = Color.Purple;
                    break;
                }
        }

        _objectsStorage.AddItem(element, element.Xpos, element.Ypos);
    }
}
