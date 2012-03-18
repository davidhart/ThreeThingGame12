using System;
using System.Collections.Generic;

using Sce.Pss.Core.Audio;

namespace TTG
{
	public class SoundSystem
	{
		Sound[] sound;
		SoundPlayer[] soundPlayer;
		
		public SoundSystem ()
		{
			sound = new Sound[]
			{
				new Sound("assets/sounds/titlemusic.wav")
			};
			
			soundPlayer = new SoundPlayer[]
			{
				sound[0].CreatePlayer()
			};
		}
		
		public void Play(int index, float volume, bool loop)
		{
			soundPlayer[index].Play();
			soundPlayer[index].Volume = volume;
			soundPlayer[index].Loop = loop;
		}
		
		public void Stop(int index)
		{
			soundPlayer[index].Stop();
		}
	}
}

