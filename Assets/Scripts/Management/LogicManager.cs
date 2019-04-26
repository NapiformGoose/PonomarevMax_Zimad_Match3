using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Логика игрового уровня 
/// </summary>
public class LogicManager : ILogicManager, IUpdatable
{
    IObjectsStorage _objectsStorage;
    IUpdateManager _updateManager;
    ILevelManager _levelManager;
    IElementsCreator _elementsCreator;

    GameObject _circle;
    GameObject _rectangle;
    GameObject _empty;

    Material _red;
    Material _green;
    Material _blue;
    Material _pink;
    Material _purple;

    bool _isSelect; //
    bool _isReady;  //описаны ниже
    bool _isRemove; //
    int countCreateElements = 0; 

    int selectX = -1; //
    int selectY = -1; //
    int swapX = -1;   // описаны ниже
    int swapY = -1;   //

    GameObject _selectObject;
    GameObject _swapObject;

    public int Rows { get; set; }
    public int Columns { get; set; }

    public GameObject[,] Cells { get; set; } //представление двумерного массива Elements в виде двумерного массива GameObject
                                             //Cells[i, j], где i - координата по X и j - координата по Y
                                             //В процессе работы меняются только свойства объектов, сами они всегда соответствуют своим координатам и неподвижны

    public LogicManager(IObjectsStorage objectsStorage, IUpdateManager updateManager, ILevelManager levelManager, IElementsCreator elementsCreator)
    {
        _objectsStorage = objectsStorage;
        _updateManager = updateManager;
        _levelManager = levelManager;
        _elementsCreator = elementsCreator;

        _updateManager.AddUpdatable(this);

        Rows = 12;    //
        Columns = 12; //в персперктиве данные должны быть получени извне

        _isSelect = false;
        _isReady = false;
        _isRemove = false;

        Cells = new GameObject[Columns, Rows];

        LoadResources();
    }

    public void CustomUpdate()
    {
        if (!_isReady)
        {
            Preparation();

        }
        else
        {
            CreateElements();
            Swap();
            Shift();
            CheckLine();
            //ChangeEmpty();
        }
    }

    public void LoadResources()
    {
        _circle = Resources.Load("Prefabs/Circle") as GameObject;
        _rectangle = Resources.Load("Prefabs/Rectangle") as GameObject;
        _empty = Resources.Load("Prefabs/Empty") as GameObject;

        _red = new Material(Resources.Load("Materials/Red") as Material);
        _green = new Material(Resources.Load("Materials/Green") as Material);
        _blue = new Material(Resources.Load("Materials/Blue") as Material);
        _pink = new Material(Resources.Load("Materials/Pink") as Material);
        _purple = new Material(Resources.Load("Materials/Purple") as Material);
    }

    public void CreateNewElements() //создаём двумерный массив элементов
    {
        _levelManager.CreateRandomElements();
    }

