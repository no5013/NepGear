using System.Collections;
using System.Collections.Generic;


public class BasicFrame : Frame {

	private string ultimateName;
    private float ultimateDuration;

	public BasicFrame() : base(100f, 100f, 200f, 5){
		this.ultimateName = "You're power up get in there!!!";
        this.ultimateDuration = 10f;
	}
		
	public override void Ultimate() {
		this.SetSpeed (this.GetSpeed () * 2f);
		this.SetStamina(this.GetStamina() * 2f);
		this.SetAttackMultiplier(this.GetAttackMultiplier() * 2f);
	}
    public override void UltimateEnd()
    {
        this.SetAttackMultiplier(this.GetAttackMultiplier() / 2f);
        this.SetSpeed(this.GetSpeed() / 2f);
        this.SetStamina(this.GetStamina() / 2f);
    }

    public override float GetUltimateDuration()
    {
        return ultimateDuration;
    }

}