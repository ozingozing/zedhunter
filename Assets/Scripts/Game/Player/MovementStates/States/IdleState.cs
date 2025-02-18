using UnityEngine;

public class IdleState : MovementBaseState
{
	public override void EnterState(MovementStateManager movement)
	{
	}

	public override void UpdateState(MovementStateManager movement)
	{
		if(movement.dir.magnitude > 0.1f) {
			movement.SwitchState(Input.GetKey(KeyCode.LeftShift) ? movement.Run : movement.Walk);
		}
	}
}
