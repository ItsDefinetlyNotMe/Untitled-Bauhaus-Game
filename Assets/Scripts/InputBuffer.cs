using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer : MonoBehaviour
{
    [Header("Buffer")]
    [SerializeField] private int toBuffer = 1;
    public delegate void BufferFunctionDelegate(InputValue input);

    private Queue<KeyValuePair<BufferFunctionDelegate,InputValue>> bufferQueue;

    private void Awake()
    {
        bufferQueue = new Queue<KeyValuePair<BufferFunctionDelegate,InputValue>>();
    }

    public void BufferEnqueue(BufferFunctionDelegate function,InputValue input)
    {
        //Debug.Log("Enqueueing");
        bufferQueue.Enqueue(new KeyValuePair<BufferFunctionDelegate,InputValue>(function,input));   
    }

    public void BufferDequeue()
    {
        //Debug.Log("Dequeueing: " + bufferQueue.Count + " elements.");
        if (bufferQueue.Count > 0)
        {
            while (bufferQueue.Count > toBuffer)
            {
                bufferQueue.Dequeue();
            }
            var function = bufferQueue.Dequeue();
            //Debug.Log(nameof(function.Key));
            function.Key(function.Value);
        }
    }
}
