using System;

using Sce.Pss.Core;

namespace TTG
{
	public class Camera
	{
		Vector3 position;
		Vector3 lookAt;
		Vector3 up;
		
		public Camera(Vector3 position, Vector3 lookAt, Vector3 up)
		{
			this.position = position;
			this.lookAt = lookAt;
			this.up = up;
		}
		
		public void SetLookAt(Vector3 lookAt)
		{
			this.lookAt = lookAt;
		}
		
		public void SetUp(Vector3 up)
		{
			this.up = up;	
		}
		
		public void SetPosition(Vector3 position)
		{
			this.position = position;	
		}
		
		public Matrix4 GetMatrix()
		{
			return Matrix4.LookAt(position, lookAt, up);
		}
		
		public Vector3 upVector()
		{
			Vector3 fwd = (lookAt - position).Normalize();
			Vector3 right = fwd.Cross(up);
			return fwd.Cross(right);
		}
		
		public Vector3 right()
		{
			Vector3 fwd = (lookAt - position).Normalize();
			return fwd.Cross(up);
		}
	}
}

