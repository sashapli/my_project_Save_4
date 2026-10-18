 using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Newtonsoft.Json;
public class NeuronNetWork3 : MonoBehaviour
{
    public List<float> Pixsels;
    public List<float> AllActivLayer1;
    public List<float> AllActivLayer2;
    public List<float> answer;
    public float learningRate = 0.001f;
    public NeuronData LayerHe1;
    public NeuronData LayerHe2;
    public NeuronData LayerExit;

    public TMP_Text[] Text90;
    public bool isLearn = false;

    public string NameFile = "AI3Data_Layer.JSON";

    public LearnDataPixel data = new LearnDataPixel();

    public paintSizeAll paintSizeAll;
    public int newWidth = 28;
    public int newHeight = 28;

    public SmartVisionSystemAi4 CVAI4;
    public bool isProcessing;

    public void Start()
    {
        string path = Path.Combine(Application.persistentDataPath, NameFile);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DataFormatJsom formatJsom = JsonUtility.FromJson<DataFormatJsom>(json);
            if (formatJsom.layer1 != null && formatJsom.layer1.neuronBaisData != null)
            {
                LayerHe1 = formatJsom.layer1;
                LayerHe2 = formatJsom.layer2;
                LayerExit = formatJsom.layer3;
            }
            else
                InitializeNetwork();

        }
        else
            InitializeNetwork();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetDataJson();
        if(Input.GetKeyDown(KeyCode.L))
        {
            isLearn = !isLearn;
            if (!isLearn) 
            {
                paintSizeAll.UpdateTextur2D(); 
                        string path = Path.Combine(Application.persistentDataPath, NameFile);
        DataFormatJsom data = new DataFormatJsom
        {
            layer1 = LayerHe1,
            layer2 = LayerHe2,
            layer3 = LayerExit
        };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
            }
        }

