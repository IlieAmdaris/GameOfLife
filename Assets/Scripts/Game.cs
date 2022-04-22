using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class Game : MonoBehaviour
{
    private static int SCREEN_WIDTH = 64;
    private static int SCREEN_HEIGHT = 48;
    private float speed = 0.1f;
    private float timer = 0f;

    public HUD hud;

    public bool simulationEnabled = false;
    Cell[,] grid = new Cell[SCREEN_WIDTH,SCREEN_HEIGHT];
    void Start()
    {
        EventManager.StartListening("SavePattern",SavePattern);
        EventManager.StartListening("LoadPattern",LoadPattern);
        PlaceCells();
    }

    // Update is called once per frame
    void Update()
    {
        if(simulationEnabled)
        {
            if(timer >= speed)
            {
                timer = 0f;
                CountNeighbors();  
                ControlPopulation(); 
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        UserInput();        
    }
    void PlaceCells()
    {
        for(int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                if(x < SCREEN_WIDTH / 2){
                    Cell cell  = Instantiate(Resources.Load("Prefabs/Cell", typeof(Cell)), new Vector2(x,y), Quaternion.identity) as Cell; 
                    grid[x,y] = cell;
                    cell.SetAlive(false);
                }
                else{
                    Cell cell  = Instantiate(Resources.Load("Prefabs/Virus", typeof(Cell)), new Vector2(x,y), Quaternion.identity) as Cell; 
                    grid[x,y] = cell;
                    cell.SetAlive(false);
                }
                
            }
        }
    }
    void CountNeighbors()
    {
        for(int y = 0; y < SCREEN_HEIGHT;y++)
        {
            for(int x = 0; x < SCREEN_WIDTH;x++)
            {
                int numNeighbors = 0;
                //-North
                if(y + 1 < SCREEN_HEIGHT)
                {
                    if(grid[x,y+1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                //East
                if(x + 1 < SCREEN_WIDTH)
                {
                    if(grid[x+1,y].isAlive) numNeighbors++;
                }
                //Sout
                if(y - 1 >= 0)
                {
                    if(grid[x,y-1].isAlive) numNeighbors++;
                }
                //west
                if(x - 1 >= 0)
                {
                    if(grid[x-1,y].isAlive) numNeighbors++;
                }
                //northeast
                if(x + 1 < SCREEN_WIDTH && y + 1 < SCREEN_HEIGHT)
                {
                    if(grid[x+1,y+1].isAlive) numNeighbors++;
                }
                //northwest 
                if(x - 1 >= 0 && y + 1 < SCREEN_HEIGHT)
                {
                    if(grid[x-1,y+1].isAlive)numNeighbors++;
                }
                //southeast
                if(x+1 < SCREEN_WIDTH && y-1 >= 0)
                {
                    if(grid[x+1,y-1].isAlive)numNeighbors++;
                }
                //southwest
                if(x-1 >= 0 && y-1 >=0){
                    if(grid[x-1,y-1].isAlive)numNeighbors++;
                }
                grid[x,y].numNeighbors = numNeighbors;
            }
        }
    }
    void ControlPopulation()
    {
        for(int y = 0; y < SCREEN_HEIGHT; y++){
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                if(grid[x,y].isAlive)
                {
                    if(grid[x,y].numNeighbors != 2 && grid[x,y].numNeighbors != 3)
                    {
                        grid[x,y].SetAlive(false);
                    }
                }
                else
                {
                    if(grid[x,y].numNeighbors == 3)
                    {
                        grid[x,y].SetAlive(true);
                    }
                }
            }
        }
    }
    bool RandomizeAliveCells()
    {
        int rand = UnityEngine.Random.Range(0,100);
        if(rand < 45) return true;
        return false;
    } 
    private void SavePattern()
    {
        string path = "patterns";
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        Pattern pattern = new Pattern();
        string patternString = null;
        for(int y = 0; y < SCREEN_HEIGHT; y++)
        {
            for(int x = 0; x < SCREEN_WIDTH; x++)
            {
                patternString += grid[x,y].isAlive ? "1" : "0";
            }
        }
        pattern.patternString = patternString;

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        StreamWriter writer = new StreamWriter($"{path}/{hud.saveDialog.patternName.text}.xml");
        serializer.Serialize(writer.BaseStream,pattern);
        writer.Close();
    }
    private void LoadPattern()
    {
        string path = "patterns";
        if(!Directory.Exists(path)) return;
        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        string patternName = hud.loadDialog.patternName.options[hud.loadDialog.patternName.value].text;
        path += $"/{patternName}.xml";
        StreamReader reader = new StreamReader(path);
        Pattern pattern = (Pattern)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        bool isAlive;
        int x = 0, y = 0;
        foreach(char c in pattern.patternString)
        {
            isAlive = c.ToString().Equals("1") ? true : false;
            grid[x,y].SetAlive(isAlive);
            x++;
            if(SCREEN_WIDTH == x)
            {
                x = 0;
                y++;
            }
        }
    }
    void UserInput()
    {
        if(!hud.isActive)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int x = Mathf.RoundToInt(mousePoint.x);
                int y = Mathf.RoundToInt(mousePoint.y);
                if(x >= 0 && y >= 0 && x < SCREEN_WIDTH && y < SCREEN_HEIGHT)
                {
                    //inbounds
                    grid[x,y].SetAlive(!grid[x,y].isAlive);
                }
            }
            if(Input.GetKeyUp(KeyCode.P))
            {   
                //pause
                simulationEnabled = false;
            }
            if(Input.GetKeyUp(KeyCode.B))
            {
                //begin
                simulationEnabled = true;
            }
            if(Input.GetKeyUp(KeyCode.S))
            {
                //Save pattern
                hud.ShowSaveDialog();
            }
            if(Input.GetKeyUp(KeyCode.L))
            {
                //load
                hud.showLoadDialog();
            }
        }
        
        
    }
}
