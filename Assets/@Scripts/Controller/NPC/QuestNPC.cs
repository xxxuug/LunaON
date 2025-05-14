using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    public int NPCID;   // 이 npc의 고유 id
    public List<int> QuestID = new(); // 이 npc가 관여하는 퀘스트 id들
}
