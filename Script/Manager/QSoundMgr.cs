using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QFramework.AB;

namespace QFramework {

	public class QSoundMgr : MonoBehaviour {

		public static QSoundMgr Instance {
			get {
				return QMonoSingletonComponent<QSoundMgr>.Instance;
			}
		}

		public IEnumerator Init() {
			yield return null;
		}

		private AudioSource musicPlayer;										// 音乐播放器
		private AudioListener listener;											// 音监听器
		private string mCurClipName;											// 当前的音效名字

		private QSoundMgr(){}

		public AudioClip[] clips = new AudioClip[SOUND.COUNT];					// 多少种Clips

		public Dictionary<string,AudioClip> musicClips = new Dictionary<string,AudioClip> ();				// 背景音乐分离出来
		public int soundState = SOUND.ON;
		public List<AudioSource>[] playersForClipId = new List<AudioSource>[SOUND.COUNT];	// 音效播放器

		/// <summary>
		/// 创建音效播放器和音乐播放器
		/// </summary>
		void Awake()
		{
			//防止被销毁
			DontDestroyOnLoad (gameObject);
			listener = gameObject.AddComponent<AudioListener> ();
			musicPlayer = gameObject.AddComponent<AudioSource> ();
			for (int i = 0; i < playersForClipId.Length; i++) {
				playersForClipId [i] = new List<AudioSource> (1);
				playersForClipId [i].Add(gameObject.AddComponent<AudioSource> ());
			}
		}

		/// <summary>
		/// 异步加载太慢了
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="id">Identifier.</param>
		public void PreloadClip(string bundleName,string soundName,int id)
		{
			QTest.TimeBegan (soundName);

			QResMgr.Instance.LoadResAsync (bundleName, soundName,delegate(bool success,Object resObj) {
				if (resObj)
				{

					Debug.LogWarning ("loaded: " + soundName + " " + id.ToString() + "time:" + QTest.TimeStop(soundName));

					clips[id] = resObj as AudioClip;
					playersForClipId[id][0].clip = clips[id];
				}
			});

		}

		public void PlayClipAsync(int id,bool loop = false)
		{
			if (soundState == SOUND.OFF) {
				return;
			}

			var players = playersForClipId [id];

			int count = players.Count;

			for (int i = 0; i < count; i++) {
				if (players [i].isPlaying) {

				} else {

					players [i].clip = clips [id];
					players [i].loop = loop;
					players [i].Play ();

					return;
				}
			}

			// 控制10个
			if (count == 10) {
				PlayClipSync (id);
				return;
			}

			var newSource = gameObject.AddComponent<AudioSource> ();
			players.Add (newSource);

			newSource.clip = clips [id];
			newSource.Play ();

		}

		// 异步播放音乐
		public void PlayClipSync(int id)
		{
			playersForClipId [id] [0].Play();
		}

		public void StopClip(int id)
		{
			var players = playersForClipId [id];
			for (int i = 0; i < players.Count; i++) {
				players [i].Stop ();
			}
		}

		public void PlayMusic(string name,bool loop = true)
		{
			Debug.LogWarning (name + "" + loop);

			musicPlayer.loop = loop;
			musicPlayer.clip = musicClips [name];
			if (soundState == SOUND.ON) {
				musicPlayer.volume = 1.0f;
			} else {
				musicPlayer.volume = 0.0f;
			}
			musicPlayer.Play ();

		}

		public void PreloadMusic(string bundleName,string musicName)
		{

			QResMgr.Instance.LoadResAsync (bundleName,musicName, delegate(bool succes,Object resObj) {
				if (resObj)
				{
					Debug.LogWarning ("loaded: " + musicName + " " + musicName.ToString());

					musicClips[musicName] = resObj as AudioClip;
				}
			}); 
		}


		public void LoadSoundSync(string path,int id)
		{
			var obj = Resources.Load (path);

			clips[id] = obj as AudioClip;
			playersForClipId[id][0].clip = clips[id];
		}

		public void StopMusic()
		{
			musicPlayer.Stop ();
		}

		/// <summary>
		/// 停止所有音效
		/// </summary>
		public void StopSound(int id)
		{
			int count = playersForClipId [id].Count;
			for (int i = 0; i < count; i++) {
				playersForClipId [id] [i].Stop ();
			}
		}
			
		public void On()
		{
			Debug.LogWarning ("Sound On");

			listener.enabled = true;
//			DataManager.Instance ().soundState = SOUND.ON;
			soundState = SOUND.ON;
			musicPlayer.volume = 1.0f;

			var audios = GetComponents<AudioSource> ();
			for (int i = 0; i < audios.Length; i++) {
				audios [i].volume = 1.0f;
			}
		}

		public void Off()
		{
			Debug.LogWarning ("Sound Off");

			listener.enabled = false;
//			DataManager.Instance ().soundState = SOUND.OFF;
			soundState = SOUND.OFF;

			var audios = GetComponents<AudioSource> ();
			for (int i = 0; i < audios.Length; i++) {
				audios [i].volume = 0.0f;
			}
		}


		void OnDestroy()
		{
			QMonoSingletonComponent<QSoundMgr>.Dispose ();
		}
	}

}