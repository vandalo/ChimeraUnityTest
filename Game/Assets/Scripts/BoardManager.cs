using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> m_cropList = new List<Sprite>();
    [SerializeField]
    private GameObject m_tile;
    [SerializeField]
    private int m_xSize, m_ySize;
    [SerializeField]
    private Canvas m_canvas;
    [SerializeField]
    private float m_cooldownBetweenMatches;

    private GameObject[,] m_board;
    private GridLayoutGroup m_gridLayout;
    private Vector2Int m_selectedTile;
    private Vector2Int m_previousSelectedTile;
    private float m_currentCooldownBetweenMatches;
    private bool m_canPlay;

    private const int OFFSET = 10;
    private Color SELECTED_COLOR = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    public delegate void OnMatchSuccess(int score);
    public static event OnMatchSuccess OnMatch;

    public delegate void OnSwapSuccess();
    public static event OnSwapSuccess OnSwap;

    public delegate void OnNewRoundStart();
    public static event OnNewRoundStart NewRoundStart;


    void OnEnable()
    {
        InitBoard();
        NewRoundStart();
        TileInfo.OnClicked += OnTileClicked;
    }

    private void OnDisable()
    {
        TileInfo.OnClicked -= OnTileClicked;
        foreach(GameObject elem in m_board)
        {
            Destroy(elem);
        }
    }

    private void InitBoard()
    {
        PopulateBoard();
        m_gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        m_gridLayout.constraintCount = m_ySize;
        float cellSize = (m_canvas.GetComponent<RectTransform>().rect.width - OFFSET) / m_xSize;
        m_gridLayout.cellSize = new Vector2(cellSize, cellSize);
        m_selectedTile = new Vector2Int(-1, -1);
        m_previousSelectedTile = new Vector2Int(-1, -1);
        m_canPlay = true;
    }

    private void PopulateBoard()
    {
        m_board = new GameObject[m_xSize, m_ySize];

        float startX = transform.position.x;
        float startY = transform.position.y;

        for (int y = 0; y < m_ySize; y++)
        {
            for (int x = 0; x < m_xSize; x++)
            {
                GameObject newTile = Instantiate(m_tile, new Vector3(1, 1, 1), m_tile.transform.rotation);
                newTile.transform.SetParent(gameObject.transform);
                newTile.transform.localScale = new Vector3(1, 1, 1);
                m_board[x, y] = newTile;
                newTile.GetComponent<TileInfo>().setCoord(x,y);
                SetRandomTile(x, y);
            }
        }
    }

    private void SetRandomTile(int x, int y)
    {
        int randomSpriteId = Random.Range(0, m_cropList.Count);
        Sprite sprite = m_cropList[randomSpriteId];
        if(x > 0 && sprite == m_board[x - 1, y].GetComponent<TileInfo>().getSprite())
        {
            ++randomSpriteId;
            randomSpriteId = randomSpriteId % m_cropList.Count;
            sprite = m_cropList[randomSpriteId];
        }
        if (y > 0 && sprite == m_board[x, y - 1].GetComponent<TileInfo>().getSprite())
        {
            ++randomSpriteId;
            randomSpriteId = randomSpriteId % m_cropList.Count;
            sprite = m_cropList[randomSpriteId];
        }
        m_board[x, y].GetComponent<TileInfo>().setSprite(sprite);
    }

    private void OnTileClicked(int x, int y)
    {
        if(!m_canPlay)
        {
            return;
        }

        m_previousSelectedTile = m_selectedTile;
        m_selectedTile = new Vector2Int(x, y);
        if (IsSameAsPreviousTile(m_selectedTile))
        {
            UnselectTile(m_selectedTile);
        }
        else
        {
            SelectTile(m_selectedTile);
        }
    }

    private bool IsSameAsPreviousTile(Vector2Int newSelectedTile)
    {
        return m_previousSelectedTile == newSelectedTile;
    }

    private void SelectTile(Vector2Int tileToSelect)
    {
        if (tileToSelect.x >= 0)
        {
            m_board[tileToSelect.x, tileToSelect.y].GetComponent<Image>().color = SELECTED_COLOR;
            UnselectTile(m_previousSelectedTile);
            if(IsNeighbor(m_selectedTile, m_previousSelectedTile))
            {
                UnselectTile(m_selectedTile);
                SwapTiles(m_selectedTile, m_previousSelectedTile);
                UpdateBoard();
                m_selectedTile = m_previousSelectedTile = new Vector2Int(-1, -1);
            }
        }
    }

    private void UnselectTile(Vector2Int tileToUnselect)
    {
        if (tileToUnselect.x >= 0)
        {
            m_board[tileToUnselect.x, tileToUnselect.y].GetComponent<Image>().color = Color.white;
        }
    }

    private bool IsNeighbor(Vector2Int tileA, Vector2Int tileB)
    {
        bool isLeft = tileA.x - 1 == tileB.x && tileA.y == tileB.y;
        bool isRight = tileA.x + 1 == tileB.x && tileA.y == tileB.y;
        bool isUp = tileA.x == tileB.x && tileA.y - 1 == tileB.y;
        bool isDown = tileA.x == tileB.x && tileA.y + 1 == tileB.y;
        return isLeft || isRight || isUp || isDown;
    }

    private void SwapTiles(Vector2Int tileA, Vector2Int tileB)
    {
        Sprite spriteTileA = m_board[tileA.x, tileA.y].GetComponent<TileInfo>().getSprite();
        Sprite spriteTileB = m_board[tileB.x, tileB.y].GetComponent<TileInfo>().getSprite();
        m_board[tileA.x, tileA.y].GetComponent<TileInfo>().setSprite(spriteTileB);
        m_board[tileB.x, tileB.y].GetComponent<TileInfo>().setSprite(spriteTileA);
        OnSwap();
    }

    private void Update()
    {
        if (m_currentCooldownBetweenMatches > 0)
        {
            m_currentCooldownBetweenMatches -= Time.deltaTime;
            if (m_currentCooldownBetweenMatches <= 0)
            {
                m_canPlay = true;
                UpdateBoard();
            }
        }
    }

    private void UpdateBoard()
    {
        if (!m_canPlay)
        {
            return;
        }

        if(!CheckVerticalMatch())
        {
            CheckHorizontalMatch();
        }
    }

    private bool CheckVerticalMatch()
    {
        for (int x = 0; x < m_xSize; ++x)
        {
            int possibleMatchStart = 0;
            int countMatch = 1;
            for (int y = 0; y < m_ySize - 1; ++y)
            {
                if (m_board[x,y].GetComponent<TileInfo>().getSprite() == m_board[x, y + 1].GetComponent<TileInfo>().getSprite())
                {
                    ++countMatch;
                }
                else if(countMatch >= 3)
                {
                    ComputeVertialMatch(x, possibleMatchStart, countMatch);
                    return true;
                }
                else
                {
                    possibleMatchStart = y + 1;
                    countMatch = 1;
                }
            }
            if(countMatch >= 3)
            {
                ComputeVertialMatch(x, possibleMatchStart, countMatch);
                return true;
            }
        }
        return false;
    }

    private bool CheckHorizontalMatch()
    {
        for (int y = 0; y < m_ySize; ++y)
        {
            int possibleMatchStart = 0;
            int countMatch = 1;
            for (int x = 0; x < m_xSize - 1; ++x)
            {
                if (m_board[x, y].GetComponent<TileInfo>().getSprite() == m_board[x + 1, y].GetComponent<TileInfo>().getSprite())
                {
                    ++countMatch;
                }
                else if (countMatch >= 3)
                {
                    ComputeHorizontalMatch(possibleMatchStart, y, countMatch);
                    return true;
                }
                else
                {
                    possibleMatchStart = x + 1;
                    countMatch = 1;
                }
            }
            if (countMatch >= 3)
            {
                ComputeHorizontalMatch(possibleMatchStart, y, countMatch);
                return true;
            }
        }
        return false;
    }


    private void ComputeVertialMatch(int x, int yStartMatch, int countMatch)
    {
        for(int count = countMatch - 1; count >= 0; --count)
        {
            int y = yStartMatch + count;
            if (y - countMatch >= 0)
            {
                m_board[x, y].GetComponent<TileInfo>().setSprite(m_board[x, y - countMatch].GetComponent<TileInfo>().getSprite());
            }
        }
        for(int count = 0; count < countMatch; ++count)
        {
            SetRandomTile(x, count);
        }
        ComputeMatch(countMatch);
    }

    private void ComputeHorizontalMatch(int xStartMatch, int yMatch, int countMatch)
    {
        for(int count = 0; count < countMatch; ++count)
        {
            int x = xStartMatch + count;
            for (int y = yMatch; y > 0; --y)
            {
                m_board[x, y].GetComponent<TileInfo>().setSprite(m_board[x, y - 1].GetComponent<TileInfo>().getSprite());
            }
        }

        for (int count = 0; count < countMatch; ++count)
        {
            SetRandomTile(xStartMatch + count, 0);
        }
        ComputeMatch(countMatch);
    }

    private void ComputeMatch(int countMatch)
    {
        if (OnMatch != null)
        {
            OnMatch(countMatch);
        }
        m_currentCooldownBetweenMatches = m_cooldownBetweenMatches;
        m_canPlay = false;
    }
}
