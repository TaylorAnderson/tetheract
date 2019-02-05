using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RoadCreator : MonoBehaviour {

	[Range(0.05f, 1.5f)]
    public float spacing = 1;

    public float roadWidth = 1;
    public bool autoUpdate;
    public float tiling = 1;
    public EdgeCollider2D innerEdgeCollider;
    public EdgeCollider2D outsideEdgeCollider;

    private List<GameObject> stripes = new List<GameObject>();

    private List<Vector2> insidePoints = new List<Vector2>();
    private List<Vector2> outsidePoints = new List<Vector2>();

    private Path path;
    private Vector2[] points;
    private MeshFilter meshFilter;
    private void OnEnable() {
    }
    public void Start() {

        UpdateRoad();
    }
    public void UpdateRoad() {
        path = GetComponent<PathCreator>().path;
        points = path.CalculateEvenlySpacedPoints(spacing);
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateRoadMesh(points, path.IsClosed);
        int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * 0.05f);
        GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);
    }

    Mesh CreateRoadMesh (Vector2[] points, bool isClosed) {
        Vector3[] verts = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[verts.Length];
        int numTris = 2 * (points.Length - 1) + (isClosed ? 2 : 0);
        int[] tris = new int[numTris*3];
        int vertIndex = 0;
        int triIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] -= path.center;
        }

        for (int i = 0; i < points.Length; i++)
		{
            
            Vector2 forward = Vector2.zero;
			if (i < points.Length-1 || isClosed) {
                forward += points[(i + 1)%points.Length] - points[i];
            }
			if (i > 0) {
                forward += points[i] - points[(i - 1 + points.Length)%points.Length];
            }

            forward.Normalize();
			Vector2 left = new Vector2(-forward.y, forward.x);

            verts[vertIndex] = points[i] + left * roadWidth * 0.5f;
            verts[vertIndex + 1] = points[i] - left * roadWidth * 0.5f;


            float completionPercent = i / (float)(points.Length - 1);
            float v = 1 - Mathf.Abs(2 * completionPercent - 1);
            uvs[vertIndex] = new Vector2(0, v);
            uvs[vertIndex+1] = new Vector2(1, v);

            if (i < points.Length - 1 || isClosed) {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 2] = vertIndex + 1;

				tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = (vertIndex + 2) % verts.Length;
                tris[triIndex + 5] = (vertIndex + 3) % verts.Length;
            }
            vertIndex += 2;
            triIndex += 6;
        }



        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;


        
        return mesh;
    }

	public void GenerateCollider() {

        Mesh mesh = meshFilter.sharedMesh;
        insidePoints = new List<Vector2>();
        outsidePoints = new List<Vector2>();
        for (int i = 0; i < mesh.vertices.Length; i+=64) {
            insidePoints.Add(mesh.vertices[i]);
        }
		for (int i = 0; i < mesh.vertices.Length-1; i+=64){
            outsidePoints.Add(mesh.vertices[i+1]);
        }

        /* for (int i = 0; i < this.stripes.Count; i++) {
            if (this.stripes[i]) {
                DestroyImmediate(this.stripes[i]);
            }
        }
        this.stripes = new List<GameObject>();
        var points = path.CalculateEvenlySpacedPoints(0.05f);
        for (int i = 0; i < points.Length; i ++) {
            Vector2 forward = Vector2.zero;
			if (i < points.Length-1 || path.IsClosed) {
                forward += points[(i + 1)%points.Length] - points[i];
            }
			if (i > 0) {
                forward += points[i] - points[(i - 1 + points.Length)%points.Length];
            }

            Vector2 left = new Vector2(-forward.y, forward.x);

            Vector2 leftSide = points[i] + left * roadWidth * 0.5f;
            Vector2 rightSide = points[i] - left * roadWidth * 0.5f;
            var stripe = Instantiate(roadStripe, points[i], Quaternion.identity);
            stripe.transform.localScale = Vector2.up + Vector2.right * roadWidth * 0.25f;
            stripe.transform.parent = stripeParent;
            stripe.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(points[i], left));
            this.stripes.Add(stripe);
        }*/

        innerEdgeCollider.points = insidePoints.ToArray();
        outsideEdgeCollider.points = outsidePoints.ToArray();
    }
}
