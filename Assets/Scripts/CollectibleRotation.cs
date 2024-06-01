using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleRotation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 rotationVector;
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(rotationVector);
    }
}
