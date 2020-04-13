using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashLoader : MonoBehaviour
{

    [SerializeField]
    private string m_image = null;

    // Use this for initialization
    void Start()
    {
        Instantiate<GameObject>(Resources.Load<GameObject>(m_image), transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
