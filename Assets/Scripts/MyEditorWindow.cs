using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    private Color singleColor;
    private Color eraseColor;
    private GameObject cubePrefab;

    private Texture2D texture2D;

    private int gridSize = 8;

    private Color[,] _boxColors;
    private void OnEnable()
    {
        _boxColors = new Color[gridSize, gridSize];
        ColorInit();
    }

    [MenuItem("Tools/My Editor Window")]
    private static void OpenMyWindow()
    {
        GetWindow<MyEditorWindow>();
    }

    private void OnGUI()
    {
        if (!cubePrefab)
        {
            Debug.Log("Нет префаба");
        }
        
        Event e = Event.current;
        
        GUILayout.Label("Toolbar");
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Paint Color");
        singleColor = EditorGUILayout.ColorField(singleColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Erase Color");
        eraseColor = EditorGUILayout.ColorField(eraseColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Object Reference");
        cubePrefab = EditorGUILayout.ObjectField(cubePrefab, typeof(GameObject), true) as GameObject;
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < gridSize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var oldColor = GUI.color;
            for (int j = 0; j < gridSize; j++)
            {
                GUILayout.Box(SetTextureColor(_boxColors[i,j]), GUILayout.ExpandHeight(true));
                if (e.button == 0 && e.isMouse && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                {
                    _boxColors[i, j] = singleColor;
                    Event.current.Use();
                }
                
                if (e.button == 1 && e.isMouse && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                {
                    _boxColors[i, j] = eraseColor;
                    Event.current.Use();
                }
            }

            GUI.color = oldColor;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Fill"))
        {
            FillWithColor();
        }
        
        if (GUILayout.Button("Apply texture"))
        {
            CreateTexture();
        }
    }

    private void CreateTexture()
    {
        Texture2D texture = new Texture2D(gridSize,  gridSize);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                texture.SetPixel(i, j, _boxColors[i,j]);
            }
        }
        texture.Apply();

        if (cubePrefab.TryGetComponent(out MeshRenderer renderer))
        {
            Material material = renderer.material; 
            material.mainTexture = texture;
            Debug.Log("Текстура создана");
        }
        else
        {
            Debug.Log("Ошибка создания текстуры (Нет рендерера)");
        }
    }

    private Texture2D SetTextureColor(Color color)
    {
        texture2D = new Texture2D(32,32);
        for (int i = 0; i < texture2D.width; i++)
        {
            for (int j = 0; j < texture2D.height; j++)
            {
                texture2D.SetPixel(i,j, color);
            }
        }
        texture2D.Apply();
        return texture2D;
    }

    private void ColorInit()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                _boxColors[i, j] = new Color(Random.value, Random.value, Random.value);
            }
        }
        Debug.Log(_boxColors.Length);
    }

    private void FillWithColor()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                _boxColors[i, j] = singleColor;
            }
        }
    }
}
