using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.Pss.Core.Audio;

namespace TTG
{
	public static class SoundSystem
	{
		private static Dictionary<string, BgmResource> bgmList = new Dictionary<string, BgmResource>();
		private static Dictionary<string, SoundResource> soundList = new Dictionary<string, SoundResource>();
		
		//Bgm[] sound;
		//BgmPlayer soundPlayer;
		
		//Sound[] effect;
		//SoundPlayer[] effectPlayer;
		
		//Bgm s1 = new Bgm("assets/sounds/One-eyed Maestro.mp3");
		//Bgm s2 = new Bgm("assets/sounds/Sneaky Snitch.mp3");
		
		//public SoundSystem ()
		//{			
		//	sound = new Bgm[]
		//	{
		//		new Bgm("assets/sounds/Sneaky Snitch.mp3"),
		//		new Bgm("assets/sounds/One-eyed Maestro.mp3"),
		//	};
		//	
		//	effect = new Sound[]
		//	{
		//		new Sound("assets/sounds/tankfiring.wav"),
		//		new Sound("assets/sounds/turretgun.wav")
		//	};
		//	
		//	//soundPlayer = new BgmPlayer[]
		//	//{
		//	//	//sound[0].CreatePlayer(),
		//	//	//sound[1].CreatePlayer(),
		//	//};
		//}
		
		public static void AddBgm(string key, string filePath)
		{
			if(!bgmList.ContainsKey(key))
			{
				bgmList.Add(key, new BgmResource(filePath));
			}
		}
		
		public static void AddSound(string key, string filePath)
		{
			if(!soundList.ContainsKey(key))
			{
				soundList.Add(key, new SoundResource(filePath));
			}
		}
		
		public static void PlayBgm(string key, float volume = 1.0f, bool loop = false)
		{
			if(bgmList.ContainsKey(key))
			{
				BgmResource bgm = bgmList[key];
				if(bgm != null)
				{
					bgm.Play(volume, loop);
				}
			}
		}
		
		public static void PlaySound(string key, float volume = 1.0f, bool loop = false)
		{
			if(soundList.ContainsKey(key))
			{
				SoundResource sound = soundList[key];
				if(sound != null)
				{
					sound.Play(volume, loop);
				}
			}
		}
		
		public static void StopBgm(string key)
		{
			if(bgmList.ContainsKey(key))
			{
				BgmResource bgm = bgmList[key];
				if (bgm != null) {
            		
            	    	bgm.Stop();
            	
        		}
			}
		}
		
		public static void StopSound(string key)
		{
			if(soundList.ContainsKey(key))
			{
				SoundResource sound = soundList[key];
				if (sound != null) {
            		
            	    	sound.Stop();
            		
        		}
			}
		}
	}
	
	public class SoundResource
	{
		private Sound sound = null;
		private SoundPlayer soundPlayer = null;
		
		public SoundResource(string filePath)
		{
			sound = new Sound(filePath);
		}
		
		public void Play(float volume = 1.0f, bool loop = false)
		{	
			Stop();
			soundPlayer = sound.CreatePlayer();
			soundPlayer.Play();
			soundPlayer.Volume = volume;
			soundPlayer.Loop = loop;	
		}
		
		public void Stop()
		{
			if(soundPlayer != null)
			{
				if(soundPlayer.Status == SoundStatus.Playing)
				{
					soundPlayer.Stop();
				}
				//soundPlayer.Dispose();
				//soundPlayer = null;
			}
		}
		
		public SoundStatus GetStatus() { return soundPlayer.Status; }
	}
	
	public class BgmResource
	{
		private Bgm bgm = null;
		private BgmPlayer bgmPlayer = null;
		
		public BgmResource(string filePath)
		{
			bgm = new Bgm(filePath);
		}
		
		public void Play(float volume = 1.0f, bool loop = false)
		{	
			bgmPlayer = bgm.CreatePlayer();
			bgmPlayer.Play();
			bgmPlayer.Volume = volume;
			bgmPlayer.Loop = loop;
		}
		
		public void Stop()
		{
			if(bgmPlayer != null)
			{
				if(bgmPlayer.Status == BgmStatus.Playing)
				{
					bgmPlayer.Stop();
				}
				bgmPlayer.Dispose();
				bgmPlayer = null;
			}
		}
		
		public BgmStatus GetStatus() { return bgmPlayer.Status; }
	}
}

