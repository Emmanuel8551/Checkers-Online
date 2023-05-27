using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tile : NetworkBehaviour
{
    public EventHandler Clicked;
    private bool selected;

    [SerializeField] private SpriteRenderer sr;

    private float side = 1;
    private Checker checker;
    public float Side { get => side; set => side = value; }
    public Checker Checker { get => checker; set => checker = value; }
    public bool Selected { get => selected; set => selected = value; }

    private void Update()
    {
        if (Selected)
        {
            sr.color = Color.gray;
            return;
        }
        if (isMouseHovering())
        {
            sr.color = Color.red;
            if (Input.GetMouseButton(0)) sr.color = Color.cyan;
            if (Input.GetMouseButtonDown(0)) Clicked.Invoke(this, new EventArgs());
        }
        else sr.color = Color.white;
    }

    private bool isMouseHovering ()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x < transform.position.x - side / 2 || mousePosition.x > transform.position.x + side / 2) return false;
        if (mousePosition.y < transform.position.y - side / 2 || mousePosition.y > transform.position.y + side / 2) return false;
        return true;
    }
}
