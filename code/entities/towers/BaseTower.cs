using System;
using Sandbox;


namespace TerryDefense.Towers {
	public class BaseTower : AnimEntity {
		public virtual float Range { get; set; }
		public virtual float Damage { get; set; }
		public virtual float FireRate { get; set; }

		public Entity Target { get; set; }
	}
}
