using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int state = 1;
    public Vector3Int location = new Vector3Int(0, 0, 0);
    public Vector3Int lastBombLocation = new Vector3Int(0, 0, 0);
    public GameObject lastBomb = null;
    public float speed = 10f; // ĳ���� ���ǵ�
    public Vector3 speedBoundary; // (���� ���ǵ�, �ִ� ���ǵ�, ����)
    public int count = 0; // ��ź ����
    public int nowcount = 0; // ���� �ʿ� ������ ��ź ����
    public Vector2Int countBoundary; // (���� ����, �ִ� ����)
    public int length = 0; // ��ź ����
    public Vector2Int lengthBoundary; // (���� ����, �ִ� ����)
    public bool ableKick = false;
    public bool ableMove = true;
    public float pushTimer = 0;
    public Vector3 pushDirection = new Vector3();

    public KeyCode upKey, downKey, leftKey, rightKey, bombKey;

    // ĳ���� �ʱ�ȭ
    public void setCharacter(int numType)
    {
        switch(numType)
        {
        case 0: // basic
            speedBoundary = new Vector3(4f, 7f, 1f); // �ӵ�
            countBoundary = new Vector2Int(1, 7); // ��ǳ�� ����
            lengthBoundary = new Vector2Int(1, 7); // ��ǳ�� ����
            break;
        case 1: // speedy
            speedBoundary = new Vector3(4.5f, 8.9f, 1.1f);
            countBoundary = new Vector2Int(1, 6);
            lengthBoundary = new Vector2Int(1, 6);
            break;
        case 2: // heavy
            speedBoundary = new Vector3(3.7f, 6.4f, 0.9f);
            countBoundary = new Vector2Int(2, 10);
            lengthBoundary = new Vector2Int(1, 10);
            break;
        case 3: // long
            speedBoundary = new Vector3(4f, 7f, 1f);
            countBoundary = new Vector2Int(1, 8);
            lengthBoundary = new Vector2Int(2, 10);
            break;
        }

        speed = speedBoundary.x;
        count = countBoundary.x;
        nowcount = 0;
        length = lengthBoundary.x;
    }

}
