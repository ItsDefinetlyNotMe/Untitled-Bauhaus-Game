using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;
    
    [TextArea(5, 15)]
    public string[] firstTimeSentences;
    
    [TextArea(5, 15)]
    public string[] loopSentences;
}
