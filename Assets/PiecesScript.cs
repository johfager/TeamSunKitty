using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesScript : MonoBehaviour
{

int []  rotations ={0, 90, 180, 270}; //obs tutorial have wrong ()

public float correctRotation; // facit (in num degrees) on per piece object basis
[SerializeField]
bool isPlaced= false; //will update to check if registered correct placement



    //switch sprite things
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;

    
    PuzzleManager puzzleManager; //for relaying numcompleted
    public GameObject otherObject;
    
    private void Start()
    {
        puzzleManager= otherObject.GetComponent<PuzzleManager>();
        

        //access to change sprite
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //Shuffle pieces
        int rand= Random.Range(0,rotations.Length);
        transform.eulerAngles= new Vector3(0,0,rotations[rand]);

        // check if right rotation
        if(transform.eulerAngles.z==correctRotation && isPlaced== false)
        {
            isPlaced= true; //register as placed

            //visible effect showing it's correct

        }
    }
   private void OnMouseDown()
   {
    //rotate clicked piece
    //transform.Rotate(new Vector3(0,0,90)); //orig
    //youtube check fix from random assortment won't be checked correctly
    transform.Rotate(0,0,90);
    transform.eulerAngles= new Vector3(0,0,Mathf.Round(transform.eulerAngles.z));

    // check if right rotation
    //modified with yt
    
    if(transform.eulerAngles.z==correctRotation && isPlaced== false) 
    {
        isPlaced= true; //register as placed
        //change sprite
         spriteRenderer.sprite = newSprite; 

        
         //numCompleted+=1;
        // print(PuzzleManager.numCompleted);
        puzzleManager.numCompleted++;

        //visible effect showing it's correct

    }
    else if(isPlaced== true)
    {
        isPlaced=false;
    }

    //check if puzzle is finished
    // give key or make interactable prop appear
   }
}
