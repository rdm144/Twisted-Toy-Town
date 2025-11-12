using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Battle Actors should invoke createDamageNumber during attack/heal animations
public class DamageNumberSpawner : MonoBehaviour
{
    public GameObject damageNumberPrefab;

    public delegate void CreateDamageNumber(Vector3 spawnPoint, int value);
    public static CreateDamageNumber createDamageNumber;

    private void Start()
    {
        createDamageNumber += SpawnNewNumber;
    }

    private void SpawnNewNumber(Vector3 spawnPoint, int value)
    {
        Vector3 cameraDirection = (Camera.main.transform.position - spawnPoint).normalized;
        Vector3 finalPosition = spawnPoint + cameraDirection;

        GameObject damageNumber = Instantiate(damageNumberPrefab, finalPosition, Quaternion.identity);
        damageNumber.transform.LookAt(Camera.main.transform);
        if(value < 0)
        {
            damageNumber.GetComponentInChildren<TextMeshPro>().color = Color.green;
            damageNumber.GetComponentInChildren<TextMeshPro>().text = (-value).ToString();
        }
        else
        {
            damageNumber.GetComponentInChildren<TextMeshPro>().color = Color.white;
            damageNumber.GetComponentInChildren<TextMeshPro>().text = value.ToString();
        }
    }

    private void OnDestroy()
    {
        createDamageNumber -= SpawnNewNumber;
    }
}
