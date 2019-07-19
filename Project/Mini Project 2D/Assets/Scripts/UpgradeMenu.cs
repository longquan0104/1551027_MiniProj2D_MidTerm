using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour
{

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Text speedText;

    [SerializeField]
    private float healthMultiplier = 1.3f;

    [SerializeField]
    private float movementSpeedMultiplier = 1.3f;

    [SerializeField]
    private int upgradeCost = 50;

    [SerializeField]
    private int upgradeGun = 10;

    private PlayerStats stats;

    private Weapon weapon;

    void OnEnable()
    {
        stats = PlayerStats.instance;
        weapon = Weapon.instance;
        UpdateValues();
    }

    void UpdateValues()
    {
        healthText.text = "HEALTH: " + stats.maxHealth.ToString();
        speedText.text = "SPEED: " + stats.movementSpeed.ToString();
    }

    public void UpgradeHealth()
    {
        if (GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("Money");
            return;
        }

        stats.maxHealth = (int)(stats.maxHealth * healthMultiplier);

        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("NoMoney");

        UpdateValues();
    }

    public void UpgradeSpeed()
    {
        if (GameMaster.Money < upgradeCost)
        {
            AudioManager.instance.PlaySound("Money");
            return;
        }

        stats.movementSpeed = Mathf.Round(stats.movementSpeed * movementSpeedMultiplier);

        GameMaster.Money -= upgradeCost;
        AudioManager.instance.PlaySound("NoMoney");

        UpdateValues();
    }
    public void UpgradeGun()
    {
        if (weapon.fireRate < 100)
        {
            if (GameMaster.Money < upgradeGun)
            {
                AudioManager.instance.PlaySound("Money");
                return;
            }

            stats.movementSpeed = Mathf.Round(stats.movementSpeed * movementSpeedMultiplier);
            weapon.fireRate = 100;
            GameMaster.Money -= upgradeGun;

            AudioManager.instance.PlaySound("NoMoney");
        }
    }

    public void ToMenu()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("MainMenu");
    }

}
