using System;


// 对外通信 赵写的代码
public enum UIEventZhao 
{
	Load = QMgrID.UI,

	Register,

	MaxValue
}

//
public enum UIEventZhou
{
	NPCAttack = UIEventZhao.MaxValue,

	NPCBlood,

	MaxValue
}

