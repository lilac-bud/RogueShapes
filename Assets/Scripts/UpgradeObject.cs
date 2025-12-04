using Unity.VisualScripting;
using UnityEngine;

public class UpgradeObject : CellObject
{
    public enum Upgrade { Strengh, Defense };

    public Upgrade UpgradeType;
    public int AmountGranted = 10;
    public AudioClip upgradeClip;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        switch(UpgradeType)
        {
            case Upgrade.Strengh:
                GameManager.Instance.ChangeStrengh(AmountGranted);
                break;
            case Upgrade.Defense:
                GameManager.Instance.ChangeDefense(AmountGranted);
                break;
            default:
                break;
        }
        GameManager.Instance.PlayerController.PlaySound(upgradeClip);
    }
}
