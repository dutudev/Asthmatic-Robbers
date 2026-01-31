using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Item
{
    [SerializeField] private float textHeight;
    
    private Vector2 _posToMove;
    public override void Interact()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().MoveToPos(_posToMove - new Vector2(0, 0.7f));
    }

    public override float GetTextHeight()
    {
        return textHeight;
    }

    public void SetPosToMove(Vector2 set)
    {
        _posToMove = set;
    }

    public Vector2 GetPosToMove()
    {
        return _posToMove;
    }
}
