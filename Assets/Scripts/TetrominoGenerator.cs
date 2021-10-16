using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoGenerator : MonoBehaviour
{
    public GameObject[] Tetrominoes;


    public Tuple<Tetromino, GameObject> Generate(GameObject Prefab = null)
    {
        if (!Prefab)
        {
            Prefab = Tetrominoes[UnityEngine.Random.Range(0, Tetrominoes.Length)];
        }
        return Tuple.Create(Instantiate(Prefab, transform.position, Quaternion.identity).GetComponent<Tetromino>(), Prefab);
    }
}
