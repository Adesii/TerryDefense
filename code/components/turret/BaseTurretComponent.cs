using System;
using Sandbox;
using TerryDefense.Towers;

namespace TerryDefense.components.turret {

	public class BaseTurretComponent : EntityComponent<BaseTower> {
		public bool TestProperty { get; set; } = false;
	}

	public class AimTurretComponent : BaseTurretComponent {
		public float TurnRate { get; set; } = 10f;
	}
	public class DamageTurretComponent : BaseTurretComponent {
		public float Damage { get; set; } = 10f;
	}
	public class RecoilTurretComponent : BaseTurretComponent {
		public float Recoil { get; set; } = 10f;
	}
}
