using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMapElement
{
    public int type; // 0: Empty
    public Color color;

    // 디자인 관련
    // public int shape; // 0: Empty, 1: Cube, 2: Sphere, 3: Capsule, 4: Cylinder, 5: Plane
    // rgb 있어야함

    public int state = 0; // 0: 없음, 1: 폭탄
                          // 11: 폭탄갯수, 12: 폭탄길이, 13: 이속, 14: 장화

    public Block block; // 움직이는 벽이라면
    public Bomb bomb; // 혹시 폭탄이 있다면
    public Item item; // 혹시 아이템이 있다면

    public CMapElement() { }
    public CMapElement(int type, Color color)
    {
        this.type = type;
        this.color = color;
    }

}
