using TMPro;
using DG.Tweening;
using UnityEngine;

public class Canvas_Manager : MonoBehaviour
{
	private static Canvas_Manager instance;
	public static Canvas_Manager Instance { get => instance; }
    private const float DoTweenDuration = 0.1f;

    [SerializeField] private Sprite lockIcon;

    [Space]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private TextMeshProUGUI textScore;

    [Space]
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private GameObject panelHit;
    [SerializeField] private TextMeshProUGUI textHit;
    public Sprite LockIcon { get => lockIcon; }
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
                tile.transform.localPosition = new Vector3(h, e, 0) * 100 + new Vector3(50, 50, 0);
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
        tile.transform.localPosition = new Vector3(coordinate, height, 0) * 100 + new Vector3(50, 50, 0);

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
    public void OpenHitCountAnimation(int hitCount)
    {
        if (hitCount == 1)
        {
            return;
        }
        panelHit.SetActive(true);
        textHit.text = "Counter Hit " + hitCount;
        hitAnimator.SetInteger("Hit", Random.Range(1, 3));
    }
    // Add Hit Counter animation for event.
    public void CloseHitCountAnimation()
    {
        hitAnimator.SetInteger("Hit", 0);
        panelHit.SetActive(false);
    }
}