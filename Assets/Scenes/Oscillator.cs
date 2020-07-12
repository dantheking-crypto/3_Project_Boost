using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [Range(0, 1)] [SerializeField] float movementFactor;
    Vector3 startingPos;
    [SerializeField] float period = 2f;
    const float tau = Mathf.PI * 2;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) return;
        float cycles = Time.time / period;
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = (rawSinWave + 1) / 2f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
