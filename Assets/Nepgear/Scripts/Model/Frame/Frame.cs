using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Frame {

	private float hitPoint;
	private float stamina;
	private float speed;
    private float ultimateMaxCharge;
	private float attackMultiplier;

	public Frame (float hitPoint, float stamina, float ultimateMaxCharge, float speed) {
		this.hitPoint = hitPoint;
		this.stamina = stamina;
		this.speed = speed;
        this.ultimateMaxCharge = ultimateMaxCharge;
		attackMultiplier = 1f;
	}

	public void TakeDamage(float damage) {
		this.hitPoint -= damage;
	}

	public void SetHitpoint(float hitPoint) {
		this.hitPoint = hitPoint;
	}

	public void SetStamina(float stamina) {
		this.stamina = stamina;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
	}

	public void SetAttackMultiplier(float attackMultiplier) {
		this.attackMultiplier = attackMultiplier;
	}

	public float GetHitPoint() {
		return this.hitPoint;
	}

	public float GetStamina() {
		return this.stamina;
	}

	public float GetSpeed() {
		return this.speed;
	}

	public float GetAttackMultiplier() {
		return this.attackMultiplier;
	}

    public float GetUltimateMaxCharge()
    {
        return ultimateMaxCharge;
    }

	public abstract void Ultimate ();

    public abstract void UltimateEnd();

    public abstract float GetUltimateDuration();
}
