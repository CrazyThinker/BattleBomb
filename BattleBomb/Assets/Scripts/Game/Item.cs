using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int item;
    public Vector3Int position = new Vector3Int();

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player 1"))
        {
            gameManager.getItem(1, position, item);
        }
        else if(other.CompareTag("Player 2"))
        {
            gameManager.getItem(2, position, item);
        }

        Explode();
    }

    public void Explode()
    {
        Destroy(gameObject);
    }
}
