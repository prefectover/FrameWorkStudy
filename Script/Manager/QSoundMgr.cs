using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QFramework.AB;
using QFramework.PRIVATE;

namespace QFramework {

	public class QSoundMgr : QMgrBehaviour {

		protected override void SetupMgrId ()
		{
			mMgrId = (ushort)QMgrID.Sound;
		}

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

		protected QSoundMgr():base(){}

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

			audio = GetComponent<AudioSource>();
			if (null == audio) {
				audio = gameObject.AddComponent<AudioSource> ();
			}
		}


		void Start() {
			msgIds = new ushort[] {
				(ushort)QSoundEvent.SoundSwitch
			};

			RegisterSelf(this,msgIds);
		}

		/// <summary>
		/// 覆盖掉上一级的消息转发
		/// </summary>
		public override void ProcessMsg (QMsg msg)
		{
			switch (msg.msgId) {
			case (ushort)QSoundEvent.SoundSwitch:
				Debug.Log ("SoundOn:" + ((QSoundMsg)msg).soundOn);
				break;
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



		private AudioSource audio;
		private Dictionary<string,AudioClip> sounds = new Dictionary<string,AudioClip>();


		/// <summary>
		/// 添加一个声音
		/// </summary>
		void Add(string key, AudioClip value) {
			if (sounds.ContainsKey(key) || value == null) return;
			sounds.Add(key, value);
		}

		/// <summary>
		/// 获取一个声音
		/// </summary>
		AudioClip Get(string key) {
			if (!sounds.ContainsKey (key)) return null;
			return sounds [key];
		}

		/// <summary>
		/// 载入一个音频
		/// </summary>
		public AudioClip LoadAudioClip(string path) {
			AudioClip ac = Get(path);
			if (ac == null) {
				ac = (AudioClip)Resources.Load(path, typeof(AudioClip));
				Add(path, ac);
			}
			return ac;
		}


		public void UnloadAudioClip(string path) {
			if (sounds.ContainsKey (path)) {
				var audioClip = sounds [path];
				sounds.Remove (path);
				Resources.UnloadAsset (audioClip);
			}
		}

		/// <summary>
		/// 是否播放背景音乐，默认是1：播放
		/// </summary>
		/// <returns></returns>
		public bool CanPlayBackSound() {
			string key = QAppConst.AppPrefix + "BackSound";
			int i = PlayerPrefs.GetInt(key, 1);
			return i == 1;
		}

		/// <summary>
		/// 播放背景音乐
		/// </summary>
		/// <param name="canPlay"></param>
		public void PlayBacksound(string name, bool canPlay) {
			if (audio.clip != null) {
				if (name.IndexOf(audio.clip.name) > -1) {
					if (!canPlay) {
						audio.Stop();
						audio.clip = null;
						QUtil.ClearMemory();
					}
					return;
				}
			}
			if (canPlay) {
				audio.loop = true;
				audio.clip = LoadAudioClip(name);
				audio.Play();
			} else {
				audio.Stop();
				audio.clip = null;
				QUtil.ClearMemory();
			}
		}

		/// <summary>
		/// 是否播放音效,默认是1：播放
		/// </summary>
		/// <returns></returns>
		public bool CanPlaySoundEffect() {
			string key = QAppConst.AppPrefix + "SoundEffect";
			int i = PlayerPrefs.GetInt(key, 1);
			return i == 1;
		}

		/// <summary>
		/// 播放音频剪辑
		/// </summary>
		/// <param name="clip"></param>
		/// <param name="position"></param>
		public void Play(AudioClip clip, Vector3 position) {
			if (!CanPlaySoundEffect()) return;
			AudioSource.PlayClipAtPoint(clip, position); 
		}
	}

}