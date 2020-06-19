using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureShaderSet : MonoBehaviour {

    [Range (0, 5)]
    public float tiling_x;
    [Range (0, 5)]
    public float tiling_y;
    [Range (0, 5)]
    public float offset_x;
    [Range (0, 5)]
    public float offset_y;

    Material m_mater = null;
    // Start is called before the first frame update
    void Start () {
        m_mater = GetComponent<Renderer> ().material;

    }

    // Update is called once per frame
    void Update () {
        m_mater.SetFloat ("tiling_x", tiling_x);
        m_mater.SetFloat ("tiling_y", tiling_y);
        m_mater.SetFloat ("offset_x", offset_x);
        m_mater.SetFloat ("offset_y", offset_y);
    }
}