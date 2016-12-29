using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

namespace QFramework {
	/// <summary>
	/// 文本日志输出
	/// </summary>
	public class QFileLogOutput : ILogOutput
	{

		#if UNITY_EDITOR
		string mDevicePersistentPath = Application.dataPath + "/../PersistentPath";
		#elif UNITY_STANDALONE_WIN
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#elif UNITY_STANDALONE_OSX
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#else
		string mDevicePersistentPath = Application.persistentDataPath;
		#endif


		static string LogPath = "Log";

		private Queue<QLog.LogData> mWritingLogQueue = null;
		private Queue<QLog.LogData> mWaitingLogQueue = null;
		private object mLogLock = null;
		private Thread mFileLogThread = null;
		private bool mIsRunning = false;
		private StreamWriter mLogWriter = null;

		public QFileLogOutput()
		{
			QApp.Instance.onApplicationQuit += Close;
			this.mWritingLogQueue = new Queue<QLog.LogData>();
			this.mWaitingLogQueue = new Queue<QLog.LogData>();
			this.mLogLock = new object();
			System.DateTime now = System.DateTime.Now;
			string logName = string.Format("Q{0}{1}{2}{3}{4}{5}",
				now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		string logPath = string.Format("{0}/{1}/{2}.txt", mDevicePersistentPath, LogPath, logName);
			if (File.Exists(logPath))
				File.Delete(logPath);
			string logDir = Path.GetDirectoryName(logPath);
			if (!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);
			this.mLogWriter = new StreamWriter(logPath);
			this.mLogWriter.AutoFlush = true;
			this.mIsRunning = true;
			this.mFileLogThread = new Thread(new ThreadStart(WriteLog));
			this.mFileLogThread.Start();
		}

		void WriteLog()
		{
			while (this.mIsRunning)
			{
				if (this.mWritingLogQueue.Count == 0)
				{
					lock (this.mLogLock)
					{
						while (this.mWaitingLogQueue.Count == 0)
							Monitor.Wait(this.mLogLock);
						Queue<QLog.LogData> tmpQueue = this.mWritingLogQueue;
						this.mWritingLogQueue = this.mWaitingLogQueue;
						this.mWaitingLogQueue = tmpQueue;
					}
				}
				else
				{
					while (this.mWritingLogQueue.Count > 0)
					{
						QLog.LogData log = this.mWritingLogQueue.Dequeue();
						if (log.Level == QLog.LogLevel.ERROR)
						{
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------");
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log + "\n");
							this.mLogWriter.WriteLine(log.Track);
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------"); 
						}
						else
						{
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log);
						}
					}
				}
			}
		}

		public void Log(QLog.LogData logData)
		{
			lock (this.mLogLock)
			{
				this.mWaitingLogQueue.Enqueue(logData);
				Monitor.Pulse(this.mLogLock);
			}
		}

		public void Close()
		{
			this.mIsRunning = false;
			this.mLogWriter.Close();
		}
	}
}