using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private float animationSpeed;
    [SerializeField] private bool realCollectible;
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
            if (EnvSFX.instance != null)
                EnvSFX.instance.PlayObjectSFX(EnvSFX.instance.collectible);
            collected = true;
            // collectibles that arent real will not add to the score nor print to the terminal
            if (levelManager != null && realCollectible)
                levelManager.CollectCollectible();
        }
    }
}
