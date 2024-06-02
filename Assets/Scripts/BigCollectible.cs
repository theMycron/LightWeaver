using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCollectible : MonoBehaviour
{
    [SerializeField] private float animationSpeed;
    [SerializeField] private float maxSize;
    private LevelManager levelManager;
    private bool collected = false;
    private bool gameEnded = false;

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

            // end game
            if (!gameEnded)
            {
                StartCoroutine(levelManager.EndGame());
                gameEnded = true;
            }

            // if it gets too big, stop growing
            if (transform.localScale.x > maxSize)
            {
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
