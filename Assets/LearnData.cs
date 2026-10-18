using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class LearnData : MonoBehaviour
{
    public List<float> Pixsels;
    public string NameFile = "LearnDataAI3.JSON";
    public LearnDataPixel data;

    public RawImage rawImage;
    public int newWidth = 28;
    public int newHeight = 28;

    void Start()
    {
        data = new LearnDataPixel();
        Pixsels = new List<float>();
        LoadData();
    }

    void LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, NameFile);

        if (File.Exists(path))
        {
            Debug.Log($"{NameFile} - существует");
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<LearnDataPixel>(json);
        }
        else
        {
            Debug.LogError($"Файл {NameFile} не найден в {path}");
        }
    }

    void Update()
    {
        Pixsels.Clear(); // ← ОЧИЩАЕМ!
        TeaTurPix();

        if (Input.GetKeyDown(KeyCode.Alpha0)) SaveLernData(0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) SaveLernData(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SaveLernData(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SaveLernData(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SaveLernData(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SaveLernData(5);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SaveLernData(6);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SaveLernData(7);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SaveLernData(8);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SaveLernData(9);
    }

    public void SaveLernData(int TrueNamer)
    {
        string path = Path.Combine(Application.persistentDataPath, NameFile);

        if (File.Exists(path))
        {
            string json2 = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<LearnDataPixel>(json2);
            if (data == null)
            {
                data = new LearnDataPixel();
            }
            else
            {
                // Убеждаемся, что все списки инициализированы
                if (data.zero == null) data.zero = new List<FloatMass>();
                if (data.one == null) data.one = new List<FloatMass>();
                if (data.two == null) data.two = new List<FloatMass>();
                if (data.three == null) data.three = new List<FloatMass>();
                if (data.four == null) data.four = new List<FloatMass>();
                if (data.five == null) data.five = new List<FloatMass>();
                if (data.six == null) data.six = new List<FloatMass>();
                if (data.seven == null) data.seven = new List<FloatMass>();
                if (data.eight == null) data.eight = new List<FloatMass>();
                if (data.nine == null) data.nine = new List<FloatMass>();
            }
        }
        else
        {
            data = new LearnDataPixel();
        }
        List<float> pixelsCopy = new List<float>(Pixsels);
        FloatMass floatMass = new FloatMass { FL = pixelsCopy };

        switch (TrueNamer)
        {
            case 0: data.zero.Add(floatMass); break;
            case 1: data.one.Add(floatMass); break;
            case 2: data.two.Add(floatMass); break;
            case 3: data.three.Add(floatMass); break;
            case 4: data.four.Add(floatMass); break;
            case 5: data.five.Add(floatMass); break;
            case 6: data.six.Add(floatMass); break;
            case 7: data.seven.Add(floatMass); break;
            case 8: data.eight.Add(floatMass); break;
            case 9: data.nine.Add(floatMass); break;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Данные сохранены в {path}, всего: {data.zero.Count + data.one.Count + data.two.Count + data.three.Count + data.four.Count + data.five.Count + data.six.Count + data.seven.Count + data.eight.Count + data.nine.Count} образцов");
    }

    public void TeaTurPix()
    {
        Texture2D source = rawImage.texture as Texture2D;

        byte[] srcData = source.GetRawTextureData(); // 117 строка
        byte[] dstData = new byte[newWidth * newHeight * 4]; // RGBA32

        float ratioX = (float)source.width / newWidth;
        float ratioY = (float)source.height / newHeight;
        int srcStride = source.width * 4;

        for (int y = 0; y < newHeight; y++)
        {
            int srcY = Mathf.FloorToInt(y * ratioY) * srcStride;
            int dstY = y * newWidth * 4;

            for (int x = 0; x < newWidth; x++)
            {
                int srcX = Mathf.FloorToInt(x * ratioX) * 4;
                int srcIndex = srcY + srcX;
                int dstIndex = dstY + x * 4;

                // Копируем 4 байта (RGBA)
                dstData[dstIndex] = srcData[srcIndex];
                dstData[dstIndex + 1] = srcData[srcIndex + 1];
                dstData[dstIndex + 2] = srcData[srcIndex + 2];
                dstData[dstIndex + 3] = srcData[srcIndex + 3];
            }
        }

        Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        result.LoadRawTextureData(dstData);
        result.Apply();

        foreach (var i in result.GetPixels())
            Pixsels.Add(i.a);
    }

}

[System.Serializable]
public class LearnDataPixel
{
    public List<FloatMass> zero = new List<FloatMass>();
    public List<FloatMass> one = new List<FloatMass>();
    public List<FloatMass> two = new List<FloatMass>();
    public List<FloatMass> three = new List<FloatMass>();
    public List<FloatMass> four = new List<FloatMass>();
    public List<FloatMass> five = new List<FloatMass>();
    public List<FloatMass> six = new List<FloatMass>();
    public List<FloatMass> seven = new List<FloatMass>();
    public List<FloatMass> eight = new List<FloatMass>();
    public List<FloatMass> nine = new List<FloatMass>();
}

[System.Serializable]
public class FloatMass
{
    public List<float> FL;
}


/*
 // Update is called once per frame
 void Update()
 {
     Pixsels.Clear(); // ← ОЧИЩАЕМ!
     TeaTurPix();

     if (Input.GetKeyDown(KeyCode.Alpha0)) SaveLernData(0);
     if (Input.GetKeyDown(KeyCode.Alpha1)) SaveLernData(1);
     if (Input.GetKeyDown(KeyCode.Alpha2)) SaveLernData(2);
     if (Input.GetKeyDown(KeyCode.Alpha3)) SaveLernData(3);
     if (Input.GetKeyDown(KeyCode.Alpha4)) SaveLernData(4);
     if (Input.GetKeyDown(KeyCode.Alpha5)) SaveLernData(5);
     if (Input.GetKeyDown(KeyCode.Alpha6)) SaveLernData(6);
     if (Input.GetKeyDown(KeyCode.Alpha7)) SaveLernData(7);
     if (Input.GetKeyDown(KeyCode.Alpha8)) SaveLernData(8);
     if (Input.GetKeyDown(KeyCode.Alpha9)) SaveLernData(9);
 }

 public void SaveLernData(int TrueNamer)
 {
     string path = Path.Combine(Application.persistentDataPath, NameFile);

     LearnDataPixel data;
     if (File.Exists(path))
     {
         string json2 = File.ReadAllText(path);
         data = JsonUtility.FromJson<LearnDataPixel>(json2);
         if (data == null)
         {
             data = new LearnDataPixel();
         }
         else
         {
             // Убеждаемся, что все списки инициализированы
             if (data.zero == null) data.zero = new List<FloatMass>();
             if (data.one == null) data.one = new List<FloatMass>();
             if (data.two == null) data.two = new List<FloatMass>();
             if (data.three == null) data.three = new List<FloatMass>();
             if (data.four == null) data.four = new List<FloatMass>();
             if (data.five == null) data.five = new List<FloatMass>();
             if (data.six == null) data.six = new List<FloatMass>();
             if (data.seven == null) data.seven = new List<FloatMass>();
             if (data.eight == null) data.eight = new List<FloatMass>();
             if (data.nine == null) data.nine = new List<FloatMass>();
         }
     }
     else
     {
         data = new LearnDataPixel();
     }
     List<float> pixelsCopy = new List<float>(Pixsels);
     FloatMass floatMass = new FloatMass { FL = pixelsCopy };

     switch (TrueNamer)
     {
         case 0: data.zero.Add(floatMass); break;
         case 1: data.one.Add(floatMass); break;
         case 2: data.two.Add(floatMass); break;
         case 3: data.three.Add(floatMass); break;
         case 4: data.four.Add(floatMass); break;
         case 5: data.five.Add(floatMass); break;
         case 6: data.six.Add(floatMass); break;
         case 7: data.seven.Add(floatMass); break;
         case 8: data.eight.Add(floatMass); break;
         case 9: data.nine.Add(floatMass); break;
     }

     string json = JsonUtility.ToJson(data, true);
     File.WriteAllText(path, json);
     Debug.Log($"Данные сохранены в {path}, всего: {data.zero.Count + data.one.Count + data.two.Count + data.three.Count + data.four.Count + data.five.Count + data.six.Count + data.seven.Count + data.eight.Count + data.nine.Count} образцов");
 }

 public void TeaTurPix()
 {
     Texture2D source = rawImage.texture as Texture2D;

     byte[] srcData = source.GetRawTextureData(); // 117 строка
     byte[] dstData = new byte[newWidth * newHeight * 4]; // RGBA32

     float ratioX = (float)source.width / newWidth;
     float ratioY = (float)source.height / newHeight;
     int srcStride = source.width * 4;

     for (int y = 0; y < newHeight; y++)
     {
         int srcY = Mathf.FloorToInt(y * ratioY) * srcStride;
         int dstY = y * newWidth * 4;

         for (int x = 0; x < newWidth; x++)
         {
             int srcX = Mathf.FloorToInt(x * ratioX) * 4;
             int srcIndex = srcY + srcX;
             int dstIndex = dstY + x * 4;

             // Копируем 4 байта (RGBA)
             dstData[dstIndex] = srcData[srcIndex];
             dstData[dstIndex + 1] = srcData[srcIndex + 1];
             dstData[dstIndex + 2] = srcData[srcIndex + 2];
             dstData[dstIndex + 3] = srcData[srcIndex + 3];
         }
     }

     Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
     result.LoadRawTextureData(dstData);
     result.Apply();

     foreach (var i in result.GetPixels())
         Pixsels.Add(i.a);
 }*/
