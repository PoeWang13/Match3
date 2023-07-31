using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField] private Vector2Int myCoordinate;
	[SerializeField] private BoardTile myBoardTile;
	[SerializeField] private Image myImageIcon;
	[SerializeField] private Image myImageLock;

	[SerializeField] private TextMeshProUGUI myTextLock;

	public BoardTile MyBoardTile { get => myBoardTile; }
	public Vector2Int MyCoordinate { get => myCoordinate; }


	[SerializeField] private Tile[] myVerticalNeighbors = new Tile[2];
	[SerializeField] private Tile[] myHorizontalNeighbors = new Tile[2];
	public Tile[] MyVerticalNeighbors { get => myVerticalNeighbors; }
	public Tile[] MyHorizontalNeighbors { get => myHorizontalNeighbors; }

	private void Start()
    {
		SetMyNeighbors();
	}
    /// <summary>
    /// Set and show tile's item and lockCount.
    /// </summary>
    /// <param name="boardTile">Our tile's item and lockCount</param>
    public void SetMyBoardTile(BoardTile boardTile)
	{
		myBoardTile = boardTile;
		myImageIcon.sprite = myBoardTile.item.icon;


		myTextLock.text = myBoardTile.lockCount.ToString();
		myTextLock.gameObject.SetActive(myBoardTile.lockCount > 1);
		myImageLock.gameObject.SetActive(myBoardTile.lockCount != 0);

		if (myBoardTile.lockCount >= 1)
		{
			myImageLock.sprite = Canvas_Manager.Instance.LockIcon;
		}
        if (myBoardTile.isLocked)
        {
			Destroy(GetComponent<Button>());
        }
	}
	public void SetMyCoordinate(Vector2Int coordinate)
	{
		myCoordinate = coordinate;
	}
	public void SetMyNeighbors(bool setNeighborAgain = false)
	{
		(Tile[], Tile[]) myHVNeighbors = Board_Manager.Instance.LearnNeighbors(MyCoordinate);
		myVerticalNeighbors = myHVNeighbors.Item1;
		myHorizontalNeighbors = myHVNeighbors.Item2;
		if (setNeighborAgain)
		{
            for (int e = 0; e < myVerticalNeighbors.Length; e++)
			{
				if (myVerticalNeighbors[e] != null)
				{
					myVerticalNeighbors[e].SetMyNeighbors();
				}
				if (myHorizontalNeighbors[e] != null)
				{
					myHorizontalNeighbors[e].SetMyNeighbors();
				}
			}
		}
	}
	public void SetLockedCount()
    {
		myBoardTile.lockCount--;
		myTextLock.text = myBoardTile.lockCount.ToString();
		myTextLock.gameObject.SetActive(myBoardTile.lockCount > 1);
		myImageLock.gameObject.SetActive(myBoardTile.lockCount != 0);
	}
	// Tile prefabindaki butona atandı
	public void ChooseTile()
	{
		// Dont choose if tile locked.
		if (myBoardTile.isLocked)
		{
			return;
		}
		// Dont choose if tile lockedCount bigger than 0
		if (myBoardTile.lockCount > 0)
		{
			return;
		}
		// Dont choose if BoardManager do something.
		if (Board_Manager.Instance.WaitForChoosing)
		{
			return;
		}
		Board_Manager.Instance.ChooseTile(this);
	}
}