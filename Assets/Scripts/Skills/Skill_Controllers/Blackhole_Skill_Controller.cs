using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    public float maxSize;
    public float growSpeed;
    public bool canGrow;

    public List<Transform> targets; //触碰到的敌人列表

    void Update()
    {
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime); //线性插值，让黑洞大小逐渐增长到最大值
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true); //冻结敌人的时间
            
            // 在敌人头顶生成一个prefab
            // targets.Add(collision.transform);
        }
    }

}