    public void CreateElements() //вывод элементов в виде двумерного массива <GameObject> на сцену
    {
        if (countCreateElements == 0) //данный метод должен сработать только один раз
        {
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    switch (_objectsStorage.Elements[i, j].ElementType)
                    {
                        case Type.Circle:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _circle);
                                break;
                            }
                        case Type.Rectangle:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _rectangle);
                                break;
                            }
                    }

                    switch (_objectsStorage.Elements[i, j].ElementColor)
                    {
                        case Color.Blue:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _circle);
                                Cells[i, j].GetComponent<MeshRenderer>().material = _blue;
                                break;
                            }
                        case Color.Red:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _rectangle);
                                Cells[i, j].GetComponent<MeshRenderer>().material = _red;
                                break;
                            }
                        case Color.Pink:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _rectangle);
                                Cells[i, j].GetComponent<MeshRenderer>().material = _pink;
                                break;
                            }
                        case Color.Green:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _rectangle);
                                Cells[i, j].GetComponent<MeshRenderer>().material = _green;
                                break;
                            }
                        case Color.Purple:
                            {
                                Cells[i, j] = new GameObject(_objectsStorage.Elements[i, j].ID);
                                CreateCell(Cells[i, j], _rectangle);
                                Cells[i, j].GetComponent<MeshRenderer>().material = _purple;
                                break;
                            }
                    }

                    Cells[i, j].transform.position = new Vector3(_objectsStorage.Elements[i, j].Xpos, _objectsStorage.Elements[i, j].Ypos, 0);
                }
            }
            countCreateElements++;
        }
    }
    void CreateCell(GameObject newCell, GameObject _form)
    {
        MeshFilter meshFilter = _form.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = _form.GetComponent<MeshRenderer>();
        BoxCollider boxCollider = _form.GetComponent<BoxCollider>();

        meshFilter = newCell.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = _form.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer = newCell.AddComponent<MeshRenderer>();
        boxCollider = newCell.AddComponent<BoxCollider>();

        newCell.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        newCell.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    void UpdateCell(GameObject newCell, GameObject _form)
    {
        MeshRenderer meshRenderer = newCell.GetComponent<MeshRenderer>();
        BoxCollider boxCollider = newCell.GetComponent<BoxCollider>();
        MeshFilter meshFilter = newCell.GetComponent<MeshFilter>();

        meshFilter.sharedMesh = _form.GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.sharedMaterial = _form.GetComponent<MeshRenderer>().sharedMaterial;

        boxCollider = _form.GetComponent<BoxCollider>();

    }

    public void Preparation() //проверка на наличие линий при старте
    {
        _isReady = true;

        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (i < Columns - 2 && j < Rows)
                {

                    if (_objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i + 1, j].ElementColor &&
                        _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i + 2, j].ElementColor)
                    {
                        _elementsCreator.CreateElement(i + 1, j);
                        _isReady = false;
                    }
                }

                if (j < Rows - 2)
                {

                    if (_objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i, j + 1].ElementColor &&
                        _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i, j + 2].ElementColor)
                    {
                        _elementsCreator.CreateElement(i, j + 1);
                        _isReady = false;
                    }
                }
            }
        }
    }

    public void CheckLine() //проверка на наличие линий и их удаление
    {
        bool _stopCheck = false;
        _isRemove = false;
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (i < Rows - 2 && //чтобы не выйти за границы игрового поля 
                        _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i + 1, j].ElementColor &&
                        _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i + 2, j].ElementColor) //проверка по X сразу на 2 координаты вправо
                {
                    Color color = _objectsStorage.Elements[i, j].ElementColor;

                    _objectsStorage.Elements[i, j].ElementType = Type.Empty;
                    _objectsStorage.Elements[i + 1, j].ElementType = Type.Empty;
                    _objectsStorage.Elements[i + 2, j].ElementType = Type.Empty;

                    while (!_stopCheck) //ПОПРОБОВАТЬ ВЫВЕСТИ В РЕКУРСИЮ
                    {
                        if (i < Columns - 3 && j < Rows && _objectsStorage.Elements[i + 3, j].ElementColor == color)
                        {
                            _objectsStorage.Elements[i + 3, j].ElementType = Type.Empty;
                            j++;
                        }
                        else
                        {
                            _stopCheck = true; //остановка проверки т.к. дальше нет совпадений по цвету
                        }
                    }
                    _isRemove = true; //есть линии для удаления
                }

                _stopCheck = false;

                if (j < Columns - 2 && //чтобы не выйти за границы игрового поля 
                    _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i, j + 1].ElementColor &&
                    _objectsStorage.Elements[i, j].ElementColor == _objectsStorage.Elements[i, j + 2].ElementColor) //проверка по Y сразу на 2 координаты вверх
                {
                    Color color = _objectsStorage.Elements[i, j].ElementColor;

                    _objectsStorage.Elements[i, j].ElementType = Type.Empty;
                    _objectsStorage.Elements[i, j + 1].ElementType = Type.Empty;
                    _objectsStorage.Elements[i, j + 2].ElementType = Type.Empty;

                    while (!_stopCheck) //ПОПРОБОВАТЬ ВЫВЕСТИ В РЕКУРСИЮ
                    {
                        if (j < Rows - 3 && _objectsStorage.Elements[i, j + 3].ElementColor == color) //проверка далее чем на 2 координаты
                        {
                            _objectsStorage.Elements[i, j + 3].ElementType = Type.Empty;
                            j++;
                        }
                        else
                        {
                            _stopCheck = true;
                        }
                    }
                    _isRemove = true;
                }
            }
        }
    }

    public void Swap() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (!_isSelect)
                {
                    _isSelect = true; //если выбранных элементов нет, то назначаем этот
                    _selectObject = hit.transform.gameObject;
                    _selectObject.transform.localScale = new Vector3(1, 1, 1);
                    selectX = System.Convert.ToInt32(_selectObject.transform.position.x); //сохраняем координаты для двух выбранных объектов
                    selectY = System.Convert.ToInt32(_selectObject.transform.position.y); //
                }
                else
                {
                    _isSelect = false; //этот элемент является вторым выбранным, следующий выбранный снова будет select
                    _swapObject = hit.transform.gameObject;
                    swapX = System.Convert.ToInt32(_swapObject.transform.position.x);
                    swapY = System.Convert.ToInt32(_swapObject.transform.position.y);
                }

                if (selectX != -1 && selectY != -1 && swapX != -1 && swapY != -1) //проверка на то что выбраны 2 элемента для дальнейшей работы
                {
                    if ((selectX + 1 == swapX && selectY == swapY) || (selectX - 1 == swapX && selectY == swapY) ||
                   (selectX == swapX && selectY + 1 == swapY) || (selectX == swapX && selectY - 1 == swapY)) //если выбранные элементы находятся рядом
                    {
                        _selectObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

                        ChangeCells(); //swap элементов и ячеек соответственно
                        CheckLine(); 

                        if (!_isRemove)
                        {
                            ReturnCells(); //swap элементов и ячеек обратно т.к. линии не получается
                            Debug.Log("ВОЗВРАТ ЭЛЕМЕНТОВ");
                        }
                    }
                    else
                    {
                        _selectObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    }

                    _selectObject = null;
                    _swapObject = null;

                    selectX = -1;
                    selectY = -1;
                    swapX = -1; 
                    swapY = -1;
                }
            }

        }
    }

    void ChangeCells() //swap элементов и ячеек соответственно
    {
        Vector3 tempPos1 = _selectObject.transform.position - _swapObject.transform.position;
        Vector3 tempPos2 = _swapObject.transform.position - _selectObject.transform.position;

        Color color = _objectsStorage.Elements[selectX, selectY].ElementColor;
        _objectsStorage.Elements[selectX, selectY].ElementColor = _objectsStorage.Elements[swapX, swapY].ElementColor;
        _objectsStorage.Elements[swapX, swapY].ElementColor = color;

        Cells[selectX, selectY].transform.Translate(tempPos2, Space.World); //здесь ячейки перемещаются хоть и должны быть неподвижны
        Cells[swapX, swapY].transform.Translate(tempPos1, Space.World);     //ниже они обмениваются значениями
        
        Cells[selectX, selectY].name = _objectsStorage.Elements[swapX, swapY].ID; 
        Cells[swapX, swapY].name = _objectsStorage.Elements[selectX, selectY].ID;
        
        GameObject tempCell = Cells[selectX, selectY];
        Cells[selectX, selectY] = Cells[swapX, swapY];
        Cells[swapX, swapY] = tempCell;
    }
    void ReturnCells() //swap элементов и ячеек обратно т.к. линии не получается
    {
        Vector3 tempPos1 = _swapObject.transform.position - _selectObject.transform.position; 
        Vector3 tempPos2 = _selectObject.transform.position - _swapObject.transform.position;

        Color color = _objectsStorage.Elements[swapX, swapY].ElementColor;
        _objectsStorage.Elements[swapX, swapY].ElementColor = _objectsStorage.Elements[selectX, selectY].ElementColor;
        _objectsStorage.Elements[selectX, selectY].ElementColor = color;

        Cells[selectX, selectY].transform.Translate(tempPos2, Space.World);
        Cells[swapX, swapY].transform.Translate(tempPos1, Space.World);

        Cells[selectX, selectY].name = _objectsStorage.Elements[swapX, swapY].ID; 
        Cells[swapX, swapY].name = _objectsStorage.Elements[selectX, selectX].ID;

        GameObject tempCell = Cells[swapX, swapY];
        Cells[swapX, swapY] = Cells[selectX, selectY];
        Cells[selectX, selectY] = tempCell;
    }
    public void Shift() //сдвигает эелементы сверху вместо пустых элементов
    {
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (_objectsStorage.Elements[i, j].ElementType == Type.Empty)
                {
                    Element topElement = CheckTop(i, j); //проверяем верхние элементы что переместить первый возможный
                    Debug.Log(_objectsStorage.Elements[i, j].ID + "-" + topElement.ID);    
                    _objectsStorage.Elements[i, j].ElementColor = topElement.ElementColor; //
                    _objectsStorage.Elements[i, j].ElementType = topElement.ElementType;   //заменяем нижний элемент верхним
                    _objectsStorage.Elements[i, j].ID = topElement.ID;                     //
                    _objectsStorage.Elements[i, j].Xpos = topElement.Xpos;                 //
                    _objectsStorage.Elements[i, j].Ypos = topElement.Ypos;                 //
                    Debug.Log(_objectsStorage.Elements[topElement.Xpos, topElement.Ypos].ElementType);

                    if(topElement.Ypos != Columns - 1)
                    {
                        _objectsStorage.Elements[topElement.Xpos, topElement.Ypos].ElementType = Type.Empty;
                    }
                    UpdateCell(Cells[i, j], Cells[topElement.Xpos, topElement.Ypos]);
                }
            }
        }
    }

    Element CheckTop(int i, int j) 
    {
        if (j < Rows - 1 && _objectsStorage.Elements[i, j + 1].ElementType == Type.Empty)
        {
            CheckTop(i, j + 1);
        }
        if (j == Rows - 1 && _objectsStorage.Elements[i, j].ElementType == Type.Empty)
        {
            ChangeEmpty();
            Debug.Log("1");
            return _objectsStorage.Elements[i, j];
        }
        else if (j != Columns - 1)
        {
            Debug.Log("2");
            Debug.Log("return " + _objectsStorage.Elements[i, j + 1].ID);
            return _objectsStorage.Elements[i, j + 1]; 
        }
        else
        {
            Debug.Log("3");
            return _objectsStorage.Elements[i, j];
        }
        
    }

    void ChangeEmpty() //элементы с типо Empty преобразуем, создав новый рандомный элемент
    {
        for (int i = 0; i < Rows; i++)
        {
            if (_objectsStorage.Elements[i, Columns - 1].ElementType == Type.Empty)
            {
                _elementsCreator.CreateElement(i, Columns - 1);

                if (_objectsStorage.Elements[i, Columns - 1].ElementType == Type.Circle)
                {
                    UpdateCell(Cells[i, Columns - 1], _circle);
                }
                else
                {
                    UpdateCell(Cells[i, Columns - 1], _rectangle);
                }
                Vector3 pos = new Vector3(_objectsStorage.Elements[i, Columns - 1].Xpos, _objectsStorage.Elements[i, Columns - 1].Ypos, 0);
                Cells[i, Columns - 1].transform.position = pos;
                Cells[i, Columns - 1].GetComponent<MeshRenderer>().material = new Material(Resources.Load("Materials/" + _objectsStorage.Elements[i, Columns - 1].ElementColor) as Material);
                Debug.Log(_objectsStorage.Elements[i, Columns - 1].ID + "заменён на " + _objectsStorage.Elements[i, Columns - 1].ElementColor);
            }
        }
    }



    
}





