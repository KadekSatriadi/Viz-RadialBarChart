using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadialBarChart : MonoBehaviour
{
    [Header("Visualisation")]
    public float radius;
    public float tickness;
    [Range(0, 360)]
    public float angularSize = 0;
    [Range(0, 360)]
    public float rotation = 0;
    public float angularMargin;
    public Color color;

    [Header("Axis settings")]
    public int axisLinesCount = 3;
    public float axisLineWidth = 0.005f;
    public Color axisLineColor;

    private Transform visualisation;
    private Transform axisLine;
    private float angularWidth;
    private float maxData;
    private float minData;
    private float eucWidth;
    private int axisLineResolution = 200;
    private List<Transform> bars = new List<Transform>();
    // Start is called before the first frame update


    public void InitiateVisualisation()
    {
        visualisation = new GameObject("Visualisation").transform;
        visualisation.position = transform.position;
        visualisation.SetParent(transform);

        axisLine = new GameObject("AxisLine").transform;
        axisLine.position = transform.position;
        axisLine.SetParent(transform);
    }

    public void ClearVisualisation()
    {
        if(visualisation) Destroy(visualisation.gameObject);
    }

    public void ClearAxisLines()
    {
        if (axisLine) Destroy(axisLine.gameObject);
    }

    public List<Transform> GetAllBars()
    {
        return bars;
    }

    public void CreateAxisLines()
    {
        for(int i = 1; i <= axisLinesCount; i++)
        {
            GameObject g = new GameObject("axis_line" + i);
            LineRenderer line = g.AddComponent<LineRenderer>();
            line.widthMultiplier = axisLineWidth;
            line.useWorldSpace = false;
            line.material.color = axisLineColor;
            line.loop = true;
            List<Vector3> points = GetCircularPoints(radius + (tickness / axisLinesCount) * i, g.transform.position, axisLineResolution);
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
            line.transform.position = axisLine.position;
            line.transform.SetParent(axisLine);
        }

        axisLine.transform.localRotation = Quaternion.Euler(axisLine.localRotation.x, axisLine.localRotation.y, rotation);
    }

    private List<Vector3> GetCircularPoints(float radius, Vector3 center,  int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i < resolution; i++)
        {
            Vector2 pos = PolarToCartesian(radius, center, (angularSize / resolution) * i);
            points.Add(pos);
        }

        return points;
    }

    public void CreateVisualisation(List<float> data)
    {
        if (data.Count == 0) return;
        bars = new List<Transform>();

        maxData = data.Max();
        minData = data.Min();
        angularWidth = (angularSize / data.Count) - angularMargin * 0.5f;
        eucWidth = GetArcLength(angularWidth, radius);

        for (int i = 0; i < data.Count; i++)
        {
            CreateBar(data[i], i);
        }

        //rotation of entire visualisation
        visualisation.transform.localRotation = Quaternion.Euler(visualisation.localRotation.x, visualisation.localRotation.y, rotation);
    }

    void CreateBar(float data, int index)
    {
        //gameobject
        GameObject g = new GameObject("bar_d" + data + "_idx" + index);
        g.AddComponent<MeshFilter>();
        g.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/TransparentColor"));
        g.GetComponent<MeshRenderer>().material.color = color;
        g.transform.position = visualisation.position;
        g.transform.SetParent(visualisation);

        //bar
        float height = Mathf.Lerp(0.001f, tickness, (data - minData) / (maxData - minData));
        Mesh mesh = CreateProceduralBox(0.01f, eucWidth, height);
        g.GetComponent<MeshFilter>().mesh = mesh;

        //position
        float angularPos = angularWidth * index + angularMargin * 0.5f * index;
        Vector2 position = PolarToCartesian(radius, new Vector2(visualisation.position.x, visualisation.position.y), angularPos);
        Vector3 wordPos = new Vector3(position.x, position.y, visualisation.position.z);
        wordPos -= (visualisation.position - wordPos).normalized * 0.5f * height;
        g.transform.localPosition = visualisation.InverseTransformPoint(wordPos);

        //rotation
        g.transform.LookAt(visualisation.transform.position);

        //fix middle bar look at error
        if((int) (g.transform.rotation.eulerAngles.y) == 0f)
        {
            Vector3 eulers = g.transform.rotation.eulerAngles;
            g.transform.rotation = Quaternion.Euler(eulers.x, 90, eulers.z);
        }

        //add to list
        bars.Add(g.transform);
    }

    float GetArcLength(float angle, float radius)
    {
        return (2 * Mathf.PI * radius) * (angle / 360f);
    }

    Vector2 PolarToCartesian(float radius, Vector2 center, float degree)
    {
        float x = radius * Mathf.Cos(degree * Mathf.Deg2Rad) + center.x;
        float y = radius * Mathf.Sin(degree * Mathf.Deg2Rad) + center.y;

        return new Vector2(x, y);
    }

    Mesh CreateProceduralBox(float length, float width, float height)
    {
    
        #region Vertices
        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
        {
	// Bottom
	p0, p1, p2, p3,
 
	// Left
	p7, p4, p0, p3,
 
	// Front
	p4, p5, p1, p0,
 
	// Back
	p6, p7, p3, p2,
 
	// Right
	p5, p6, p2, p1,
 
	// Top
	p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	// Bottom
	_11, _01, _00, _10,
 
	// Left
	_11, _01, _00, _10,
 
	// Front
	_11, _01, _00, _10,
 
	// Back
	_11, _01, _00, _10,
 
	// Right
	_11, _01, _00, _10,
 
	// Top
	_11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    Mesh CreateProceduralPlane(float length, float width)
    {
        Mesh mesh = new Mesh();

        int resX = 2; // 2 minimum
        int resZ = 2;

        #region Vertices		
        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < normales.Length; n++)
            normales[n] = Vector3.up;
        #endregion

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < nbFaces; face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }
}
