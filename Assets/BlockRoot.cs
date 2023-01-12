using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{
    public Block GetBlock()
    {
        return GetComponentInChildren<Block>();
    }
}