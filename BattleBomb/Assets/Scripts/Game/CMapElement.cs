using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMapElement
{
    public int type; // 0: Empty
    public Color color;

    // ������ ����
    // public int shape; // 0: Empty, 1: Cube, 2: Sphere, 3: Capsule, 4: Cylinder, 5: Plane
    // rgb �־����

    public int state = 0; // 0: ����, 1: ��ź
                          // 11: ��ź����, 12: ��ź����, 13: �̼�, 14: ��ȭ

    public Block block; // �����̴� ���̶��
    public Bomb bomb; // Ȥ�� ��ź�� �ִٸ�
    public Item item; // Ȥ�� �������� �ִٸ�

    public CMapElement() { }
    public CMapElement(int type, Color color)
    {
        this.type = type;
        this.color = color;
    }

}
