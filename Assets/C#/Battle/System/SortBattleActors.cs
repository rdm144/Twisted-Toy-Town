using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortBattleActors : MonoBehaviour
{
    // Partition function
    private static int Partition(List<Battle_Actor> arr, int lowIndex, int highIndex) {
        
        // Choose the pivot
        Battle_Actor pivot = arr[highIndex];
        
        // Index of smaller element and indicates 
        // the right position of pivot found so far
        int i = lowIndex - 1;

        // Traverse arr[low..high] and move all smaller
        // elements to the left side. Elements from low to 
        // i are smaller after every iteration
        for (int j = lowIndex; j <= highIndex - 1; j++) {
            if (arr[j].partyIndex < pivot.partyIndex) {
                i++;
                Swap(arr, i, j);
            }
        }
        
        // Move pivot after smaller elements and
        // return its position
        Swap(arr, i + 1, highIndex);  
        return i + 1;
    }

    // Swap function
    private static void Swap(List<Battle_Actor> arr, int i, int j) {
        Battle_Actor temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    // The QuickSort function implementation
    private static void QuickSort(List<Battle_Actor> arr, int lowIndex, int highIndex) {
        if (lowIndex < highIndex) {
            
            // pi is the partition return index of pivot
            int partitionIndex = Partition(arr, lowIndex, highIndex);

            // Recursion calls for smaller elements
            // and greater or equals elements
            QuickSort(arr, lowIndex, partitionIndex - 1);
            QuickSort(arr, partitionIndex + 1, lowIndex);
        }
    }

    public static List<Battle_Actor> QuickSort(List<Battle_Actor> arr)
    {
        int lowIndex = 0;
        int highIndex = arr.Count - 1;

        if (lowIndex < highIndex)
        {

            // pi is the partition return index of pivot
            int partitionIndex = Partition(arr, lowIndex, highIndex);

            // Recursion calls for smaller elements
            // and greater or equals elements
            QuickSort(arr, lowIndex, partitionIndex - 1);
            QuickSort(arr, partitionIndex + 1, lowIndex);
        }

        return arr;
    }
}
