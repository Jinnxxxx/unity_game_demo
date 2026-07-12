using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList; //热键列表

    private float maxSize; //黑洞的最大大小
    private float growSpeed; //黑洞增长的速度
    private float shrinkSpeed; //黑洞缩小的速度
    private float blackholeTimer; //黑洞计时器（存在时间）

    private bool canGrow = true; //是否可以增长
    private bool canShrink; //是否可以缩小
    private bool canCreateHotKeys = true; //是否可以生成热键
    private bool cloneAttackReleased; //控制克隆攻击开关
    private bool playerCanDisapear = true; //控制玩家是否可以消失,默认为true

    private int amountOfAttacks = 4; //克隆攻击的次数
    private float cloneAttackCooldown = .3f; //克隆攻击的冷却时间
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>(); //触碰到的敌人列表
    private List<GameObject> createdHotKey = new List<GameObject>(); //生成的热键GameObject列表

    public bool playerCanExitState { get; private set; } //玩家是否可以退出黑洞状态

    // 传参初始化
    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _blackholeDuration;
    }


    void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        // 超过黑洞持续时间，若敌人列表不为空，释放克隆攻击，否则销毁黑洞
        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbility();
        }

        // 按下R键释放克隆攻击
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic(); //克隆攻击!!!(cloneAttackReleased = true)

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime); //线性插值，让黑洞大小逐渐增长到最大值
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime); //线性插值，让黑洞大小逐渐缩小到最小值

            if (transform.localScale.x < 0)
                Destroy(gameObject); //当黑洞大小缩小到最小值时，销毁黑洞
        }
    }

    // 克隆攻击
    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        DestroyHotKeys(); //销毁热键
        cloneAttackReleased = true; //进行克隆攻击
        canCreateHotKeys = false; //不能生成热键

        if (playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true); //使玩家透明
        }
    }

    // 克隆攻击逻辑
    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown; //设置攻击时间间隔

            int randomIndex = UnityEngine.Random.Range(0, targets.Count);

            // 偏移量设置
            float xOffset;
            if (UnityEngine.Random.Range(0, 50) > 50)
                xOffset = 2;
            else
                xOffset = -2;

            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0)); //随机克隆攻击
            amountOfAttacks--; //限制攻击次数

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", .5f); //延迟进行黑洞结束步骤
            }
        }
    }

    // 黑洞结束步骤
    private void FinishBlackHoleAbility()
    {
        DestroyHotKeys(); //销毁createdHotKey列表中的热键
        playerCanExitState = true; //克隆攻击耗尽后，玩家可以退出黑洞状态
        canShrink = true; //克隆攻击耗尽后，自动缩小黑洞
        cloneAttackReleased = false;
    }

    // 集中销毁热键
    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
            return;

        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true); //冻结敌人的时间

            CreateHotKey(collision); //敌人头上生成热键
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false); //解冻敌人的时间
    }

    // private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);


    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hotkeys in a keyCodeList!!");
            return;
        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity); //在敌人头顶生成一个hotkey实例
        createdHotKey.Add(newHotKey); //将生成的热键添加到生成的热键列表中

        KeyCode choosenKey = keyCodeList[UnityEngine.Random.Range(0, keyCodeList.Count)]; //随机选择一个热键
        keyCodeList.Remove(choosenKey); //移除已选择的热键

        Blackhole_Hotkey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_Hotkey_Controller>(); //获取Blackhole_Hotkey_Controller脚本

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this); //调用SetupHotKey方法，设置热键        
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform); //将按下快捷键的敌人添加到敌人列表中
}
