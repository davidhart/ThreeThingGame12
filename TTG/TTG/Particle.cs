using System;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class ParticleEmitter
	{
		public struct Vec3MinMax
		{
			public Vector3 min;
			public Vector3 max;
		};
		
		public struct FloatMinMax
		{
			public float min;
			public float max;
		};
		
		private struct Particle
		{
			public float life;
			public float orgLife;
			public Vector3 pos;
			public Vector3 vel;
			public Vector3 orgVel;
		};
		
		private Particle[] particles;
		
		public ParticleEmitter ()
		{}
		
		public void Initialize(int maxParticles, Vec3MinMax vel, FloatMinMax life)
		{
			Random rand = new Random();
			
			particles = new Particle[maxParticles];
			
			for(int i = 0; i < maxParticles; ++i)
			{
				particles[i].pos = Vector3(0,0,0);
				particles[i].orgVel = GenerateRandomVec3(vel, rand);
				particles[i].vel = particles[i].orgVel;
				particles[i].orgLife = particles[i].life = GenerateRandomFloat (life.min, life.max, rand);
			}
		}
		
		private float GenerateRandomFloat(float min, float max, Random rand)
		{
			return (float)(min + (rand.NextDouble() * (max - min)));
		}
		
		private Vector3 GenerateRandomVec3(Vec3MinMax minmax, Random rand)
		{
			float x = GenerateRandomFloat(minmax.min.X, minmax.max.X, rand);
			float y = GenerateRandomFloat(minmax.min.Y, minmax.max.Y, rand);
			float z = GenerateRandomFloat(minmax.min.Z, minmax.max.Z, rand);
			
			return new Vector3(x,y,z);
		}
		
		public void Update(float dt)
		{
			for(int i = 0; i < particles.Length; ++i)
			{
				if(particles[i].life > 0.0f)
				{
					particles[i].pos += particles[i].vel * dt;
				}
			}
		}
		
		public void Draw()
		{
			
		}
	}
}

