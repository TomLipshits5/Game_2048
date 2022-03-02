using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private int value;
    [SerializeField] private SpriteRenderer renderer;



   public void init(BlockType type){
        value = type.value;
        renderer.color = type.color;

    }
}
