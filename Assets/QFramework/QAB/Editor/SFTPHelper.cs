using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace QFramework
{
	class SFTPHelper:IFTPInterface
	{
		private Session m_session;
		private Channel m_channel;
		private ChannelSftp m_sftp;

		//host:sftp地址   user：用户名   pwd：密码         
		public SFTPHelper(string ip, string port, string user, string pwd)
		{
			UnityEngine.Debug.Log("SFTP ip:" + ip + " port:" + port);

			int serverport = Int32.Parse(port);

			JSch jsch = new JSch();
			m_session = jsch.getSession(user, ip, serverport);

			MyUserInfo ui = new MyUserInfo();
			ui.setPassword(pwd);
			m_session.setUserInfo(ui);
		}

		//SFTP连接状态         
		public bool Connected { get { return m_session.isConnected(); } }

		//连接SFTP         
		public bool Connect()
		{
			try
			{
				if (!Connected)
				{
					m_session.connect();
					m_channel = m_session.openChannel("sftp");
					m_channel.connect();
					m_sftp = (ChannelSftp)m_channel;
				}
				return true;
			}
			catch(Exception ex)
			{
				UnityEngine.Debug.Log("SFTP连接异常！" + ex.ToString());
				return false;
			}
		}

		//断开SFTP         
		public void Disconnect()
		{
			if (Connected)
			{
				m_channel.disconnect();
				m_session.disconnect();
			}
		}

		//SFTP存放文件         
		public bool Upload(string localPath, string remotePath)
		{
			try
			{
				Connect();
				if (this.Connected)
				{
					Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
					Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
					m_sftp.put(src, dst);
					return true;
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("SFTP上传文件错误！" + ex.ToString());
				return false;
			}
			return false;
		}

		//SFTP获取文件         
		public bool Get(string remotePath, string localPath)
		{
			try
			{
				Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
				Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
				m_sftp.get(src, dst);
				return true;
			}
			catch(Exception io)
			{
				UnityEngine.Debug.Log (io.StackTrace);
				UnityEngine.Debug.Log (io.Data);
				UnityEngine.Debug.Log (io.Message);
				return false;
			}
		}

		public bool Rename(string oldPath,string newPath){
			try
			{
				Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(oldPath);
				Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(newPath);
				m_sftp.rename(src,dst);
				return true;
			}
			catch{
				return false;
			}
		}

		public bool MakeDir(string remotePath){
			try
			{
			    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
			    m_sftp.mkdir (src);
				return true;
			}
			catch {
			
				return false;
			}
		}

		//删除SFTP文件 
		public bool Delete(string remoteFile)
		{
			try
			{
				m_sftp.rm(remoteFile);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Exist(string remotePath,string fileName){
			try
			{
				Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);

				foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
				{
					string sss = qqq.getFilename();
					if(sss==fileName){
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
			return false;
		
		}
		//获取SFTP文件列表         
		public ArrayList GetFileList(string remotePath, string fileType)
		{
			try
			{
				Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);
				ArrayList objList = new ArrayList();
				foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
				{
					string sss = qqq.getFilename();
					UnityEngine.Debug.Log(sss+"files*********");
					if (sss.Length > (fileType.Length + 1) && fileType == sss.Substring(sss.Length - fileType.Length))
					{ objList.Add(sss); }
					else { continue; }
				}

				return objList;
			}
			catch
			{
				return null;
			}
		}

	}

	//登录验证信息         
	public class MyUserInfo : UserInfo
	{
		String passwd;

		public String getPassword() { return passwd; }
		public void setPassword(String passwd) { this.passwd = passwd; }

		public String getPassphrase() { return null; }
		public bool promptPassphrase(String message) { return true; }

		public bool promptPassword(String message) { return true; }
		public bool promptYesNo(String message) { return true; }
		public void showMessage(String message) { }

	}
}