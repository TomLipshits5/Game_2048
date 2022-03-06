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
    public Block mergingBlock;
    public bool isMerging;
    public Vector2 Pos => transform.position;
    public Node node;


    




    public void init(BlockType type){
        value = type.value;
        renderer.color = type.color;
        blockText.text = type.value.ToString();

    }


    public void SetNode(Node node) 
    {
        if (this.node != null) this.node.currentBlock = null;

        this.node = node;
        this.node.currentBlock = this;
    
    }

    public int getVal()
    {
        return value; 
    }


    public void merge(Block blockToMergeWith)
    {
        mergingBlock = blockToMergeWith;
        mergingBlock.isMerging = true;

        node.currentBlock = null;


    }


    public bool CanMerge(int value) => value == this.value && mergingBlock == null && !isMerging;
}
