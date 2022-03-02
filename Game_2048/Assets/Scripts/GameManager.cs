using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;


[System.Serializable]
public class GameManager : MonoBehaviour
{
    

    [SerializeField] private int width = 4;
    [SerializeField] private int hight = 4;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private Block blockPrifab;

    [SerializeField] public List<BlockType> types = new List<BlockType>();


    private List<Node> nodes = new List<Node>();
    private List<Block> blocks= new List<Block>();
    private GameState gameState;






    // Start is called before the first frame update
    void Start()
    {
        generateGrid();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private BlockType getBlockType(int val) => types.First(t => t.value == val);

    private void changeGameState(GameState state){
        gameState = state;

        switch (state)
        {
            
            default:
        }
                    
    }


    void generateGrid(){
        
        for (int i = 0 ; i<width;i++){
            for(int j = 0; j<hight;j++){
                var node = Instantiate(nodePrefab,new Vector2(i,j),Quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2((float) width/2 -0.5f,(float)hight/2 - 0.5f);

        var board = Instantiate(boardPrefab, center,Quaternion.identity);
        board.size = new Vector2(width,hight);

        Camera.main.transform.position = new Vector3(center.x,center.y,-10);

        getNewBlock(2);
    }



    void getNewBlock(int amount){

        var freeNodes = nodes.Where(n => n.currentBlock == null).OrderBy(b=>Random.value).ToList();
        BlockType blockType = getBlockType(Random.value>0.8 ? 4 : 2);
        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(blockPrifab,node.pos,Quaternion.identity);
            block.init(blockType);
            blocks.Add(block);
        }
        
    }
}

[System.Serializable]
public struct BlockType
{
    public int value;
    public Color color;

    
}


public enum GameState{
    GenerateLevel,
    SpawnBlocks,
    Move,
    WaitForInput,
    Win,
    Loss
}
