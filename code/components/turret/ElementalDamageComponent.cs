using System.Threading.Tasks;
using Sandbox;
using TerryDefense.Units;

namespace TerryDefense.components.turret {
	public class ElementalDamageComponent : BaseTurretComponent {
		public enum ElementType {
			Physical,
			Fire,
			Ice,
			Lightning,
			Poison,
		}
		public ElementType Element { get; set; } = ElementType.Physical;
		public float Damage { get; set; } = 1f;
		public float OverTimeDuration { get; set; } = 0f;
		public float OverTimeDamageTickRate { get; set; } = 0.5f;


		public virtual void DealDamage(BaseUnit Unit) {
			Debug.WorldText(Unit.Position, $"{Damage}", Color.Red, 1f);
			if(OverTimeDuration > 0) {
				OverTimeDamage(Unit);
			} else {
				Unit.TakeDamage(Damage);
			}
		}

		public async void OverTimeDamage(BaseUnit Unit) {
			TimeSince StartOfOverTime = 0;
			while(Unit.IsValid() && StartOfOverTime < OverTimeDuration && Unit.LifeState == LifeState.Alive) {
				Unit.TakeDamage(Damage);
				await GameTask.DelaySeconds(OverTimeDamageTickRate);
			}
		}

	}

}
