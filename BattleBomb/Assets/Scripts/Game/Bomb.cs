using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3Int position;
    public int player;
    public int length;
    public GameObject splashPrefab;
    private GameManager gameManager;
    private Coroutine timerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();

        timerCoroutine = StartCoroutine(timer(2.0f)); // 2�� �� ����
    }

    private IEnumerator timer(float delay)
    {
        // delay �� ���� ���
        yield return new WaitForSeconds(delay);

        // ���� ó��
        //Instantiate(splashPrefab, transform.position, Quaternion.identity);

        gameManager.clearCheck();
        Explode();
        gameManager.destroyBlockCheck();
    }

    public void Explode()
    {
        if (gameManager != null)
        {
            gameManager.splashBomb(player, position, length, splashPrefab);
        }

        Destroy(gameObject);

    }

    public void kickBomb(Vector3Int destination)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        // ���ο� ��ġ�� �̵�
        position = destination;
        transform.position = destination;

        timerCoroutine = StartCoroutine(timer(2.0f));
    }

}
