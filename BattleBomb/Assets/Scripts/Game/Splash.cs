using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(timer(0.5f)); // 0.5�� �� �����
    }

    private IEnumerator timer(float delay)
    {
        // delay �� ���� ���
        yield return new WaitForSeconds(delay);

        Explode();
    }

    public void Explode()
    {
        Destroy(gameObject);
    }
}
