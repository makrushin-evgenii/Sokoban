#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SokobanEditorSetup))]
public class SokobanEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawCustomInspector();
    }

    void OnSceneGUI()
    {
        var t = (SokobanEditorSetup)target;

        DrawPrefabPalette(t);
        HandleUserInput(t);
    }

    private void DrawCustomInspector()
    {
        var t = (SokobanEditorSetup)target;

        GUILayout.Label("Управление:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Создать / Обновить сетку"))
        {
            t.Create();
        }

        if (GUILayout.Button("Очистить сетку"))
        {
            t.ClearMap();
        }

        GUILayout.EndHorizontal();
    }

    private void DrawPrefabPalette(SokobanEditorSetup target)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(target.Position.x, target.Position.y, target.Width, target.Height), EditorStyles.helpBox);

        if (GUILayout.Button("Загрузить список префабов"))
        {
            target.LoadResources();
        }

        GUILayout.TextArea("установить выбранный префаб ЛКМ, убрать префаб Shift+ЛКМ");
        GUILayout.BeginHorizontal();
        GUILayout.TextField("Выбор префаба: ");
        target.Index = EditorGUILayout.Popup(target.Index, target.PrefabNames);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    private void HandleUserInput(SokobanEditorSetup target)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // отмена выбора объекта ЛКМ в окне редактора

        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown || Event.current.button == 0 && Event.current.type == EventType.mouseDrag)
        {
            var clickPos = new Vector2(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y);
            var hit = Physics2D.Raycast(SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(clickPos), Vector2.zero);

            UpdateHitCell(target, hit);
        }
    }

    private void UpdateHitCell(SokobanEditorSetup target, RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            if (!Event.current.shift)
            {
                // В одну клетку можно поставить только один обьект одного типа
                if (hit.collider.tag.CompareTo(target.PrefabTags[target.Index]) != 0)
                {
                    target.SetPrefab(hit.transform.gameObject);
                }
            }
            else if (hit.collider.tag.CompareTo("EditorOnly") != 0)
            {
                // Разметка имеет тэг EditorOnly, её удалять нельзя
                DestroyImmediate(hit.transform.gameObject);
            }
        }
    }
}
#endif