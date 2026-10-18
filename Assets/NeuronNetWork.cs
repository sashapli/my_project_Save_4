using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronNetWork : MonoBehaviour
{
    public float ruble;
    private float learningRate = 0.1f; // Скорость обучения
    public Neuron neuron;

    private void Start()
    {
        neuron = new Neuron();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            UpdateNeuron();
        if (Input.GetKeyDown(KeyCode.L))
            Learn();
    }

    public void UpdateNeuron()
    {
        Debug.Log("ввывод: " + neuron.reaction(ruble));
        if(neuron.weight > ruble * 77.06)
            neuron.weight -= learningRate;
        else if(neuron.weight < ruble * 77.06)
            neuron.weight += learningRate;
    }

    public void Learn()
    {
        for(var i =0; i < 1000; i++)
        {
            if (neuron.weight > ruble * 77.06)
                neuron.weight -= learningRate;
            else if (neuron.weight < ruble * 77.06)
                neuron.weight += learningRate;
        }
    }
}

public class Neuron
{
    public float weight = 1;

    public float reaction(float f)
    {
        return f * weight;
    }
}
