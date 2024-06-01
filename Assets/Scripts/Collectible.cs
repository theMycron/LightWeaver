using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float animationSpeed;
    private LevelManager levelManager;
    private bool collected = false;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void Update()
    {
        if (collected)
        {
            transform.localScale *= animationSpeed;
            // if it gets too small, destroy
            if (transform.localScale.x < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.tag.StartsWith("Robot"))
        {
            // play collection sound here
            collected = true;
            levelManager.CollectCollectible();
        }
    }
}
