using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBarChartTestDataset : MonoBehaviour
{
    public RadialBarChart barchart;
    public SQLiteDatabase database;
    public string labelQuantitative;
    public string condition;
    public bool realtimeUpdate = false;
    List<float> data = new List<float>();
    // Update is called once per frame

    private void Start()
    {
        if(condition.Length > 0)
        {
            data = database.GetFloatRecordsByField(labelQuantitative, condition);
        }
        else
        {
            data = database.GetFloatRecordsByField(labelQuantitative);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {          
            UpdateChart();
        }

        if (realtimeUpdate)
        {
            UpdateChart();
        }

    }

    private void UpdateChart()
    {
        barchart.ClearVisualisation();
        barchart.ClearAxisLines();
        barchart.InitiateVisualisation();
        barchart.CreateVisualisation(data);
        barchart.CreateAxisLines();
    }
}
