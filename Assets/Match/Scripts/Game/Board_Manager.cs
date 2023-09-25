using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

/// <summary>
/// It is used to replace tiles after they are destroyed.
/// </summary>
[Serializable]
public class SwapTile
{
	public List<Tile> swapTile = new List<Tile>();
	public int selectedTilePosYPower;

	public void AddTile(Tile tile)
    {
		selectedTilePosYPower++;
		swapTile.Add(tile);
	}
}
[Serializable]
public class BoardTile
{
	public Item item;
	public int lockCount;
	public bool isLocked;

	public BoardTile(Item item, int lockCount = 0, bool isLocked = false)
	{
		this.item = item;
		this.isLocked = isLocked;
		this.lockCount = lockCount;
	}
}
public class Board_Manager : MonoBehaviour
{
    private static Board_Manager instance;
	public static Board_Manager Instance { get => instance; }

	private const float DoTweenDuration = 0.25f;
	//[SerializeField, Range(0f, 1f)] private float lockedChance;
	[SerializeField, Range(0f, 1f)] private float lockedCountChance = 0.1f;
	[SerializeField] private List<Item> boardItemList = new List<Item>();

	private int score;
	private int scoreAdd = 0;
    private int scoreMulti = 0;
    private int width;
	private int height;
	private bool waitForChoosing;
	private List<Tile> choosedTiles = new List<Tile>();

	private Tile[,] myTile;
	private List<SwapTile> swapTiles = new List<SwapTile>();
	public List<Item> BoardItemList { get => boardItemList; }
	public Tile[,] Tiles { get => myTile; }
	public int Width { get => width; }
	public int Height { get => height; }
	public int ScoreAdd { get => scoreAdd; }
	public int Score { get => score; set => score = value; }
	public bool WaitForChoosing { get => waitForChoosing; }

