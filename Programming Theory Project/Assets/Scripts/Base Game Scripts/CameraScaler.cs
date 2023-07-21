using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is used on Camera to adjust the view based on row and column pieces
public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = .562f;
    public float padding;
    public float yOffset = 1f;
    private float orthographicSize;
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            
            RepositionCamera(board.width, board.height);
        }
    }

    void RepositionCamera(float x, float y)
    {
        
        if (board.width >= board.height)
        {
            float boardRatio = (float)board.height / (float)board.width;
            orthographicSize = (boardRatio*board.width) + padding;
            
            Vector3 tempPosition = new Vector3(x/2, y/2 + yOffset, cameraOffset);
            transform.position = tempPosition; 
            Camera.main.orthographicSize = orthographicSize;
        }
        else
        {
            float boardRatio = (float) board.width / (float) board.height;
            orthographicSize = (boardRatio * board.height) + padding;
            
            Vector3 tempPosition = new Vector3(x/2, y/2 + yOffset, cameraOffset);
            transform.position = tempPosition;
            Camera.main.orthographicSize = orthographicSize;
        }
        
    }
 
}
