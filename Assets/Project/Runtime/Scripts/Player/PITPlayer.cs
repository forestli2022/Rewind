using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PITPlayer
{
    public RigidbodyState rigidbodyState;

    // health
    private float health;
    private float damageTimer;
    // pistol
    private bool pistolActive;
    private int pistolBulletNum;
    private bool reloading;
    private float currentReloadTime;
    // finisher
    private bool katanaActive;
    private int finisherType;
    private float finisherTime;
    private GameObject finisherTargetEnemy;
    private string animationName;
    private float animationTime;
    //cube
    private bool hasCube;
    private GameObject currentCube;


    public PITPlayer(RigidbodyState rbState, float health, float damageTimer, bool pistolActive, int pistolBulletNum, bool reloading, float currentReloadTime, bool katanaActive, int finisherType, float finisherTime, GameObject finisherTargetEnemy, string animationName, float animationTime, bool hasCube, GameObject currentCube)
    {
        this.rigidbodyState = rbState;

        this.health = health;
        this.pistolActive = pistolActive;
        this.pistolBulletNum = pistolBulletNum;
        this.reloading = reloading;
        this.currentReloadTime = currentReloadTime;
        this.katanaActive = katanaActive;
        this.finisherType = finisherType;
        this.finisherTime = finisherTime;
        this.finisherTargetEnemy = finisherTargetEnemy;
        this.animationName = animationName;
        this.animationTime = animationTime;
        this.damageTimer = damageTimer;
        this.hasCube = hasCube;
        this.currentCube = currentCube;
    }

    public void SetState(out RigidbodyState rbs, out float health, out float damageTimer, out bool pistolActive, out int pistolBulletNum, out bool reloading, out float currentReloadTime, out bool katanaActive, out int finisherType, out float finisherTime, out GameObject finisherTargetEnemy, out string animationName, out float animationTime, out bool hasCube, out GameObject currentCube)
    {
        rbs = this.rigidbodyState;
        health = this.health;
        damageTimer = this.damageTimer;
        pistolActive = this.pistolActive;
        pistolBulletNum = this.pistolBulletNum;
        reloading = this.reloading;
        currentReloadTime = this.currentReloadTime;
        katanaActive = this.katanaActive;
        finisherType = this.finisherType;
        finisherTime = this.finisherTime;
        finisherTargetEnemy = this.finisherTargetEnemy;
        animationName = this.animationName;
        animationTime = this.animationTime;
        damageTimer = this.damageTimer;
        hasCube = this.hasCube;
        currentCube = this.currentCube;
    }
}
