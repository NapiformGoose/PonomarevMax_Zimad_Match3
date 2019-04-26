using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ILevelManager
{
    IElementsCreator _elementsCreator;
    
    public int Rows { get; set; }
    public int Columns { get; set; }

    public LevelManager(IElementsCreator elementsCreator)
    {
        _elementsCreator = elementsCreator;
        Rows = 12;
        Columns = 12;
    }
    
    

    public void CreateRandomElements ()//int Rows, int Columns//
    {
        for(int i = 0; i < Rows; i++) 
        {
            for(int j = 0; j < Columns; j++)
            {
                
                _elementsCreator.CreateElement(i, j);
            }
        }
    }


}
