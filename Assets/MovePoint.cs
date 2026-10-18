using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour
{
    public NeuronNetWork2 neuronNetWork2;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            neuronNetWork2.UpdateDataNeuron(transform.position.x, transform.position.y);
        }
    }
}
