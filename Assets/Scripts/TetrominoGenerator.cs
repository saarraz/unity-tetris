using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
    public GameObject[] Tetrominoes;


    public void Generate()
    {
        var Prefab = Tetrominoes[UnityEngine.Random.Range(0, Tetrominoes.Length)];
        Instantiate(Prefab, transform.position, Quaternion.identity);
    }
}
