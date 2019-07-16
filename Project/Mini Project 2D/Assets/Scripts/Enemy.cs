using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemiesAI))]
public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemyStats
    {
        public int maxHealth=100;
        private int _curHealth;
        public int damage = 40;
        public int curHealth
        {
            get
            {
                return _curHealth;
            }
            set
            {
                _curHealth = Mathf.Clamp(value,0,maxHealth);
            }
        }

        public void Init()
        {
            curHealth = maxHealth;
        }
    }
    public string deathSoundName = "Explosion";

    public EnemyStats stats = new EnemyStats();

    [Header("Optional: ")]
    [SerializeField]
    public StatusIndicator statusIndicator;

    public Transform deathParticles;

    public float shakeAmt = 0.1f;
    public float shakeLength = 0.1f;

    private void Start()
    {
        stats.Init();
        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

        if(deathParticles == null)
        {
            Debug.LogError("no death particles");
        }
    }

    public int moneyDrop=10;

    void OnUpgradeMenuToggle(bool active)
    {
        GetComponent<EnemiesAI>().enabled = !active;
    }
    public void DamageEnemy(float damage)
    {
        stats.curHealth -= (int)damage;
        if (stats.curHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }
        if (statusIndicator != null)
        {
            statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        }
    }

    private void OnCollisionEnter2D(Collision2D _colliInfo)
    {
        Player _player = _colliInfo.collider.GetComponent<Player>();
        if (_player != null)
        {
            _player.DamagePlayer(stats.damage);
            DamageEnemy(999999);
        }
    }

    private void OnDestroy()
    {
        GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
    }
}
