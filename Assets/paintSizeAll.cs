using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

public class paintSizeAll : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Настройки кисти")]
    public float Radius;
    public int ImageSize = 28; // Размер кисти в пикселях
    public Vector2 normalized;
    public Vector2 localPoint;
    public bool isDraw;

    public RawImage rawImage;
    private Texture2D drawTexture; // Текстура, на которой рисуем
    private Color[] resetColorArray; // Для сброса
    public int curentColor = 1;

    public Color ColorBlek = Color.black;
    public Color ColorWhait = Color.white;
    public bool isColor = true;

    public TMP_InputField SizeText;
    public Slider FillRadius;

    public bool isWebCamera;
    public WebCamTexture webcamTexture;

    private void Start()
    {
        UpdateTextur2D();
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("Камера не найдена!");
            return;
        }

        // Создаем и запускаем текстуру с первой камеры
        webcamTexture = new WebCamTexture(devices[0].name, 640, 480, 30);
    }

    private void Update()
    {
        Radius = (ImageSize / 4) * FillRadius.value;

        if (Input.GetKeyDown(KeyCode.W))
            isWebCamera = !isWebCamera;
        if (isWebCamera)
        {
            // Включаем камеру
            if (webcamTexture != null)
            {
                rawImage.texture = webcamTexture;
                webcamTexture.Play();
                Debug.Log("Камера запущена");
            }
        }
        else
        {
            // Выключаем камеру и возвращаем текстуру для рисования
            if (webcamTexture != null && webcamTexture.isPlaying)
            {
                webcamTexture.Stop();
                rawImage.texture = drawTexture;
                Debug.Log("Камера остановлена");
            }
        }
    }

    public void ButtUpdat()
    {
        if (isWebCamera) return;
        ImageSize = int.Parse(SizeText.text);
        Debug.Log("" + ImageSize);
        UpdateTextur2D();
    }

    public void ButtColor()
    {
        if (isWebCamera) return;
        isColor = !isColor;
        curentColor = isColor ? 1 : -1;
    }

    public void UpdateTextur2D()
    {
        if (isWebCamera) return;
        drawTexture = new Texture2D(ImageSize, ImageSize);

        Color[] white = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < white.Length; i++)
            white[i] = new Color(0,0,0,0);
        drawTexture.SetPixels(white);
        drawTexture.filterMode = FilterMode.Point;
        drawTexture.Apply();

        resetColorArray = drawTexture.GetPixels();
        rawImage.texture = drawTexture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isWebCamera) return;
        isDraw = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isWebCamera) return;
        Debug.Log("КЛИК СРАБОТАЛ по " + gameObject.name);

        RectTransform rect = GetComponent<RectTransform>();

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            Camera.main,
            out localPoint
            ))
        {
            Vector2 size = rect.rect.size;

            normalized = new Vector2(
                ((localPoint.x / size.x) + 0.5f) * ImageSize,
                ((localPoint.y / size.y) + 0.5f) * ImageSize
                );

            List<Vector2> BoxPint = new List<Vector2>();
            List<Vector2> CirclePint = new List<Vector2>();
            Vector2 starPos = new Vector2((int)normalized.x - Radius/2, (int)normalized.y + Radius/2);
            int wall = 0;
            for(var i =0; i < Radius; i++)
            {
                wall--;
                for (var i2 = 0; i2 < Radius; i2++)
                {
                    Vector2 currentPix = new Vector2((int)starPos.x + (int)i2, (int)starPos.y + (int)wall);
                    BoxPint.Add(currentPix);
                }
            }

            foreach (var i in BoxPint)
            {
                if (Mathf.Pow(Radius/2, 2) >= (int)Mathf.Pow(i.x - normalized.x, 2) + (int)Mathf.Pow(i.y - normalized.y, 2))
                {
                    drawTexture.SetPixel((int)i.x, (int)i.y, drawTexture.GetPixel((int)i.x, (int)i.y) + new Color(0, 0, 0, 0.2f * curentColor));
                    drawTexture.Apply();
                    if (Mathf.Pow(Radius / 3, 2) >= (int)Mathf.Pow(i.x - normalized.x, 2) + (int)Mathf.Pow(i.y - normalized.y, 2))
                    {
                        drawTexture.SetPixel((int)i.x, (int)i.y, drawTexture.GetPixel((int)i.x, (int)i.y) + new Color(0,0,0,0.4f * curentColor));
                        drawTexture.Apply();
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isWebCamera) return;
        isDraw = false;
    }

    public void ResetPaint()
    {
        if (isWebCamera) return;
        Color[] white = new Color[drawTexture.width * drawTexture.height];
        for (int i = 0; i < white.Length; i++)
            white[i] = new Color(0, 0, 0, 0);
        drawTexture.SetPixels(white);
        drawTexture.filterMode = FilterMode.Point;
        drawTexture.Apply();
    }
}