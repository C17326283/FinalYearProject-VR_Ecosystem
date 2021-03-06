using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script for teh data of all the individual animals, make a profile from this for each animal
[CreateAssetMenu(fileName = "Animal Profile",menuName = "Animal Profile")]//Create from asset menu
public class AnimalProfile : ScriptableObject
{
    public String name;
    public GameObject model;
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float reproductiveUrge = 0;
    public float age = 0;
    public float deathAge = 0;
    
    public float maxHealth = 100f;
    public float maxStat = 100;
    public float healthStarveDecrement = 3f;
    public float hungerDecrement = 3f;
    public float thirstDecrement = 1f;
    public float reproductiveIncrement = 6f;
    public float ageIncrement = 1f;
    
    public float memoryLossRate = 20;
    public float sensoryRange = 15;
    public float moveSpeed = 5;
    public float rotSpeed = 50;
    public float wanderRadius = 5;
    public float forwardWanderBias = 2;
    public String foodTag = "Food";
    public float maxMutatePercent = 2;
    public int numOfLegs = 4;
    public float attackRate = 1;
    public float attackDamage = 5;
    
    public float litterSize = 1;
    public float foodWorth = 2;
    
    public float predatorRating = 1;
    public float preyRating = 1;
    public bool eatsPlants = true;
    public bool eatsMeat = true;
    
    public int gen = 1;

    public float bounceMult = 1;

    public GameObject attackCanvas;
    public GameObject heartCanvas;
    public GameObject deathCanvas;
    public GameObject foodCanvas;
    public GameObject drinkCanvas;

    public AudioClip footstep;
    public AudioClip attack;
    public AudioClip ambient;
    
    public float hungerThresh = 50;
    public float thirstThresh = 50;
    public float mateThresh = 90;
    
    
    
}
