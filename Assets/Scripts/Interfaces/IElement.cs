using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IElement 
{
    int Xpos { get; set; }
    int Ypos { get; set; }
    string ID { get; set; }
    Color ElementColor { get; set; }
    Type ElementType { get; set; }
}