        if (isLearn)
        {
            if (isProcessing) return;
            int RandomNamder = Random.Range(0, 10);
            CVAI4.ThinkCV(RandomNamder, true);
        }
        else
        {
            CVAI4.ThinkCV(99, true);
        }
    }

    void ResetDataJson()
    {
        Debug.Log("reset Json");
        string path = Path.Combine(Application.persistentDataPath, NameFile);
        if (File.Exists(path))
        {
            InitializeNetwork();

            DataFormatJsom data = new DataFormatJsom
            {
                layer1 = LayerHe1,
                layer2 = LayerHe2,
                layer3 = LayerExit
            };
            string newJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, newJson);
        }
    }

    public List<float> ThinkAi(int trueNamder, bool isLearn, List<float> pix)
    {
        AllActivLayer1.Clear();
        AllActivLayer2.Clear();
        answer.Clear();

        Pixsels = pix;

        for (var i = 0; i < LayerHe1.neuronBaisData.Length; i++) // строка 130
        {
            LayerHe1.neuronBaisData[i].Activation(Pixsels, "ReLU");
            AllActivLayer1.Add(LayerHe1.neuronBaisData[i].Ativation);
        }

        for (var i = 0; i < LayerHe2.neuronBaisData.Length; i++)
        {
            LayerHe2.neuronBaisData[i].Activation(AllActivLayer1, "ReLU");
            AllActivLayer2.Add(LayerHe2.neuronBaisData[i].Ativation);
        }

        for (var i = 0; i < LayerExit.neuronBaisData.Length; i++)
        {
            LayerExit.neuronBaisData[i].Activation(AllActivLayer2, "Linear");
            answer.Add(LayerExit.neuronBaisData[i].Ativation);
        }
        answer = Softmax(answer);

        for(var i =0; i <10; i++)
            Text90[i].text = (int)(answer[i] * 100) + "%";
        return answer;

        if (trueNamder != 99)
            LearnAi(trueNamder);
        isProcessing = false;
    }

    public void LearnAi(int NamberTrue)
    {
        float[] target = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        float[] error = new float[10];
        target[NamberTrue] = 1;
        for (var i = 0; i < 10; i++)
            error[i] = target[i] - answer[i];

        // обучение выходного слоя

        for (var i = 0; i < LayerExit.neuronBaisData.Length; i++) {
            LayerExit.neuronBaisData[i].bias += learningRate * error[i];
            for (var i2 = 0; i2 < LayerExit.neuronBaisData[i].weight.Length; i2++)
                LayerExit.neuronBaisData[i].weight[i2] += learningRate * error[i] * AllActivLayer2[i2];
        }
        // обучение 2 слоя

        float[] error2 = new float[16];
        for(var i = 0; i < error2.Length; i++)
        {
            float vinaSum = 0;
            for (var i2 = 0; i2 < 10; i2++)
            {
                vinaSum += error[i2] * LayerExit.neuronBaisData[i2].weight[i];
            }
            error2[i] = vinaSum * (AllActivLayer2[i] > 0 ? 1 : 0);
        }

        for (var i = 0; i < LayerHe2.neuronBaisData.Length; i++) {
            LayerHe2.neuronBaisData[i].bias += learningRate * error2[i];
            for (var i2 = 0; i2 < LayerHe2.neuronBaisData[i].weight.Length; i2++)
                LayerHe2.neuronBaisData[i].weight[i2] += learningRate * error2[i] * AllActivLayer1[i2]; }

        // обучение 1 слоя

        float[] error3 = new float[16];
        for (var i = 0; i < error3.Length; i++)
        {
            float vinaSum = 0;
            for (var i2 = 0; i2 < 16; i2++)
            {
                vinaSum += error2[i2] * LayerHe2.neuronBaisData[i2].weight[i];
            }
            error3[i] = vinaSum * (AllActivLayer1[i] > 0 ? 1 : 0);
        }

        for (var i = 0; i < LayerHe1.neuronBaisData.Length; i++)
        {
            LayerHe1.neuronBaisData[i].bias += learningRate * error3[i];
            for (var i2 = 0; i2 < LayerHe1.neuronBaisData[i].weight.Length; i2++)
                LayerHe1.neuronBaisData[i].weight[i2] += learningRate * error3[i] * Pixsels[i2];
        }
    }

    public static List<float> Softmax(List<float> input)
    {
        float max = input[0];
        for(var i = 1; i < input.Count; i++)
            if (input[i] > max)
                max = input[i];

        float[] ExpVaulems = new float[input.Count];
        float sum = 0f;
        for (var i = 0; i < input.Count; i++)
        {
            ExpVaulems[i] = Mathf.Exp(input[i] - max);
            sum += ExpVaulems[i];
        }

        float[] result = new float[input.Count];
        for (var i = 0; i < input.Count; i++)
        {
            result[i] = ExpVaulems[i] / sum;
        }

        return result.ToList();
    }

    public void InitializeNetwork()
    {
        LayerHe1 = new NeuronData();
        LayerHe2 = new NeuronData();
        LayerExit = new NeuronData();

        LayerHe1.neuronBaisData = new NeuronBais[16];
        LayerHe2.neuronBaisData = new NeuronBais[16];
        LayerExit.neuronBaisData = new NeuronBais[10];

        for (var i = 0; i < 128; i++)
        {
            NeuronBais bais = new NeuronBais();
            bais.StartNeuron(784);
            LayerHe1.neuronBaisData[i] = bais;

            NeuronBais bais2 = new NeuronBais();
            bais2.StartNeuron(128);
            LayerHe2.neuronBaisData[i] = bais2;
        }

        for (var i = 0; i < 10; i++)
        {
            NeuronBais bais = new NeuronBais();
            bais.StartNeuron(16);
            LayerExit.neuronBaisData[i] = bais;
        }
    }

}
[System.Serializable]
public class NeuronBais
{
    public float[] weight;
    public float bias;
    public int previousCountNeuron;
    public float Ativation;

    public void StartNeuron(int PCN)
    {
        weight = new float[PCN];
        float shft = Mathf.Sqrt(2f / PCN); 
        for (var i = 0; i < PCN; i++)
            weight[i] = Random.Range(-shft, shft);
        bias = 0.01f; ;
    }

    public float Activation(List<float> PAN, string ActivationMode)
    {
        float sum = bias;
        for (var i = 0; i < PAN.Count; i++)
            sum += weight[i] * PAN[i];

        switch (ActivationMode)
        {
            case "Linear": Ativation = sum; return sum;
            case "Sigmoid": Ativation = 1f / (1f + Mathf.Exp(-sum)); return 1f / (1f + Mathf.Exp(-sum));
            case "ReLU":Ativation = Mathf.Max(0, sum); return Mathf.Max(0,sum);
            default:
                return sum;
        }
    }
}

[System.Serializable]
public class NeuronData
{
    public NeuronBais[] neuronBaisData;
}

[System.Serializable]
public class DataFormatJsom 
{
    public NeuronData layer1;
    public NeuronData layer2;
    public NeuronData layer3;
}
[System.Serializable]
public class SpriteListLearn
{
    public List<Sprite> sprites = new List<Sprite>();
}