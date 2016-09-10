using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QFramework {
	/// <summary>
	/// 日志输出接口
	/// </summary>
	public interface ILogOutput
	{
		/// <summary>
		/// 输出日志数据
		/// </summary>
		/// <param name="logData">日志数据</param>
		void Log(QLog.LogData logData);
		/// <summary>
		/// 关闭
		/// </summary>
		void Close();
	}
}