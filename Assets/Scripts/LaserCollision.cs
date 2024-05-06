using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollision : MonoBehaviour
{
    public Color color;
    private ParticleSystem _particleSystem;
    private Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        mesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;

        Color32[] colors = new Color32[mesh.vertices.Length];
        for (int i=0; i<colors.Length; i++)
        {
            colors[i] = color;
        }
        mesh.colors32 = colors;

        var main = _particleSystem.main;
        main.startColor = color;

    }

    public void MoveCollision(Vector3 pos, Quaternion dir)
    {
        transform.position = pos;
        transform.rotation = dir;
    }

    private void OnDestroy()
    {
        Destroy(mesh);
    }
}
