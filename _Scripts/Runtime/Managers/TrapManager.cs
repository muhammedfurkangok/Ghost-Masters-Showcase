using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public List<Trap> traps = new List<Trap>();
    
    
    public void AddTrap(Trap trap)
    {
        traps.Add(trap);
    }
    
    public void RemoveTrap(Trap trap)
    {
        traps.Remove(trap);
    }
}
