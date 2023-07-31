using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
	private static Game_Manager instance;
	public static Game_Manager Instance { get => instance; }
    [SerializeField] private Vector2Int boardSize;
    [SerializeField] private Vector3 boardPosition;
    public Vector2Int BoardSize { get => boardSize; }
    public Vector3 BoardPosition { get => boardPosition; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Board_Manager.Instance.SetBoard(boardSize, boardPosition);
    }
}