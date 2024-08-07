using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChatSocketManager.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
