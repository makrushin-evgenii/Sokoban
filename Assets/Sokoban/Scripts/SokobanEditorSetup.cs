using UnityEngine;

public class SokobanEditorSetup : MonoBehaviour
{

    [Header("Настройки меню редактора")]
    public Vector2 Position = new Vector2(10, 10);
    public float Width = 400;
    public float Height = 60;
    [Header("Папка с префабами (в Resources)")]
    public string PrefabsPath = "Prefabs";
    [Header("Настройки сетки")]
    public Sprite CellSprite;
    public Color CellColor = Color.white;
    public int GridWidth = 10;
    public int GridHeight = 10;
    public float CellSize = 1;
    [Header("Объект карты")]
    public Transform Map;

    // Данные переменные используются меню выбора префабов
    [HideInInspector] public Transform[] Prefabs;
    [HideInInspector] public string[] PrefabNames;
    [HideInInspector] public string[] PrefabTags;
    [HideInInspector] public int Index;
    [HideInInspector] public bool ShowButton, Project2D;
    [HideInInspector] public string TagField;
    [HideInInspector] public LayerMask LayerMask;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ClearMap()
    {
        ClearTransform(Map);
    }

    public void Create()
    {
        ClearTransform(transform);
        var blankCell = GetBlankCell();
        FillGrid(blankCell);
        DestroyImmediate(blankCell.gameObject);
    }

    public void SetPrefab(GameObject obj)
    {
        if (Prefabs.Length == 0)
        {
            return;
        }

        var clone = Instantiate(Prefabs[Index], obj.transform.position - Vector3.forward * 0.05f, Quaternion.identity) as Transform;
        clone.gameObject.name = Prefabs[Index].name;
        clone.parent = Map;
    }

    public void LoadResources()
    {
        Prefabs = Resources.LoadAll<Transform>(PrefabsPath);

        PrefabNames = new string[Prefabs.Length];
        PrefabTags = new string[Prefabs.Length];
        for (int i = 0; i < Prefabs.Length; i++)
        {
            PrefabNames[i] = Prefabs[i].name;
            PrefabTags[i] = Prefabs[i].tag;
        }

        Index = 0;
    }

    private Transform GetBlankCell()
    {
        Transform template = new GameObject().transform;
        template.gameObject.tag = "EditorOnly";
        template.gameObject.AddComponent<SpriteRenderer>().sprite = CellSprite;
        template.gameObject.GetComponent<SpriteRenderer>().color = CellColor;
        template.gameObject.AddComponent<BoxCollider2D>();

        return template;
    }

    private void FillGrid(Transform blankCell)
    {
        var leftCorner = -CellSize * GridWidth / 2 + CellSize / 2;
        var topCorner = CellSize * GridHeight / 2 - CellSize / 2;
        int cellIndex = 0;

        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                PutCell(blankCell, cellIndex++, leftCorner + j * CellSize, topCorner - i * CellSize);
            }
        }
    }

    private void PutCell(Transform blankCell, int cellIndex, float x, float y)
    {
        var tr = Instantiate(blankCell) as Transform;
        tr.SetParent(transform);
        tr.localScale = Vector3.one;
        tr.position = new Vector2(x, y);
        tr.name = "Cell_" + cellIndex;
    }

    private void ClearTransform(Transform tr)
    {
        GameObject[] obj = new GameObject[tr.childCount];

        for (int i = 0; i < tr.childCount; i++)
        {
            obj[i] = tr.GetChild(i).gameObject;
        }

        foreach (GameObject t in obj)
        {
            DestroyImmediate(t);
        }
    }
}