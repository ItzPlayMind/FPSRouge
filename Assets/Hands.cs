using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public enum Hand
    {
        Left, Right
    }

    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    public Object Instantiate(Object obj, Hand hand)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Left:
                spot = leftHandTransform;
                break;
            case Hand.Right:
                spot = rightHandTransform;
                break;
        }
        return Instantiate(obj, spot);
    }
}
