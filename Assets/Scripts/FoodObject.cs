using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;
    public AudioClip foodClip;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        GameManager.Instance.ChangeFood(AmountGranted);
        GameManager.Instance.PlayerController.PlaySound(foodClip);
    }
}
