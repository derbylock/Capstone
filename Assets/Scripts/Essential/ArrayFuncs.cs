// List of necessary Array functions
using System.Collections.Generic;

public class ArrayFuncs {
  /*public static void PushObjectsToFront<T>(ref IList<T> arr) {
    try {
      int firstNullIndex = arr.IndexOf(null); //FindFirstNull(arr);
      if(firstNullIndex == -1 || firstNullIndex == arr.Length-1) {
        return;
      }
      for (int i=firstNullIndex+1; i<arr.Length; ++i) {
        if(arr[i] != null) {
          arr[firstNullIndex] = arr[i];
          arr[i] = null;
          firstNullIndex = i;
        }
      }
    } catch (System.Exception e) {
      Debug.Log("ArrayFuncs.PushObjectsToFront encountered the following error: " + e.Message);
    }
  }
  /*private static int FindFirstNull<T>(IList<T> arr) {
    for(int i=0; i<arr.Length; ++i) {
      if(arr[i] == null) {
        return i;
      }
    }
    return -1;
  }*/
}