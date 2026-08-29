using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // 使该类数据在编辑器中可见
public class Stat
{
    [SerializeField] private int baseValue;

    public List<int> modifiers; // 修改器列表
 
    public int GetValue()
    {
        int finalValue = baseValue;

        // 遍历修改器列表，累加修改器值到最终值上
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }


    // 设置默认值
    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }


    // 添加修改器
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    // 移除修改器
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }


}
