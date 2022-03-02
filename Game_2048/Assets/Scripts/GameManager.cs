using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    

    [SerializeField] private int width = 4;
    [SerializeField] private int hight = 4;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private Block blockPrifab;
    [SerializeField] private List<BlockType> types;


    private List<Node> nodes = new List<Node>();
    private List<Block> blocks= new List<Block>();






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
        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(blockPrifab,node.pos,Quaternion.identity);
            block.init(getBlockType(2));
        }
    }
}

[SerializeField]
public struct BlockType
{
    public int value;
    public Color color;

    
}
