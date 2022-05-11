using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerManager
{
    //Locates the first object with the Player tag
    static Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

    //Locates the first object with the Player tag
    static Transform cameraTrans = playerTrans.GetChild(0);

    // Tracks Player's Health
    static int currentHealth = 100;
    static int maxHealth = 100;

    static bool isArmed = false;

    static string currentElement = "Fire";

    static Vector3 spawnPoint = new Vector3(-3, 0, -3);

    // Tracks Player's Ammunition
    public static Dictionary<string, int> currentAmmo = new Dictionary<string, int>(){
        {"Fire", 0},
        {"Water", 0},
        {"Ice", 0},
        {"Earth", 0}
    };

    public static Dictionary<string, int> maxAmmo = new Dictionary<string, int>(){
        {"Fire", 50},
        {"Water", 50},
        {"Ice", 50},
        {"Earth", 50}
    };

    // Reduces Player's Health
    public static void PlayerHurt(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) PlayerDied();
    }

    // Increase Player's Health
    public static void PlayerHeal(int amount)
    {
        currentHealth = (currentHealth += amount) > maxHealth ? maxHealth : currentHealth += amount;
    }

    // Reduces Player's Ammo
    public static void UseAmmo(int amount, string element)
    {
        int ammoTarget = currentAmmo[element] -= amount;
        currentAmmo[element] = ammoTarget < 0 ? 0 : ammoTarget;
    }

    public static void setElement(string element)
    {
        currentElement = element;
    }

    public static string Element()
    {
        return currentElement;
    }
    // Increase Player's Ammo
    public static void AddAmmo(int amount, string element)
    {
        int ammoTarget = currentAmmo[element] += amount;
        currentAmmo[element] = ammoTarget > maxAmmo[element] ? maxAmmo[element] : ammoTarget;
    }
    // Increase Player's Ammo
    public static int GetCurrentAmmo(string element)
    {
        return currentAmmo[element];
    }

    // Returns Player's Position as a Vector3
    public static Vector3 playerPosition()
    {
        return playerTrans.position;
    }

    // Returns Player's Position as a Vector3
    public static Transform PlayerTranform()
    {
        return playerTrans;
    }

    // Returns Player's Position as a Vector3
    public static Vector3 cameraPosition()
    {
        return cameraTrans.position;
    }

    // Gets distance to player
    public static float playerDistance(Transform target)
    {
        return Vector3.Distance(target.position, playerTrans.position);
    }

    // Gets distance to player
    public static float cameraDistance(Transform target)
    {
        return Vector3.Distance(target.position, cameraTrans.position);
    }

    // Gets distance to player
    public static float PlayerHealth()
    {
        return currentHealth;
    }

    public static void Arm()
    {
        isArmed = true;
    }

    public static void DisArm()
    {
        isArmed = false;
    }

    public static bool IsArmed()
    {
        return isArmed;
    }

    static void PlayerDied()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}