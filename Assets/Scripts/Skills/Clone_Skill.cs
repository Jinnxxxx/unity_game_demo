using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration; //克隆体
    [Space]
    [SerializeField] private bool canAttack; //克隆体是否可以攻击

    [SerializeField] private bool createCloneOnDashStart; //冲刺开始生成克隆
    [SerializeField] private bool createCloneOnDashOver; //冲刺结束生成克隆
    [SerializeField] private bool canCreateCloneOnCounterAttack; //反击成功生成克隆

    [Header("Clone can duplicate")]
    [SerializeField] private bool canDuplicateClone; //是否可以再次克隆
    [SerializeField] private float chanceToDuplicate; //再次克隆几率


    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        //GameObject newClone = Instantiate(clonePrefab);
        GameObject newClone = Instantiate(clonePrefab, player.transform.position, Quaternion.identity);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate);
    }

    //冲刺开始生成克隆 
    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    //冲刺结束生成克隆
    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    //反击成功生成克隆
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(player.facingDir * 2, 0))); //延迟调用
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
        CreateClone(_transform, _offset);
    }

}
