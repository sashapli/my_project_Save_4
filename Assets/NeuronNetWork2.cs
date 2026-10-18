using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NeuronNetWork2 : MonoBehaviour
{
    public List<Neuron2> neuron;
    public List<NeuronEnd2> Nend;
    public int CountNeuron = 4;
    public int CountEndNeuron = 4;
    public float g1, g2, g3, g4;
    public float v11, v12, v13, v14, v21, v22, v23, v24, v31, v32, v33, v34, v41, v42, v43, v44;
    public float s1, s2, s3, s4;
    public TMP_Text t1, t2, t3, t4;
    public int CurentZona;
    public float Error01;
    public float learningRate = 0.1f;
    private float lastX;
    private float lastY;

    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;

    private void Start()
    {
        neuron = new List<Neuron2>();
        Nend = new List<NeuronEnd2>();
        for (var i = 0; i < CountNeuron; i++)
            neuron.Add(new Neuron2() { weight1 = Random.value *2 -1, weight2 = Random.value * 2 - 1, bias = Random.value * 2 - 1, });
        for (var i = 0; i < CountEndNeuron; i++)
            Nend.Add(new NeuronEnd2() { we1 = Random.value * 2 - 1, we2 = Random.value * 2 - 1, we3 = Random.value * 2 - 1, we4 = Random.value * 2 - 1, bias = Random.value * 2 - 1, });
    }

    public void UpdateDataNeuron(float x, float y)
    {
        Debug.Log("координаты сигмойды. X - " + x + "Y - " + y);      
        lastX = x;
        lastY = y;

        g1 = x * neuron[0].weight1 + y * neuron[0].weight2 + neuron[0].bias;
        g2 = x * neuron[1].weight1 + y * neuron[1].weight2 + neuron[1].bias;
        g3 = x * neuron[2].weight1 + y * neuron[2].weight2 + neuron[2].bias;
        g4 = x * neuron[3].weight1 + y * neuron[3].weight2 + neuron[3].bias;

        Vector3[] points = new Vector3[30];
        for (int i = 0; i < 30; i++)
        {
            float t = i / (float)30;
            float x2 = t * 10f; // Диапазон по X
            float y2 = 1f / (1f + Mathf.Exp(x * neuron[0].weight1 + y * neuron[0].weight2 + neuron[0].bias)); // Ваша формула: y = f(x)
            points[i] = new Vector3(x, y, 0);
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        Vector3[] points2 = new Vector3[30];
        for (int i = 0; i < 30; i++)
        {
            float t = i / (float)30;
            float x2 = t * 10f; // Диапазон по X
            float y2 = g2; // Ваша формула: y = f(x)
            points2[i] = new Vector3(x, y, 0);
        }
        lineRenderer1.positionCount = points2.Length;
        lineRenderer1.SetPositions(points2);

        Vector3[] points3 = new Vector3[30];
        for (int i = 0; i < 30; i++)
        {
            float t = i / (float)30;
            float x2 = t * 10f; // Диапазон по X
            float y2 = g3; // Ваша формула: y = f(x)
            points3[i] = new Vector3(x, y, 0);
        }
        lineRenderer2.positionCount = points3.Length;
        lineRenderer2.SetPositions(points3);

        Vector3[] points4 = new Vector3[30];
        for (int i = 0; i < 30; i++)
        {
            float t = i / (float)30;
            float x2 = t * 10f; // Диапазон по X
            float y2 = g4; // Ваша формула: y = f(x)
            points4[i] = new Vector3(x, y, 0);
        }
        lineRenderer3.positionCount = points4.Length;
        lineRenderer3.SetPositions(points4);

        s1 = g1 * Nend[0].we1 + g2 * Nend[0].we2 + g3 * Nend[0].we3 + g4 * Nend[0].we4 + Nend[0].bias;
        s2 = g1 * Nend[1].we1 + g2 * Nend[1].we2 + g3 * Nend[1].we3 + g4 * Nend[1].we4 + Nend[1].bias;
        s3 = g1 * Nend[2].we1 + g2 * Nend[2].we2 + g3 * Nend[2].we3 + g4 * Nend[2].we4 + Nend[2].bias;
        s4 = g1 * Nend[3].we1 + g2 * Nend[3].we2 + g3 * Nend[3].we3 + g4 * Nend[3].we4 + Nend[3].bias;

        float exp1 = Mathf.Exp(s1);
        float exp2 = Mathf.Exp(s2);
        float exp3 = Mathf.Exp(s3);
        float exp4 = Mathf.Exp(s4);

        float sumExp = exp1 + exp2 + exp3 + exp4;

        s1 = exp1 / sumExp;
        s2 = exp2 / sumExp;
        s3 = exp3 / sumExp;
        s4 = exp4 / sumExp;

        // --- Вывод в консоль для наглядности ---
        Debug.Log($"Скрытые: g1={g1:F4}, g2={g2:F4}, g3={g3:F4}, g4={g4:F4}");
        Debug.Log($"Вероятности: 1={s1:F4}, 2={s2:F4}, 3={s3:F4}, 4={s4:F4}");

        t1.text = "" + s1;
        t2.text = "" + s2;
        t3.text = "" + s3;
        t4.text = "" + s4;

        if (x > 0 && y > 0)
            CurentZona = 1;
        if (x < 0 && y > 0)
            CurentZona = 2;
        if (x < 0 && y < 0)
            CurentZona = 3;
        if (x > 0 && y < 0)
            CurentZona = 4;

        switch(CurentZona)
        {
            case 1: Error01 = 1 - s1; break;
            case 2: Error01 = 1 - s2; break;
            case 3: Error01 = 1 - s3; break;
            case 4: Error01 = 1 - s4; break;
        }
        Debug.Log("ИИ ошибается на - " + Error01);
        Train();
    }

    // ==============================================
    //  МЕТОД ОБУЧЕНИЯ (САМОЕ ВАЖНОЕ!)
    // ==============================================
    public void Train()
    {
        float[] target = { 0, 0, 0, 0 };
        target[CurentZona - 1] = 1;
        float[] error = new float[4];
        error[0] = target[0] - s1;
        error[1] = target[1] - s2;
        error[2] = target[2] - s3;
        error[3] = target[3] - s4;

        Nend[0].we1 += error[0] * g1 * learningRate;
        Nend[0].we2 += error[0] * g2 * learningRate;
        Nend[0].we3 += error[0] * g3 * learningRate;
        Nend[0].we4 += error[0] * g4 * learningRate;
        Nend[0].bias += error[0] * learningRate;

        Nend[1].we1 += error[1] * g1 * learningRate;
        Nend[1].we2 += error[1] * g2 * learningRate;
        Nend[1].we3 += error[1] * g3 * learningRate;
        Nend[1].we4 += error[1] * g4 * learningRate;
        Nend[1].bias += error[1] * learningRate;

        Nend[2].we1 += error[2] * g1 * learningRate;
        Nend[2].we2 += error[2] * g2 * learningRate;
        Nend[2].we3 += error[2] * g3 * learningRate;
        Nend[2].bias += error[2] * learningRate;

        Nend[3].we1 += error[3] * g1 * learningRate;
        Nend[3].we2 += error[3] * g2 * learningRate;
        Nend[3].we3 += error[3] * g3 * learningRate;
        Nend[3].we4 += error[3] * g4 * learningRate;
        Nend[3].bias += error[3] * learningRate;

        float[] erroe2 = new float[4];

        erroe2[0] = (error[0] * Nend[0].we1 +
                                    error[1] * Nend[1].we1 +
                                    error[2] * Nend[2].we1 +
                                    error[3] * Nend[3].we1) * g1 * (1 - g1);

        erroe2[1] = (error[0] * Nend[0].we2 +
                                    error[1] * Nend[1].we2 +
                                    error[2] * Nend[2].we2 +
                                    error[3] * Nend[3].we2) * g2 * (1 - g2);

        erroe2[2] = (error[0] * Nend[0].we3 +
                                    error[1] * Nend[1].we3 +
                                    error[2] * Nend[2].we3 +
                                    error[3] * Nend[3].we3) * g3 * (1 - g3);

        erroe2[3] = (error[0] * Nend[0].we4 +
                                    error[1] * Nend[1].we4 +
                                    error[2] * Nend[2].we4 +
                                    error[3] * Nend[3].we4) * g4 * (1 - g4);

        neuron[0].weight1 += erroe2[0] * lastX * learningRate;
        neuron[1].weight1 += erroe2[1] * lastX * learningRate;
        neuron[2].weight1 += erroe2[2] * lastX * learningRate;
        neuron[3].weight1 += erroe2[3] * lastX * learningRate;

        neuron[0].weight2 += erroe2[0] * lastY * learningRate;
        neuron[1].weight2 += erroe2[1] * lastY * learningRate;
        neuron[2].weight2 += erroe2[2] * lastY * learningRate;
        neuron[3].weight2 += erroe2[3] * lastY * learningRate;

        neuron[0].bias += erroe2[0] * learningRate;
        neuron[1].bias += erroe2[1] * learningRate;
        neuron[2].bias += erroe2[2] * learningRate;
        neuron[3].bias += erroe2[3] * learningRate;
    }
}

public class Neuron2
{
    public float weight1;
    public float weight2;
    public float bias;
}
public class NeuronEnd2
{
    public float we1, we2, we3, we4;
    public float bias;
}