using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBarChartTest : MonoBehaviour
{
    public RadialBarChart barchart;
    public int nbars;
    public float max;
    public float min;

    List<float> data = new List<float>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            data = new List<float>();
            for(int i = 0; i < nbars; i++)
            {
                data.Add(Random.Range(min, max));
            }

            barchart.ClearVisualisation();
            barchart.ClearAxisLines();
            barchart.InitiateVisualisation();
            barchart.CreateVisualisation(data);
            barchart.CreateAxisLines();
        }

       

    }
}
