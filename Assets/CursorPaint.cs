using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorPaint : MonoBehaviour
{
    public Slider slider;
    public float MaxPaint;
    public bool ModePaint;
    Collider2D[] hitcolli;
    private void Start()
    {
        hitcolli = FindObjectsOfType<BoxCollider2D>();
    }

    void Update()
    {
        transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,0);

        if (Input.GetMouseButton(0))
        {
            Collider2D[] hitcolli = Physics2D.OverlapCircleAll(transform.position, 0.3f + (MaxPaint * slider.value));
            foreach (var i in hitcolli)
            {
                if (i.name == "Pixsel")
                {
                    Debug.Log("каснулся пискеля под именем - " + i.gameObject.name);
                    float a = i.gameObject.GetComponent<Image>().color.a + (ModePaint ? 0.15f: - 0.15f);
                    i.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Clamp(a,0,1));
                }
                else
                    Debug.Log("каснулся обекта под именем - " + i.gameObject.name);
            }
        }
    }

    public void ResetPaint()
    {
        foreach (var i in hitcolli)
            i.gameObject.GetComponent<Image>().color = new Color(0,0,0,0);
    }

    public void LasticPaint() => ModePaint = !ModePaint;

}
