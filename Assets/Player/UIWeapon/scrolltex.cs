using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrolltex : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(.5f, .5f);
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * scrollSpeed.x, Time.time * scrollSpeed.y);
        _renderer.material.mainTextureOffset = offset;
    }
}
