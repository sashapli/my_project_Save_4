using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class SmartVisionSystemAi4 : MonoBehaviour
{
    public static SmartVisionSystemAi4 systemAi4;

    public List<float> Pixsel;

    public RenderTexture renderTexture;
    public RawImage rawImage3;
    public RawImage rawImage;
    public RawImage rawImage2;
    public Texture2D texture2D;

    public int newWidth = 512;
    public int newHeight = 512;

    public List<float> EdgesCV = new List<float>();
    public List<bool> BinaryEdgesCV = new List<bool>();
    public List<List<Vector2Int>> FindConnectedCom = new List<List<Vector2Int>>();
    public List<BoundingBox> components = new List<BoundingBox>();

    public float OffsetXGUI, OffsetYGUI, SizeGUI, Size2GUI, OffsetX2GUI, OffsetY2GUI;
    public List<Texture2D> AllCommponTextur;
    public float SixeEyeAi3 = 28;
    public float NoiseSelect = 600;

    public NeuronNetWork3 MNSET;
    public LearnData learnData;
    public List<SpriteListLearn> LearnSprit;

    private Texture2D whiteTexture;

    void Start()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.red);
        whiteTexture.Apply();
        systemAi4 = this;
    }

    public void ThinkCV(int trueNamder, bool isLearn)
    {
        Pixsel.Clear();
        AllCommponTextur.Clear();
        components.Clear();
        FindConnectedCom.Clear();
        Texture2D result;
        if (trueNamder == 99)
        {
            Texture2D sourceTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            sourceTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            sourceTexture.Apply();
            RenderTexture.active = null;
            result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            result.SetPixels(sourceTexture.GetPixels());
            result.Apply();

            // 3. Конвертируем в серый для анализа
            foreach (var pixel in sourceTexture.GetPixels())
                Pixsel.Add(1 - ((pixel.r + pixel.g + pixel.b) / 3));

            EdgesCV = FindEdges(Pixsel, newWidth, newHeight);
            BinaryEdgesCV = BinaryMap(EdgesCV, newWidth, newHeight, 0.05f);
            FindConnectedCom = FindConnectedComponents(BinaryEdgesCV, newWidth, newHeight);
        }
        else
        {
            NoiseSelect = 50;
            result = new Texture2D(28, 28, TextureFormat.RGBA32, false);
            List<float> FData= new List<float>();
            switch (trueNamder)
            {
                case 0: FData = learnData.data.zero[Random.Range(0, learnData.data.zero.Count)].FL; break;
                case 1: FData = learnData.data.one[Random.Range(0, learnData.data.one.Count)].FL; break;
                case 2: FData = learnData.data.two[Random.Range(0, learnData.data.two.Count)].FL; break;
                case 3: FData = learnData.data.three[Random.Range(0, learnData.data.three.Count)].FL; break;
                case 4: FData = learnData.data.four[Random.Range(0, learnData.data.four.Count)].FL; break;
                case 5: FData = learnData.data.five[Random.Range(0, learnData.data.five.Count)].FL; break;
                case 6: FData = learnData.data.six[Random.Range(0, learnData.data.six.Count)].FL; break;
                case 7: FData = learnData.data.seven[Random.Range(0, learnData.data.seven.Count)].FL; break;
                case 8: FData = learnData.data.eight[Random.Range(0, learnData.data.eight.Count)].FL; break;
                case 9: FData = learnData.data.nine[Random.Range(0, learnData.data.nine.Count)].FL; break;
            }

            Color[] Clis = new Color[FData.Count];
            for (var i = 0; i < Clis.Length; i++)
                Clis[i] =new Color(1-FData[i],1- FData[i],1- FData[i]);
            result.SetPixels(Clis);
            result.Apply();

            EdgesCV = FindEdges(FData, 28, 28);
            BinaryEdgesCV = BinaryMap(EdgesCV, 28, 28, 0.05f);
            FindConnectedCom = FindConnectedComponents(BinaryEdgesCV, 28, 28);
        }

        for (int i = 0; i < FindConnectedCom.Count; i++)
        {
            BoundingBox newBox = new BoundingBox(FindConnectedCom[i]);
            components.Add(newBox);
        }
        if (trueNamder != 99)
            rawImage3.texture = result;
        rawImage.texture = result;

        if (components.Count == 0)
        {
            Debug.Log("Объект не найден!");
            MNSET.isProcessing = false;  // ← СБРОСИТЬ ФЛАГ!
            return;  // ← ВЫЙТИ ИЗ МЕТОДА!
        }

        for (var i = 0; i < components.Count; i++)
        {
            Color[] CurCompon = result.GetPixels(components[i].x, components[i].y, components[i].width, components[i].height);
            Texture2D CurTextur = new Texture2D(components[i].width, components[i].height);
            CurTextur.SetPixels(CurCompon);
            CurTextur.Apply();

            int drawW, drawH, offsetX, offsetY;
            // 3. Рассчитываете, какого размера рисовать
            float SA = (float)components[i].width / components[i].height;
            if (SA > 1) // Широкое
            {
                drawW = 28;
                drawH = (int)(28 / SA);
                offsetX = 0;
                offsetY = (28 - drawH) / 2;
            }
            else // Высокое
            {
                drawW = (int)(28 * SA);
                drawH = 28;
                offsetX = (28 - drawW) / 2;
                offsetY = 0;
            }
            Color[] newPixels = new Color[28 * 28];

            for (int i2 = 0; i2 < newPixels.Length; i2++)
                newPixels[i2] = Color.white;

            for (var y = 0; y < drawH; y++)
            {
                for (var x = 0; x < drawW; x++)
                {
                    int srcX = (int)((float)x / drawW * components[i].width);
                    int srcY = (int)((float)y / drawH * components[i].height);

                    int destX = offsetX + x;
                    int destY = offsetY + y;
                    newPixels[destY * 28 + destX] = CurCompon[srcY * components[i].width + srcX];
                }
            }

            // Создаём текстуру из нового массива
            Texture2D resultCom = new Texture2D(28, 28);
            resultCom.SetPixels(newPixels);
            resultCom.Apply();

            List<float> OnePixsel = new List<float>();
            foreach (var p in resultCom.GetPixels())
                OnePixsel.Add(1 - (p.r + p.g + p.b) / 3);

            Texture2D testTExtur = new Texture2D(28, 28);

            Color[] colors = new Color[784];
            for (var i2 = 0; i2 < OnePixsel.Count; i2++)
                colors[i2] = new Color(OnePixsel[i2], OnePixsel[i2], OnePixsel[i2], 1);

            testTExtur.SetPixels(colors);
            testTExtur.Apply();
            rawImage2.texture = testTExtur;
            List<float> NambTt =  MNSET.ThinkAi(trueNamder, isLearn, OnePixsel);
            string ind = "NaN";
            for(var i3 = 0; i3 < NambTt.Count;i3++) {
                if (NambTt[i3] > 0.75f)
                {
                    ind = "" + i3;
                }
            }

            components[i].NameObject = "" + ind;

            MNSET.isProcessing = false;
        }
    }

    public List<float> FindEdges(List<float> MapVisual, int width, int height)
    {
        List<float> edges = new List<float>(new float[MapVisual.Count]);

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int index = y * width + x;

                float left = MapVisual[y * width + (x - 1)];
                float right = MapVisual[y * width + (x + 1)];
                float up = MapVisual[(y - 1) * width + x];
                float down = MapVisual[(y + 1) * width + x];

                float gradX = right - left;
                float gradY = down - up;
                edges[index] = Mathf.Sqrt(gradX * gradX + gradY * gradY);
            }
        }
        //   Debug.Log(string.Join(": ", edges));
        return edges;
    }

    public List<bool> BinaryMap(List<float> edges, int width, int height, float threshold = 0.5f)
    {
        List<bool> binary = new List<bool>(new bool[edges.Count]);

        for (int i = 0; i < edges.Count; i++)
        {
            binary[i] = edges[i] > threshold;
        }
        return binary;
    }

    public List<List<Vector2Int>> FindConnectedComponents(List<bool> binary, int width, int height)
    {
        List<List<Vector2Int>> components = new List<List<Vector2Int>>();
        bool[] visited = new bool[binary.Count];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (binary[index] && !visited[index])
                {
                    List<Vector2Int> currentObject = new List<Vector2Int>();
                    FloodFill(binary, visited, x, y, width, height, currentObject);

                    if (currentObject.Count > NoiseSelect)
                        components.Add(currentObject);
                }
            }
        }
        return components;
    }

    void FloodFill(List<bool> binary, bool[] visited, int startX, int startY, int width, int height, List<Vector2Int> currentObject)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0),
            new Vector2Int(1,0)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current.x < 0 || current.x >= width || current.y < 0 || current.y >= height)
                continue;
            int index = current.y * width + current.x;
            if (!binary[index] || visited[index])
                continue;
            visited[index] = true;
            currentObject.Add(current);
            foreach (Vector2Int dir in directions)
            {
                queue.Enqueue(new Vector2Int(current.x + dir.x, current.y + dir.y));
            }
        }
    }

    public Texture2D SpriteToTextur2D(Sprite sprite)
    {
        Rect rect = sprite.rect;

        Texture2D regionTexture = new Texture2D(28, 28);

        Color[] pixsels = sprite.texture.GetPixels(
            (int)rect.x,
            (int)rect.y,
            28,
            28
            );
        regionTexture.SetPixels(pixsels);
        regionTexture.Apply();

        return regionTexture;
    }

    private void OnGUI()
    {
        foreach (var obj in components)
        {
            // Нормализованные координаты внутри текстуры (0..1)
            float centerX = (obj.x + obj.width / 2f) / 128f;
            float centerY = (obj.y + obj.height / 2f) / 128f;

            // Размер RawImage на экране
            Vector2 rawSize = rawImage.rectTransform.rect.size;

            // Позиция левого верхнего угла RawImage в экранных координатах
            Vector3 topLeft = rawImage.transform.position;
            Vector2 screenTopLeft = Camera.main.WorldToScreenPoint(topLeft);
            float screenX = screenTopLeft.x + centerX * rawSize.x * SizeGUI + OffsetXGUI;
            float screenY = Screen.height - (screenTopLeft.y + centerY * rawSize.y * SizeGUI + OffsetYGUI);

            // Размер рамки в экранных пикселях
            float rectWidth = (obj.width / 128f) * rawSize.x * SizeGUI;
            float rectHeight = (obj.height / 128f) * rawSize.y * SizeGUI;

            // Центрируем рамку
            Rect rect = new Rect(
                screenX - rectWidth / 2f,
                screenY - rectHeight / 2f,
                rectWidth,
                rectHeight
            );

            // Рисуем 4 линии вместо бокса
            Vector2 topLeft2 = new Vector2(rect.x, rect.y);
            Vector2 topRight = new Vector2(rect.x + rect.width, rect.y);
            Vector2 bottomLeft = new Vector2(rect.x, rect.y + rect.height);
            Vector2 bottomRight = new Vector2(rect.x + rect.width, rect.y + rect.height);

            float thickness = 2f;
            Color color = Color.red;

            // Верхняя линия
            GUI.DrawTexture(new Rect(topLeft2.x, topLeft2.y, rect.width, thickness), whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            // Нижняя линия
            GUI.DrawTexture(new Rect(bottomLeft.x, bottomLeft.y - thickness, rect.width, thickness), whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            // Левая линия
            GUI.DrawTexture(new Rect(topLeft2.x, topLeft2.y, thickness, rect.height), whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            // Правая линия
            GUI.DrawTexture(new Rect(topRight.x - thickness, topRight.y, thickness, rect.height), whiteTexture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.red;
            float labelX = rect.x + rect.width + 5f; // 5px отступ
            float labelY = rect.y + rect.height / 2f - 15f; // По центру по вертикали

            GUI.Label(new Rect(labelX, labelY, 200, 30), "Object - " + obj.NameObject, labelStyle);
        }
    }
}
[System.Serializable]
public class BoundingBox
{
    public int x;      // Левый верхний угол X
    public int y;      // Левый верхний угол Y
    public int width;  // Ширина
    public int height; // Высота

    public string NameObject;

    public BoundingBox(List<Vector2Int> pixels)
    {
        if (pixels == null || pixels.Count == 0)
        {
            x = y = width = height = 0;
            return;
        }

        // Находим крайние точки
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int pixel in pixels)
        {
            if (pixel.x < minX) minX = pixel.x;
            if (pixel.x > maxX) maxX = pixel.x;
            if (pixel.y < minY) minY = pixel.y;
            if (pixel.y > maxY) maxY = pixel.y;
        }

        // Создаем прямоугольник
        x = minX;
        y = minY;
        width = maxX - minX + 1;
        height = maxY - minY + 1;
    }

    // Для отладки
    public override string ToString()
    {
        return $"Rect: ({x}, {y}) -> {width}x{height}";
    }
}