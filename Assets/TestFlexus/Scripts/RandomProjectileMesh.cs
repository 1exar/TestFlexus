using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RandomProjectileMesh : MonoBehaviour
{
    [Header("Mesh Settings")]
    [SerializeField] private int latitudeSegments = 12;   // по высоте
    [SerializeField] private int longitudeSegments = 18;  // по ширине
    [SerializeField] private float length = 1.5f;         // длина снаряда
    [SerializeField] private float radius = 0.25f;        // толщина
    [SerializeField] private float noiseStrength = 0.05f; // случайный шум

    private Mesh mesh;

    void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "RandomProjectile";

        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        int vertIndex = 0;
        int triIndex = 0;

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float a1 = Mathf.PI * lat / latitudeSegments; // 0..pi
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float a2 = 2 * Mathf.PI * lon / longitudeSegments; // 0..2pi
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                // базовая форма – вытянутый эллипсоид
                float x = radius * sin1 * cos2;
                float y = radius * cos1 * 0.5f + (lat / (float)latitudeSegments - 0.5f) * length;
                float z = radius * sin1 * sin2;

                // добавим немного шума для "красоты"
                Vector3 offset = new Vector3(
                    Random.Range(-noiseStrength, noiseStrength),
                    Random.Range(-noiseStrength, noiseStrength),
                    Random.Range(-noiseStrength, noiseStrength)
                );

                vertices[vertIndex] = new Vector3(x, y, z) + offset;
                uvs[vertIndex] = new Vector2((float)lon / longitudeSegments, (float)lat / latitudeSegments);

                if (lat < latitudeSegments && lon < longitudeSegments)
                {
                    int current = vertIndex;
                    int next = current + longitudeSegments + 1;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next + 1;
                    triangles[triIndex++] = next;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = current + 1;
                    triangles[triIndex++] = next + 1;
                }

                vertIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
