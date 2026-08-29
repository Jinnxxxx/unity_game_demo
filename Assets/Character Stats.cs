using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength; // 角色力量值(每1点力量，增加1点伤害，增加1%暴击伤害)
    public Stat agility; // 角色敏捷值（每1点敏捷，增加1点闪避几率，增加1%暴击几率）
    public Stat intelligence; // 角色智力值（每1点智力，增加1点魔法伤害，增加3点魔法防御）
    public Stat vitality; // 角色活力值（每1点活力，增加3-5点hp）


    [Header("Offensive Stats")]
    public Stat damage; // 角色伤害值
    public Stat critChance; // 角色暴击几率
    public Stat critPower; // 角色暴击伤害值(default = 150%)


    [Header("Defensive Stats")]
    public Stat maxHealth; // 角色最大生命值
    public Stat armor; // 角色护甲
    public Stat evasion; // 角色闪避几率
    public Stat magicResistance; // 角色魔法防御值


    [Header("Magic Stats")]
    public Stat fireDamage; // 火系魔法伤害（持续造成伤害）
    public Stat iceDamage; // 冰系魔法伤害（减少20%护甲）
    public Stat lightingDamage; // 电系魔法伤害（减少20%命中率）


    public bool isIgnited; // 是否被点燃
    public bool isChilled; // 是否被冰冻
    public bool isShocked; // 是否被电击


    [SerializeField] private float ailmentsDuration = 4; // 默认持续时间
    private float ignitedTimer; // 点燃持续时间
    private float chilledTimer; // 冰冻持续时间
    private float shockedTimer; // 电击持续时间


    private float igniteDamageCooldown = .3f; // 点燃造成时间间隔（默认0.3）
    private float igniteDamageTimer; // 点燃造成伤害时间timer
    private int igniteDamage; // 点燃造成伤害值



    public int currentHealth; // 角色当前生命值

    public System.Action onHealthChanged; // 当角色生命值改变时触发的事件


    protected virtual void Start()
    {
        critPower.SetDefaultValue(150); // 设置默认暴击伤害为150%
        currentHealth = GetMaxHealthValue(); // 初始化当前生命值为最大生命值

        fx = GetComponent<EntityFX>();
    }


    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;


        if (igniteDamageTimer < 0 && isIgnited)
        {
            Debug.Log("take burn damage : " + igniteDamage);

            DecreaseHealthy(igniteDamage); // 减少血量（触发生命值改变事件）

            if (currentHealth <= 0)
            {
                isIgnited = false; //self_fix: 防止在死亡后继续点燃造成伤害
                Die();
            }

            igniteDamageTimer = igniteDamageCooldown;
        }

    }


    // 计算并造成伤害(是否闪避-初始化伤害-是否暴击-计算护甲-执行物理伤害)（执行魔法伤害）
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        // 判断是否闪避攻击
        if (TargetCanAvoidAttack(_targetStats))
            return;

        // 计算总伤害值
        int totalDamage = damage.GetValue() + strength.GetValue();

        // 判断并计算暴击
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        // 检查并计算护甲
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);



        // 执行目标角色受到物理伤害
        //_targetStats.TakeDamage(totalDamage);



        // 执行目标角色受到魔法伤害
        DoMagicalDamage(_targetStats);
    }


    // 计算和执行魔法伤害
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue(); //计算总魔法伤害值
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage); // 减去魔抗
        _targetStats.TakeDamage(totalMagicalDamage); // 执行目标角色受到魔法伤害


        // 不造成元素伤害直接return（避免后面while无限循环）
        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;


        // 判断为那种元素效果
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        // 如果多个效果伤害相同，则随机选择
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Ignite");
                return;
            }
            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Chill");
                return;
            }
            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("Shock");
                return;
            }
        }

        // 设置点燃伤害(20%的火焰伤害值)
        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        // 执行目标角色受到的负面效果(哪个效果伤害更高就执行哪个)
        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    // 检查并计算魔抗
    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3); // 减去魔抗（魔抗+智力x3）
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue); // 防止魔法伤害值为负数

        return totalMagicalDamage;
    }

    // 执行元素效果
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return;

        if (_ignite)
        {
            isIgnited = true;
            ignitedTimer = ailmentsDuration; // 点燃持续时间

            fx.IgniteFxFor(ailmentsDuration); // 触发点燃特效
        }

        if (_chill)
        {
            isChilled = true;
            chilledTimer = ailmentsDuration;

            float slowPercentage = .2f;
            
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration); // 减速20%
            fx.ChillFxFor(ailmentsDuration);
        }

        if (_shock)
        {
            isShocked = true;
            shockedTimer = ailmentsDuration;

            fx.ShockFxFor(ailmentsDuration);
        }
    }


    // 设置点燃造成伤害值
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    // 执行造成伤害
    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealthy(_damage); // 减少血量（触发生命值改变事件）

        if (currentHealth < 0)
            Die();

        Debug.Log(name + " took damage : " + _damage);

    }

    // 减少血量（触发生命值改变事件）(需要减少血量的时候使用此函数)
    public virtual void DecreaseHealthy(int _damage)
    {
        currentHealth -= _damage;   // 减少当前生命值

        onHealthChanged?.Invoke(); // 触发生命值改变事件
    }


    protected virtual void Die()
    { }

    // 判断目标角色是否闪避攻击
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue(); //计算总闪避几率

        if (isShocked)
            totalEvasion += 20; // 若自身受电击，增加目标角色闪避几率

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true; // 成功闪避攻击
        }

        return false; // 未闪避攻击
    }

    // 检查并计算护甲
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f); // 若目标角色chilled，护甲减少20%
        else
            totalDamage -= _targetStats.armor.GetValue(); // 正常减去护甲


        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue); // 防止伤害值为负数
        return totalDamage;
    }

    // 判断是否暴击
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    // 计算暴击伤害
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage); // 取整int
    }


    // 获取最大生命值
    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

}