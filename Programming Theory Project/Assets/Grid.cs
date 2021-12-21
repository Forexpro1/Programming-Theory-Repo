using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject[] boxPrefabs;
    public int column = 8;
    public int row = 8;
    public int spacing = 2;
    List<GameObject> theList;
    public GameObject[,] grid; 

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[column, row];
        InitLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitLevel()
    {
         
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                GameObject temp  = Instantiate(boxPrefabs[RandomPrefab()], transform.position, transform.rotation);
                temp.transform.parent = this.transform;
                temp.transform.position = new Vector3(j * spacing, i * spacing, 0);
                 grid[j,i]= temp; 
            }
        }
        Destroy(grid[3, 5]);
        grid[3, 5] = null;
        if (grid[3,5]==null)
        {
            for (int i = 6; i < row; i++)
            {
                grid[3, i-1] = grid[3, i];
                grid[3, i - 1].transform.Translate(Vector3.down * spacing);
                grid[3, i] = null;
            }
        }
        Debug.Log(grid[3, 5]);
        Debug.Log(grid[3, 7]);
    }
   public int RandomPrefab ()
    {
        int result;
       result = Random.Range(0, boxPrefabs.Length);
        return result;
    }

}
