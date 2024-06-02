using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCollectible : MonoBehaviour
{
    [SerializeField] private float animationSpeed;
    [SerializeField] private float maxSize;
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
            if (transform.localScale.x > maxSize)
            {
                // end game
                StartCoroutine(levelManager.EndGame());
                collected = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.tag.StartsWith("Robot"))
        {
            collected = true;
            levelManager.CollectCollectible();
        }
    }
}
