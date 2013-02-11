using UnityEngine;

public class Decals
{
    // no instantiating!
    private Decals() { }

    private static Mesh _plane;

    public static GameObject CreateDecal(Ray target, Texture texture)
    {
        GameObject g = _createDecal(target, new Vector2(1, 1));
        g.renderer.material.mainTexture = texture;
        return g;
    }

    public static GameObject CreateDecal(Ray target, Material material)
    {
        GameObject g = _createDecal(target, new Vector2(1, 1));
        g.renderer.material = material;
        return g;
    }

    public static GameObject CreateDecal(Ray target, Texture texture, Vector2 scale)
    {
        GameObject g = _createDecal(target, scale);
        g.renderer.material.mainTexture = texture;
        return g;
    }

    public static GameObject CreateDecal(Ray target, Material material, Vector2 scale)
    {
        GameObject g = _createDecal(target, scale);
        g.renderer.material = material;
        return g;
    }

    private static GameObject _createDecal(Ray target, Vector2 scale)
    {
        RaycastHit hit;
        if (Physics.Raycast(target, out hit))
        {
            GameObject go = new GameObject("Decal");

            go.AddComponent(typeof(MeshRenderer));
            MeshFilter m = go.AddComponent(typeof(MeshFilter)) as MeshFilter;

            m.mesh = _getMesh();

            go.transform.localScale = new Vector3(scale.x, scale.y, 1);

            go.transform.position = hit.point;
            go.transform.LookAt(hit.point + hit.normal);
            go.transform.Translate(Vector3.forward * 0.0001f);
            go.transform.eulerAngles = new Vector3(go.transform.eulerAngles.x, go.transform.eulerAngles.y, Random.Range(0, 359));
            return go;
        }
        else
        {
            Debug.LogWarning("The raycast for the decal didn't hit anything!");
            return null;
        }
    }

    private static Mesh _getMesh()
    {
        if (_plane != null) return _plane;

        _plane = new Mesh();

        _plane.name = "Decal Plane Mesh";

        // Create verticies for "flat" plane (normal up).
        Vector3[] verts = new Vector3[4];
        verts[0] = new Vector3(-.5f, -.5f, 0);
        verts[1] = new Vector3(.5f, -.5f, 0);
        verts[2] = new Vector3(-.5f, .5f, 0);
        verts[3] = new Vector3(.5f, .5f, 0);

        int[] tris = new int[] { 0, 1, 2, 3, 2, 1 };

        Vector2[] uvs = new Vector2[verts.Length];

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(1, 1);

        _plane.vertices = verts;
        _plane.triangles = tris;
        _plane.uv = uvs;
        _plane.RecalculateNormals();

        return _plane;
    }
}