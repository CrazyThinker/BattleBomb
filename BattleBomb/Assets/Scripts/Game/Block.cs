using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void moveBlock(Vector3 move)
    {
        StartCoroutine(moveBlockCoroutine(move));
    }

    private IEnumerator moveBlockCoroutine(Vector3 move)
    {
        int i;

        for (i = 0; i < 10; i++)
        {
            transform.position += move * 0.1f;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void destroyBlock()
    {
        Destroy(gameObject);
    }
}
