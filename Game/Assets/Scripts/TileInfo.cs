using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour, IPointerClickHandler
{
    private int xCoord, yCoord;
    private Sprite tileSprite;

    public delegate void OnTileClicked(int x, int y);
    public static event OnTileClicked OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClicked != null)
        {
            OnClicked(xCoord, yCoord);
        }
    }

    public void setCoord(int x, int y)
    {
        xCoord = x;
        yCoord = y;
    }

    public Vector2Int getCoord() 
    {
        return new Vector2Int(xCoord, yCoord);
    }

    public void setSprite(Sprite sprite)
    {
        tileSprite = sprite;
        GetComponent<Image>().sprite = sprite;
    }

    public Sprite getSprite()
    {
        return tileSprite;
    }
}
