using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Block : MonoBehaviour
{
    private int value;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private TextMeshPro blockText;



   public void init(BlockType type){
        value = type.value;
        renderer.color = type.color;
        blockText.text = type.value.ToString();

    }
}
