using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Checker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
}
