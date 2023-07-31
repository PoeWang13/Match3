using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Canvas_Manager : MonoBehaviour
{
	private static Canvas_Manager instance;
	public static Canvas_Manager Instance { get => instance; }
    private const float DoTweenDuration = 0.1f;

    [SerializeField] private Sprite lockIcon;
    public Sprite LockIcon { get => lockIcon; }

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private TextMeshProUGUI textScore;
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
    /// <summary>
    /// Show game board view
    /// </summary>
    /// <param name="myBoard">Game Board</param>
    public void SetBoardView(Tile[,] myTile, Vector3 boardPosition)
    {
        int width = myTile.GetLength(0);
        int height = myTile.GetLength(1);
        tileParent.position = boardPosition;
        tileParent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height) * 100;

        // Create tile view
        for (int h = 0; h < width; h++)
        {
            for (int e = 0; e < height; e++)
            {
                Vector2Int coordinate = new Vector2Int(h, e);
                Tile tile = Instantiate(tilePrefab, tileParent);
                tile.transform.position = new Vector3(h + 1, e + 1, 0) * 100;
                tile.name = "Tile > x : " + h + " --- y : " + e;
                Board_Manager.Instance.SetTiles(coordinate, tile);
            }
        }
    }
    [ContextMenu("Set New Board")]
    private void SetNewBoard()
    {
        for (int e = tileParent.childCount - 1; e >= 0; e--)
        {
            Destroy(tileParent.GetChild(e).gameObject);
        }
        Board_Manager.Instance.SetBoard(Game_Manager.Instance.BoardSize, Game_Manager.Instance.BoardPosition);
    }
    /// <summary>
    /// Create new tile after destroyed match's tile
    /// </summary>
    /// <param name="coordinate">New tile's X position point.</param>
    /// <param name="height">Specifies how high it will be created.</param>
    /// <returns>Returning new tile</returns>
    public Tile CreateTiles(int coordinate, int height)
    {
        Tile tile = Instantiate(tilePrefab, tileParent);
        tile.transform.position = new Vector3(coordinate + 1, height, 0) * 100;

        return tile;
    }
    /// <summary>
    /// Set our match score.
    /// </summary>
    public void SetScore()
    {
        DOTween.To(value => 
            {
                textScore.text = "Score : " + (Board_Manager.Instance.Score + (int)value); 
            }, 
            startValue: 0, 
            endValue: Board_Manager.Instance.ScoreAdd, duration: DoTweenDuration)
            .OnComplete(() => { Board_Manager.Instance.ClearMatchControlling(); });
    }
}