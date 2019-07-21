using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;


    [SerializeField]
    private int maxLives = 3;
    public string respawnCountDownName = "RespawnCountDown";
    public string spawnSoundName = "Spawn";
    public string gameOverSoundName = "GameOver";

    private static int _remainingLives = 3;
    public static int RemainingLives
    {
        get
        {
            return _remainingLives;
        }
    }

    public Transform playerPrefab;
    public Transform spawnPoint;
    public Transform spawnPrefab;

    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject upgradeMenu;

    [SerializeField]
    private WaveSpawner waveSpawner;

    public delegate void UpgradeMenuCallBack(bool active);
    public UpgradeMenuCallBack onToggleUpgradeMenu;

    [SerializeField]
    private int startingMoney;
    public static int Money;

    private AudioManager audioManager;

    public float spawnDelay = 4;

    public CameraShake cameraShake;


    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    private void Start()
    {
        if (cameraShake == null)
        {
            Debug.LogError("no camera shake found");
        }
        _remainingLives = maxLives;

        Money = startingMoney;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audiomanager");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgradeMenu();
        }
        if (upgradeMenu == null)
        {
            upgradeMenu = GameObject.FindWithTag("UpgradeMenu");

        }
        if (gameOverUI == null)
        {
            gameOverUI = GameObject.FindWithTag("GameOver");
        }
    }

    private void ToggleUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        waveSpawner.enabled = !upgradeMenu.activeSelf;
        onToggleUpgradeMenu.Invoke(upgradeMenu.activeSelf);
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        gameOverUI.SetActive(true);
        audioManager.PlaySound(gameOverSoundName);
    }

    public IEnumerator RespawnPlayer()
    {
        audioManager.PlaySound(respawnCountDownName);
        //GetComponent<AudioSource>().Play(); 
        yield return new WaitForSeconds(spawnDelay);

        audioManager.PlaySound(spawnSoundName);

        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
        Destroy(clone.gameObject, 3f);
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        _remainingLives--;
        if (_remainingLives <= 0)
        {
            gm.EndGame();
        }
        else
        {
            gm.StartCoroutine(gm.RespawnPlayer());
        }
    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        audioManager.PlaySound(_enemy.deathSoundName);

        Money += _enemy.moneyDrop;
        audioManager.PlaySound("Money");
        Transform clone =  Instantiate(_enemy.deathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
        Destroy(clone.gameObject, 5f);
        Destroy(_enemy.gameObject);
        cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
    }
}
