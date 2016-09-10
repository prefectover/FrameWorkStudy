using UnityEngine;
using System.Collections;

/// <summary>
/// 网络消息
/// </summary>
public class QNetMsg 
{
    public int Type { get; set; }
    public int Length 
    {
        get
        {
            return this.Data.Length;
        }
    }
    public byte[] Data { get; set; }

    public QNetMsg(int type, byte[] data)
    {
        if (data == null)
        {
            Debug.LogError("NetMsg data is null");
            return;
        }
        this.Type = type;
        this.Data = data;
    }
}
