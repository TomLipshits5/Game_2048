using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[System.Serializable]
public class GameManager : MonoBehaviour
{
    

    [SerializeField] private int width = 4;
    [SerializeField] private int hight = 4;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private SpriteRenderer boardPrefab;
    [SerializeField] private Block blockPrifab;
    [SerializeField] private float swipeMinimumDistance;
    [SerializeField] private float travelTime = 0.2f;
    [SerializeField] private int winCon = 2048;

    [SerializeField] public List<BlockType> types = new List<BlockType>();

    [SerializeField] private GameObject winScreen, loseScreen;

    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private Swipe swipe;
    private List<Node> nodes = new List<Node>();
    private List<Block> blocks= new List<Block>();
    private GameState gameState;
    private bool isFirstTurn = true;






    // Start is called before the first frame update
    void Start()
    {
        swipeMinimumDistance = Screen.height * 15 / 100;
        changeGameState(GameState.GenerateLevel);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != GameState.WaitForInput) return;

        swipe = getPlayerInput();
        changeGameState(GameState.Move);
        changeGameState(GameState.WaitForInput);


    }


    private BlockType getBlockType(int val) => types.First(t => t.value == val);

    private void changeGameState(GameState state){
        gameState = state;

        switch(gameState){
            case GameState.GenerateLevel:
                generateGrid();
                break;
            case GameState.SpawnBlocks:
                getNewBlock(isFirstTurn ? 2: 1);
                isFirstTurn = false;
                break;
            case GameState.Move:
                moveBoard(swipe);
                break;
            case   GameState.WaitForInput:
                break;
            case GameState.Loss:
                loseScreen.SetActive(true);
                break;
            case GameState.Win:
                winScreen.SetActive(true);
                break;
            
                default: throw new System.ArithmeticException();

          
        }
        


    }

    private void moveBoard(Swipe swipe)
    {
        switch (swipe)
        {
            case Swipe.up:
                shift(Vector2.up);
                break;

            case Swipe.down:
                shift(Vector2.down);
                break;

            case Swipe.right:
                shift(Vector2.right);
                break;

            case Swipe.left:
                shift(Vector2.left);
                break;


            default: 
                changeGameState(GameState.WaitForInput);
                break;

        }
    }


    private void shift(Vector2 dir) 
    {
        var orderedBlocks =  blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach(var block in orderedBlocks)
        {
            var next = block.node;
            do
            {
                block.SetNode(next);

                var possibleNode = getNodeAtPos(next.pos + dir);
                if(possibleNode != null)
                {
                    if(possibleNode.currentBlock != null && possibleNode.currentBlock.CanMerge(block.getVal()))
                    {
                       block.merge(possibleNode.currentBlock);
                    }

                    else if(possibleNode.currentBlock == null)
                    {
                        next = possibleNode;
                    }
                }

            }while (next != block.node);

            
        }

        var sequnce = DOTween.Sequence();

        foreach(var block in orderedBlocks)
        {
            var movePoint = block.mergingBlock != null ? block.mergingBlock.node.pos : block.node.pos;

            sequnce.Insert(0, block.transform.DOMove(movePoint, travelTime));

        }

        sequnce.OnComplete(() =>
        {
            foreach (var block in blocks.Where(b => b.mergingBlock != null))
            {
                mergeBlocks(block.mergingBlock, block);
            }
            changeGameState(GameState.SpawnBlocks);
        });

    }


    private void mergeBlocks(Block baseBlock, Block other)
    {
        spawnBlock(baseBlock.node,getBlockType(baseBlock.getVal() * 2));
        removeBlock(baseBlock);
        removeBlock(other);


    }


    private void removeBlock(Block block)
    {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }

    private Node getNodeAtPos(Vector2 pos)
    {
        Node node = nodes.FirstOrDefault(n => n.pos == pos);
        return node;
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

        changeGameState(GameState.SpawnBlocks);
    }



    private void getNewBlock(int amount){

        var freeNodes = nodes.Where(n => n.currentBlock == null).OrderBy(b=>Random.value).ToList();
        BlockType blockType = getBlockType(Random.value>0.8 ? 4 : 2);
        foreach (var node in freeNodes.Take(amount))
        {
            spawnBlock(node, blockType);
        }



        if(freeNodes.Count() == 1)
        {
            changeGameState(GameState.Loss);
            return;
        }

        changeGameState(blocks.Any(b => b.getVal() == winCon) ? GameState.Win : GameState.WaitForInput);
        
    }


    private void spawnBlock(Node node,BlockType blockType)
    {
        var block = Instantiate(blockPrifab, node.pos, Quaternion.identity);
        block.init(blockType);
        block.SetNode(node);
        blocks.Add(block);
    }
    
    

    
    private Swipe getPlayerInput()
    {
        Swipe swipe = Swipe.tap;
        
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 
            if(touch.phase == TouchPhase.Began)
            {
                firstPosition = touch.position;
                lastPosition = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                lastPosition = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                lastPosition = touch.position;
                float xDis = Mathf.Abs(firstPosition.x - lastPosition.x);
                float yDis = Mathf.Abs(firstPosition.y - lastPosition.y);
                swipe = GetSwipeDirection(xDis, yDis);
               
            }
        }




        return swipe;
    }


    private Swipe GetSwipeDirection(float xDis, float yDis)
    {
        if (xDis > swipeMinimumDistance || yDis > swipeMinimumDistance)
        {
            if (xDis > yDis)
            {
                if (lastPosition.x > firstPosition.x)
                {
                    return Swipe.right;
                }
                else
                {
                    return Swipe.left;
                }
            }
            else
            {
                if (lastPosition.y > firstPosition.y)
                {
                    return Swipe.up;
                }
                else
                {
                    return Swipe.down;
                }
            }
        }
        return Swipe.tap;
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


public enum Swipe
{
    right,
    left,
    up,
    down,
    tap
}
