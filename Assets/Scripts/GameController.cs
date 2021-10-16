using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TetrominoGenerator Generator;
    public RowDetector[] RowDetectors;

    void Start()
    {
        Spawn();
        RowDetectors = GetComponentsInChildren<RowDetector>();
    }

    public void Spawn()
    {
        Generator.Generate();
    }

    public void DecomposeTetromino(Tetromino tetromino)
    {
        Transform[] blocks = (from Transform block in tetromino.Body.transform select block).ToArray();
        foreach (Transform block in blocks)
        {
            block.SetParent(this.transform);
        }
        Destroy(tetromino.gameObject);
    }

    public IEnumerable<GameObject> AllBlocks()
    {
        return GameObject.FindGameObjectsWithTag("Block");
    }

    public void OnTetrominoTermination(Tetromino tetromino)
    {
        StartCoroutine(ProcessTetrominoTermination(tetromino));
    }

    private IEnumerator ProcessTetrominoTermination(Tetromino tetromino)
    { 
        DecomposeTetromino(tetromino);
        yield return new WaitForFixedUpdate();
        var destroyedRows = new List<Tuple<RowDetector, IEnumerable<GameObject>>> ();
        foreach (RowDetector rowDetector in RowDetectors)
        {
            var blocks = rowDetector.DetectRow();
            if (blocks == null)
            {
                continue;
            }
            destroyedRows.Add(Tuple.Create(rowDetector, blocks));
        }
        foreach (var row in destroyedRows)
        {
            foreach (GameObject block in row.Item2)
            {
                Destroy(block);
            }
        }
        foreach (GameObject block in AllBlocks())
        {
            int rowsDestroyedBelow = (from row in destroyedRows where row.Item1.transform.position.y < block.transform.position.y select row).Count();
            if (rowsDestroyedBelow != 0)
            {
                block.transform.position -= new Vector3(0, rowsDestroyedBelow * Tetromino.BlockSize);
            }
        }
        Spawn();
        yield break;
    }

    void Update()
    {
        
    }
}
