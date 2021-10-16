#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TetrominoGenerator? Generator;
    public RowDetector[]? RowDetectors;

    public Tetromino? CurrentTetromino { get; private set; }
    public GameObject? CurrentTetrominoPrefab { get; private set; }
    public GameObject? SavedTetrominoPrefab { get; private set; }

    private bool _tetrominoSavedSinceLastSpawn = false;

    public void Start()
    {
        Spawn();
        RowDetectors = GetComponentsInChildren<RowDetector>();
    }

    public void Spawn(GameObject? Prefab = null)
    {
        Debug.Assert(!CurrentTetrominoPrefab, "Spawn() called while turn is in progress");
        (CurrentTetromino, CurrentTetrominoPrefab) = Generator!.Generate(Prefab);
        Debug.Log($"Prefab: {CurrentTetrominoPrefab}");
        _tetrominoSavedSinceLastSpawn = false;
    }

    public void CancelTurn()
    {
        if (!CurrentTetromino)
        {
            return;
        }
        Destroy(CurrentTetromino!.gameObject);
        CurrentTetrominoPrefab = null;
        CurrentTetromino = null;
    }

    public void DecomposeTetromino(Tetromino tetromino)
    {
        Transform[] blocks = (from Transform block in tetromino.Body!.transform select block).ToArray();
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
        foreach (RowDetector rowDetector in RowDetectors!)
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

    private void SaveTetromino()
    {
        if (!CurrentTetrominoPrefab) {
            Debug.LogWarning("CurrentTetrominoPrefab was null when trying to save tetromino");
            return;
        }
        GameObject newSaved = CurrentTetrominoPrefab!;
        CancelTurn();
        Spawn(SavedTetrominoPrefab);
        SavedTetrominoPrefab = newSaved;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!_tetrominoSavedSinceLastSpawn)
            {
                SaveTetromino();
                _tetrominoSavedSinceLastSpawn = true;
            } else
            {
                Debug.Log("Skipping save since already saved since last spawn");
            }
        }
    }
}
