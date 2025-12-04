using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;
    public AudioClip exitClip;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        GameManager.Instance.BoardManager.SetCellTile(coord, EndTile);
    }
    public override void PlayerEntered()
    {
        GameManager.Instance.PlayerController.PlaySound(exitClip);
        GameManager.Instance.NewLevel();
    }

}
