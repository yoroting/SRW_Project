using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyClassLibrary;

public class cCoolDown
{
    cUnitData Owner;        // owner
    public cCoolDown(cUnitData unit)
    {
        Pool = new Dictionary<int, int>();
        Owner = unit;
    }
    public Dictionary<int, int> Pool; // < skillid , sec >
    List<int> RemoveList;

    // clear all buff
    public void Reset()
    {
        Pool.Clear();
    }

    public int GetCD(int nSkillID)
    {
        if (Pool.ContainsKey(nSkillID))
        {
            return Pool[nSkillID];
        }
        return 0;
    }

    public void AddCD(int nSkillID, int nSec = 0)
    {
        if (nSec == 0)
        {
            // 沒指定的，撈技能預設資料
            cSkillData skill = GameDataManager.Instance.GetSkillData(nSkillID);
            if (skill != null)
            {
                nSec = skill.skill.n_CD;
            }
        }
        // 依然無CD～則不處理
        if (nSec == 0)
            return;

        if (Pool.ContainsKey(nSkillID))
        {
            Pool[nSkillID] = nSec;
        }
        else
        {
            // insert
            Pool.Add(nSkillID, nSec);
        }
        // 
    }

    public void RemoveCD(int nSkillID, int nSec = 0)
    {
        // 
        if (Pool.ContainsKey(nSkillID))
        {
            Pool[nSkillID] = Pool[nSkillID] - nSec;
            if (Pool[nSkillID] <= 0)
            {
                Pool.Remove(nSkillID);
            }
        }
    }


    // 經過一回合
    public void DecAll(int nSec = 1)
    {
        foreach (KeyValuePair<int, int> p in Pool)
        {
            int nNew = p.Value - nSec;
            if (p.Value <= 0)
            {
                RemoveList.Add(p.Key);
            }
            else
            {
                Pool[p.Key] = nNew;
            }
        }
        // remove cd
        foreach (int id in RemoveList)
        {
            Pool.Remove(id);
        }

    }

    // save 
    public List<cCDSaveData> ExportSavePool()
    {
        List<cCDSaveData> pool = new List<cCDSaveData>();
        foreach (KeyValuePair<int, int> pair in Pool)
        {
            pool.Add(new cCDSaveData(pair.Key, pair.Value));
        }
        return pool;
    }
    //load
    public void ImportSavePool(List<cCDSaveData> pool)
    {
        Pool.Clear();
        if (pool == null)
            return;

        foreach (cCDSaveData data in pool)
        {
            AddCD(data.nID, data.nTime);
        }
    }

}
