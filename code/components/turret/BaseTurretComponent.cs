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

	public class TestingTurretComponent : BaseTurretComponent {
		public float test { get; set; } = 10f;
		public float TestingSomething { get; set; }
		public float TestingSomethingElse { get; set; }
		public float TestingSomethingElseAgain { get; set; }
		public float TestingSomethingElseAgainAgain { get; set; } = 10f;
		public float TestingSomethingElseAgainAgainAgain { get; set; } = 10f;

	}

	public class SomethingComponent : BaseTurretComponent {
		public float NewStuff { get; set; } = 10f;
		public string NewStuff2 { get; set; } = "Some Text That could be yours !";
		public Vector3 NewStuff3 { get; set; } = new Vector3(1, 2, 3);

	}
	public class SomethingElseComponent : BaseTurretComponent {
		public float NewStuff { get; set; } = 10f;
	}

	public class SomethingElseAgainComponent : BaseTurretComponent {
		public float NewStuff { get; set; } = 10f;
	}

	public class SomethingElseAgainAgainComponent : BaseTurretComponent {
		public float NewStuff { get; set; } = 10f;
	}

	public class SomethingElseAgainAgainAgainComponent : BaseTurretComponent {
		public float NewStuff { get; set; } = 10f;
	}

}
