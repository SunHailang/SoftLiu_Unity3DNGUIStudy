using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ShaderStady : MonoBehaviour
{
    [SerializeField]
    private GameObject m_cube;
    // Start is called before the first frame update
    void Start()
    {
        m_cube.GetComponent<Renderer>().material.SetVector("_SetColor",new Vector4(1,0,0,1));
    }

    // Update is called once per frame
    void Update()
    {

    }
}