	[ContextMenu("Clear Board ChoosedTiles")]
	private void ClearBoardChoosedTiles()
	{
		ClearMatchControlling();
	}
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
    }

    /// <summary>
    /// Create a tile board
    /// </summary>
    public void SetBoard(Vector2Int boardSize, Vector3 boardPosition)
	{
		width = boardSize.x;
		height = boardSize.y;
		myTile = new Tile[width, height];
		choosedTiles.Clear();
		swapTiles.Clear();
		for (int e = 0; e < width; e++)
		{
			swapTiles.Add(new SwapTile());
		}

		Canvas_Manager.Instance.SetBoardView(myTile, boardPosition);
        FindNewMatch();
    }
	public void SetTiles(Vector2Int tileCoordinate, Tile tile)
	{
		Item item = BoardItemList[Random.Range(0, BoardItemList.Count)];
		int lockedCount = Random.value < lockedCountChance ? Random.Range(1, 3) : 0;
		//bool isLocked = Random.value < lockedChance ? true : false;
		//tile.SetMyBoardTile(new BoardTile(item));
		tile.SetMyBoardTile(new BoardTile(item, lockedCount));
		//tile.SetMyBoardTile(new BoardTile(item, lockedCount, isLocked));
		tile.SetMyCoordinate(tileCoordinate);

		myTile[tileCoordinate.x, tileCoordinate.y] = tile;
	}

	/// <summary>
	/// You can learn special tiles all neighbors
	/// </summary>
	/// <param name="myCoordinate">Coordinate of the tile whose neighbors you want to know.</param>
	/// <returns>First item is vertical neighbors, second item is horizontal neighbors.</returns>
	public (Tile[], Tile[]) LearnNeighbors(Vector2Int myCoordinate)
	{
		Tile myLeftNeighbor = myCoordinate.x == 0 ? null : Tiles[myCoordinate.x - 1, myCoordinate.y];
		Tile myRightNeighbor = myCoordinate.x == Width - 1 ? null : Tiles[myCoordinate.x + 1, myCoordinate.y];
		Tile myUpNeighbor = myCoordinate.y == 0 ? null : Tiles[myCoordinate.x, myCoordinate.y - 1];
		Tile myDownNeighbor = myCoordinate.y == Height - 1 ? null : Tiles[myCoordinate.x, myCoordinate.y + 1];
		Tile[] myVNeighbors = new Tile[2] { myUpNeighbor, myDownNeighbor };
		Tile[] myHNeighbors = new Tile[2] { myLeftNeighbor, myRightNeighbor };

		(Tile[], Tile[]) myAllNeighbors;
		myAllNeighbors.Item1 = myVNeighbors;
		myAllNeighbors.Item2 = myHNeighbors;
		return myAllNeighbors;
	}

	/// <summary>
	/// Choose tile for swaping
	/// </summary>
	/// <param name="tile">Choosing Tile</param>
	public void ChooseTile(Tile tile)
	{
		if (choosedTiles.Count == 0)
		{
			choosedTiles.Add(tile);
			EffectFirstChoosedTile();
		}
		else if (choosedTiles.Contains(tile))
		{
			choosedTiles.Remove(tile);
		}
		else
		{
			// Are they Neighbors? 
            if (!AreWeNeighbor(choosedTiles[0], tile))
			{
				return;
			}
			choosedTiles.Add(tile);
			waitForChoosing = true;
			DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 3);
			SwapTile(choosedTiles[0], choosedTiles[1]);
		}
	}
	private void EffectFirstChoosedTile()
    {
        Transform firstTile = choosedTiles[0].transform;
        firstTile.DOScale(Vector3.one * 1.25f, DoTweenDuration).OnComplete(() =>
		{
			if (firstTile != null)
			{
				firstTile.DOScale(Vector3.one, DoTweenDuration).OnComplete(() =>
				{
					if (firstTile != null)
					{
						EffectFirstChoosedTile();
					}
				});
			}
		});
    }
	/// <summary>
	/// Check tiles for neighbor. If they are not neighbor, don't choose tile.
	/// </summary>
	/// <returns>True is they are neighbors, false is they are not neighbors.</returns>
	private bool AreWeNeighbor(Tile tile1, Tile tile2)
	{
        if (Array.IndexOf(tile1.MyHorizontalNeighbors, tile2) != -1)
		{
			return true;
		}
        else if (Array.IndexOf(tile1.MyVerticalNeighbors, tile2) != -1)
		{
			return true;
		}
        return false;
    }
	
	[ContextMenu("Swap Tile")]
	private void SwapTile()
	{
		SwapTile(choosedTiles[0], choosedTiles[1], false);
	}
	/// <summary>
	/// Swap choosing tile
	/// </summary>
	private void SwapTile(Tile tile1, Tile tile2, bool checkMatch = true)
	{
		// Swap tile1
		var rectTransformTile1 = tile1.GetComponent<RectTransform>();
		var rectTransformTile2 = tile2.GetComponent<RectTransform>();
		var coordinateTile1 = rectTransformTile1.anchoredPosition;
		var coordinateTile2 = rectTransformTile2.anchoredPosition;

        var sequence = DOTween.Sequence();
		sequence.Join(rectTransformTile1.DOAnchorPos(coordinateTile2, DoTweenDuration))
				.Join(rectTransformTile1.DOScale(1.25f * Vector3.one, DoTweenDuration * 0.5f))
				.Join(DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 0.5f)
					.OnComplete(() =>
					{
						rectTransformTile1.DOScale(Vector3.one, DoTweenDuration * 0.5f);
					}))
				.Join(rectTransformTile2.DOAnchorPos(coordinateTile1, DoTweenDuration))
				.Join(rectTransformTile2.DOScale(1.25f * Vector3.one, DoTweenDuration * 0.5f))
				.Join(DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 0.5f)
					.OnComplete(() =>
					{
						rectTransformTile2.DOScale(Vector3.one, DoTweenDuration * 0.5f);
					}));
		sequence.Play().OnComplete(() =>
		{
			var coordinate1 = tile1.MyCoordinate;
			var coordinate2 = tile2.MyCoordinate;

			// Set Tiles Coordinate
			tile1.SetMyCoordinate(coordinate2);
			tile2.SetMyCoordinate(coordinate1);

			// Set Tiles Name
			var newName = tile1.name;
			tile1.name = tile2.name;
			tile2.name = newName;

			// Set Neighbors
			Tiles[tile1.MyCoordinate.x, tile1.MyCoordinate.y] = tile1;
			Tiles[tile2.MyCoordinate.x, tile2.MyCoordinate.y] = tile2;

			tile1.SetMyNeighbors(true);
			tile2.SetMyNeighbors(true);

            if (checkMatch)
			{
				CheckMatch();
			}
		});
	}

	/// <summary>
	/// We check board starting choosing tiles for match.
	/// </summary>
	private void CheckMatch()
	{
		DOTween.Kill(choosedTiles[0].gameObject);
		List<Tile> controllingList = new List<Tile>();
		List<Tile> checkedList = new List<Tile>();
		List<Tile> selected = new List<Tile>();
		scoreMulti = 0;
		// Checking 0. Element - Horizontal
		List<Tile> firstElementList = new List<Tile>();
		Tile checkingTile = choosedTiles[0];
		controllingList.Add(checkingTile);
		checkedList = new List<Tile>();
        for (int e = 0; e < controllingList.Count; e++)
        {
            if (!checkedList.Contains(controllingList[e]))
            {
                for (int h = 0; h < controllingList[e].MyHorizontalNeighbors.Length; h++)
                {
                    if (controllingList[e].MyHorizontalNeighbors[h] == null)
                    {
                        continue;
                    }
                    if (controllingList[e].MyHorizontalNeighbors[h].MyBoardTile.item == checkingTile.MyBoardTile.item)
                    {
                        if (!controllingList.Contains(controllingList[e].MyHorizontalNeighbors[h]))
                        {
                            controllingList.Add(controllingList[e].MyHorizontalNeighbors[h]);
                        }
                    }
                    if (!checkedList.Contains(controllingList[e]))
                    {
                        checkedList.Add(controllingList[e]);
                    }
                }
            }
		}
        if (controllingList.Count > 2)
		{
			// We found horizontal match for 0. Element
			firstElementList.AddRange(controllingList);
			AddScore(controllingList[0].MyBoardTile.item.point, controllingList.Count);
		}

		// Checking 0. Element - Vertical
		controllingList.Clear();
		checkedList.Clear();
		checkingTile = choosedTiles[0];
		controllingList.Add(checkingTile);
        for (int e = 0; e < controllingList.Count; e++)
		{
			if (!checkedList.Contains(controllingList[e]))
			{
				for (int h = 0; h < controllingList[e].MyVerticalNeighbors.Length; h++)
				{
					if (controllingList[e].MyVerticalNeighbors[h] == null)
					{
						continue;
					}
					if (controllingList[e].MyVerticalNeighbors[h].MyBoardTile.item == checkingTile.MyBoardTile.item)
					{
						if (!controllingList.Contains(controllingList[e].MyVerticalNeighbors[h]))
						{
							controllingList.Add(controllingList[e].MyVerticalNeighbors[h]);
						}
					}
					if (!checkedList.Contains(controllingList[e]))
					{
						checkedList.Add(controllingList[e]);
					}
				}
			}
		}
		if (controllingList.Count > 2)
		{
			if (firstElementList.Count > 2)
			{
				// We found vertical match for 0. Element but we found horizontal match so we skip first item because we add in horizontal match 
				firstElementList.AddRange(controllingList.Skip(1));
			}
            else
			{
				// We found vertical match for 0. Element
				firstElementList.AddRange(controllingList);
			}
			AddScore(controllingList[0].MyBoardTile.item.point, controllingList.Count);
		}

		// Checking 1. Element - Horizontal
		List<Tile> secondElementList = new List<Tile>();
		controllingList.Clear();
		checkedList.Clear();
		checkingTile = choosedTiles[1];
		controllingList.Add(checkingTile);
		for (int e = 0; e < controllingList.Count; e++)
		{
			if (!checkedList.Contains(controllingList[e]))
			{
				for (int h = 0; h < controllingList[e].MyHorizontalNeighbors.Length; h++)
				{
					if (controllingList[e].MyHorizontalNeighbors[h] == null)
					{
						continue;
					}
					if (controllingList[e].MyHorizontalNeighbors[h].MyBoardTile.item == checkingTile.MyBoardTile.item)
					{
						if (!controllingList.Contains(controllingList[e].MyHorizontalNeighbors[h]))
						{
							controllingList.Add(controllingList[e].MyHorizontalNeighbors[h]);
						}
					}
					if (!checkedList.Contains(controllingList[e]))
					{
						checkedList.Add(controllingList[e]);
					}
				}
			}
		}
		if (controllingList.Count > 2)
		{
			// We found vertical match for 1. Element
			secondElementList.AddRange(controllingList);
			AddScore(controllingList[0].MyBoardTile.item.point, controllingList.Count);
		}

		// Checking 1. Element - Vertical
		controllingList.Clear();
		checkedList.Clear();
		checkingTile = choosedTiles[1];
		controllingList.Add(checkingTile);
		for (int e = 0; e < controllingList.Count; e++)
		{
			if (!checkedList.Contains(controllingList[e]))
			{
				for (int h = 0; h < controllingList[e].MyVerticalNeighbors.Length; h++)
				{
					if (controllingList[e].MyVerticalNeighbors[h] == null)
					{
						continue;
					}
					if (controllingList[e].MyVerticalNeighbors[h].MyBoardTile.item == checkingTile.MyBoardTile.item)
					{
						if (!controllingList.Contains(controllingList[e].MyVerticalNeighbors[h]))
						{
							controllingList.Add(controllingList[e].MyVerticalNeighbors[h]);
						}
					}
					if (!checkedList.Contains(controllingList[e]))
					{
						checkedList.Add(controllingList[e]);
					}
				}
			}
		}
		if (controllingList.Count > 2)
		{
			if (secondElementList.Count > 2)
			{
				// We found vertical match for 1. Element but we found horizontal match so we skip first item because we add in horizontal match 
				secondElementList.AddRange(controllingList.Skip(1));
			}
			else
			{
				// We found vertical match for 1. Element
				secondElementList.AddRange(controllingList);
			}
			AddScore(controllingList[0].MyBoardTile.item.point, controllingList.Count);
		}

		selected.AddRange(firstElementList);
		selected.AddRange(secondElementList);

        if (selected.Count > 0)
        {
			// Delete selected tile
			int matchingTile = 0;
			foreach (var tile in selected)
			{
				Transform tileTransform = tile.transform.GetComponent<Transform>();
				if (tile.MyBoardTile.lockCount == 0)
				{
					tileTransform.DOScale(1.5f * Vector3.one, DoTweenDuration * 0.5f);
					DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 0.5f)
						.OnComplete(() =>
						{
							tileTransform.DOScale(0.25f * Vector3.one, DoTweenDuration * 0.5f).OnComplete(() =>
							{
								// Tile not locked so delete and create new tile
								Item item = boardItemList[Random.Range(0, boardItemList.Count)];
								Tile swapTile = Canvas_Manager.Instance.CreateTiles(tile.MyCoordinate.x, Height + 1 + swapTiles[tile.MyCoordinate.x].selectedTilePosYPower);
								swapTile.SetMyBoardTile(new BoardTile(item));
								swapTiles[tile.MyCoordinate.x].AddTile(swapTile);
								Tiles[tile.MyCoordinate.x, tile.MyCoordinate.y] = null;
								Destroy(tile.gameObject);
								matchingTile++;
								if (matchingTile == selected.Count)
								{
									SendDownTiles();
								}
							});
						});
				}
				else
				{
					tile.SetLockedCount();
					matchingTile++;
					if (matchingTile == selected.Count)
					{
						SendDownTiles();
					}
				}
			}
		}
        else
		{
			SwapTile(choosedTiles[0], choosedTiles[1], false);
			ClearMatchControlling();
		}
	}
	private void AddScore(int point, int controllingListCount)
	{
		scoreMulti++;
		Canvas_Manager.Instance.OpenHitCountAnimation(scoreMulti);
		scoreAdd += point * scoreMulti * controllingListCount;
	}
	public void ClearMatchControlling()
	{
		waitForChoosing = false;
		choosedTiles.Clear();
		swapTiles.Clear();
		score += scoreAdd;
		scoreAdd = 0;
		for (int e = 0; e < width; e++)
		{
			swapTiles.Add(new SwapTile());
		}
	}
	/// <summary>
	/// Send new tiles and tiles over null places into the null places.
	/// </summary>
	private void SendDownTiles()
	{
		//UnityEditor.EditorApplication.isPaused = true;
		List<Tile> downedTiles = new List<Tile>();
		for (int e = 0; e < Width; e++)
		{
			int posY = 0;
			bool tileSwaped = false;
			for (int h = 0; h < Height; h++)
			{
				if (Tiles[e, h] != null)
				{
					bool isSwap = false;
                    for (int c = 0; c < h && !isSwap; c++)
					{
						if (Tiles[e, c] == null)
						{
							// Swap tiles from full place to null place
							isSwap = true;
							tileSwaped = true;
							posY = c;
							Tiles[e, c] = Tiles[e, h];
							Tiles[e, h] = null;
							Tiles[e, c].SetMyCoordinate(new Vector2Int(e, c));
							Tiles[e, c].name = "Tile > x : " + e + " --- y : " + c;
							downedTiles.Add(Tiles[e, c]);
						}
					}
				}
                else
				{
					// We cant swap if cant find full place top side.
                    if (!tileSwaped)
					{
						tileSwaped = true;
						posY = h - 1;
					}
				}
			}
            if (tileSwaped)
			{
				// We set new tiles and spawed tiles coordinate and places.
				for (int i = 0; i < swapTiles[e].swapTile.Count; i++)
				{
					Tiles[e, posY + 1] = swapTiles[e].swapTile[i];
					Tiles[e, posY + 1].SetMyCoordinate(new Vector2Int(e, posY + 1));
					Tiles[e, posY + 1].name = "Tile > x : " + e + " --- y : " + (posY + 1);
					downedTiles.Add(Tiles[e, posY + 1]);
					posY++;
				}
            }
		}
        // Send to all tiles from top side (full place) to down side (null places)
        for (int e = 0; e < downedTiles.Count; e++)
        {
            Vector2 endPos = new Vector2(downedTiles[e].MyCoordinate.x, downedTiles[e].MyCoordinate.y) * 100 + Vector2.one * 50;
			RectTransform rect = Tiles[downedTiles[e].MyCoordinate.x, downedTiles[e].MyCoordinate.y].GetComponent<RectTransform>();
			
			rect.DOAnchorPos(endPos, DoTweenDuration * 0.5f);
            Tiles[downedTiles[e].MyCoordinate.x, downedTiles[e].MyCoordinate.y].SetMyNeighbors(true);
        }

		DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 0.5f)
			.OnComplete(() =>
			{
				FindNewMatch();
            });
		for (int e = 0; e < swapTiles.Count; e++)
		{
			swapTiles[e] = new SwapTile();
		}
	}
	private void FindNewMatch()
	{
		List<Tile> newGeneralMatchElementList = new List<Tile>();
		List<Tile> newHorizontalMatchElementList = new List<Tile>();
		List<Tile> newVerticalMatchElementList = new List<Tile>();
		for (int h = 0; h < Width; h++)
        {
            for (int e = 0; e < Height; e++)
			{
				// We can not check some tiles in last 2 horizontal lines.
				// If they are match, they will be already in last 3rd tiles match.
				if (h < Width - 2)
				{
					if (!newHorizontalMatchElementList.Contains(Tiles[h, e]))
					{
						Tile checkingTile = Tiles[h, e];
						List<Tile> horizontalElementList = new List<Tile>() { checkingTile };
						// Horizontal Check
						bool horizontalCheck = true;
						int nextHorizontalAmount = h + 1;
						while (horizontalCheck)
						{
							// Check next tiles item
							if (checkingTile.MyBoardTile.item == Tiles[nextHorizontalAmount, e].MyBoardTile.item)
							{
								horizontalElementList.Add(Tiles[nextHorizontalAmount, e]);
								// Check height limit
								if (nextHorizontalAmount != Width - 1)
								{
									// Change checking tiles amount for next tiles
									nextHorizontalAmount++;
								}
								else
								{
									// Finish Width tiles
									horizontalCheck = false;
								}
							}
							else
							{
								// not same next tile's item
								horizontalCheck = false;
							}
						}
						// Check horizontal match list
						if (horizontalElementList.Count > 2)
						{
							AddScore(horizontalElementList[0].MyBoardTile.item.point, horizontalElementList.Count);
							newHorizontalMatchElementList.AddRange(horizontalElementList);
						}
					}
				}
				// We can not check some tiles in last 2 vertical lines.
				// If they are match, they will be already in last 3rd tiles match.
				if (e < Height - 2)
				{
					if (!newVerticalMatchElementList.Contains(Tiles[h, e]))
					{
						Tile checkingTile = Tiles[h, e];
						List<Tile> verticalElementList = new List<Tile>() { checkingTile };
						// Vertical Check
						bool verticalCheck = true;
						int nextVerticalAmount = e + 1;
						while (verticalCheck)
						{
							// Check next tiles item
							if (checkingTile.MyBoardTile.item == Tiles[h, nextVerticalAmount].MyBoardTile.item)
							{
								verticalElementList.Add(Tiles[h, nextVerticalAmount]);
								// Check height limit
								if (nextVerticalAmount != Height - 1)
								{
									// Change checking tiles amount for next tiles
									nextVerticalAmount++;
								}
								else
								{
									// Finish width tiles
									verticalCheck = false;
								}
							}
							else
							{
								// not same next tile's item
								verticalCheck = false;
							}
						}
						// Check vertical match list
						if (verticalElementList.Count > 2)
						{
							AddScore(verticalElementList[0].MyBoardTile.item.point, verticalElementList.Count);
							newVerticalMatchElementList.AddRange(verticalElementList);
						}
					}
				}
            }
		}
		// Add general list to horizonta list and vertical list
		newGeneralMatchElementList.AddRange(newHorizontalMatchElementList);
		newGeneralMatchElementList.AddRange(newVerticalMatchElementList);

		// FarklıList1'de farklı olan tüm elementleri FarklıList2'ye ekler.
		newGeneralMatchElementList = newGeneralMatchElementList.Distinct().ToList();

		if (newGeneralMatchElementList.Count > 2)
        {
			// Destroy tiles in list
			int matchingTile = 0;
			foreach (var tile in newGeneralMatchElementList)
			{
				Transform tileTransform = tile.transform.GetComponent<Transform>();
				if (tile.MyBoardTile.lockCount == 0)
				{
					tileTransform.DOScale(1.5f * Vector3.one, DoTweenDuration * 0.5f);
					DOTween.To(value => { }, startValue: 0, endValue: 1, duration: DoTweenDuration * 0.5f)
						.OnComplete(() =>
						{
							tileTransform.DOScale(0.25f * Vector3.one, DoTweenDuration * 0.5f).OnComplete(() =>
							{
								// Tile not locked so delete and create new tile
								Item item = boardItemList[Random.Range(0, boardItemList.Count)];
								Tile swapTile = Canvas_Manager.Instance.CreateTiles(tile.MyCoordinate.x, Height + 1 + swapTiles[tile.MyCoordinate.x].selectedTilePosYPower);
								swapTile.SetMyBoardTile(new BoardTile(item));
								swapTiles[tile.MyCoordinate.x].AddTile(swapTile);
								Tiles[tile.MyCoordinate.x, tile.MyCoordinate.y] = null;
								Destroy(tile.gameObject);
								matchingTile++;
								if (matchingTile == newGeneralMatchElementList.Count)
								{
									SendDownTiles();
								}
							});
						});
				}
				else
				{
					tile.SetLockedCount();
					matchingTile++;
					if (matchingTile == newGeneralMatchElementList.Count)
					{
						SendDownTiles();
					}
				}
			}
		}
        else
		{
			Canvas_Manager.Instance.SetScore();
		}
    }
}