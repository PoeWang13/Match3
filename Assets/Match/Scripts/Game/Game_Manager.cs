using UnityEngine;

public class Game_Manager : MonoBehaviour
{
	private static Game_Manager instance;
	public static Game_Manager Instance { get => instance; }

    [Header("Size of board")]
    [SerializeField] private Vector2Int boardSize;
    [Header("The starting position where the board will be on the screen.")]
    [SerializeField] private Vector3 boardPosition;
    /// <summary>
    /// Size of board
    /// </summary>
    public Vector2Int BoardSize { get => boardSize; }
    /// <summary>
    /// The starting position where the board will be on the screen.
    /// </summary>
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