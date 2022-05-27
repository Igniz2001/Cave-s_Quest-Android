
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;
    public Vector3 minValues, maxValue;

    // Update is called once per frame
    private void FixedUpdate()
    {
        Follow();
    }

    void Follow()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            //Verifica si el targetPosition esta fuera de limite o no
            //Está limitado a los valores max y min observados en el componente del script

            Vector3 boundPosition = new Vector3(Mathf.Clamp(targetPosition.x, minValues.x, maxValue.x),
                Mathf.Clamp(targetPosition.y, minValues.y, maxValue.y),
                Mathf.Clamp(targetPosition.z, minValues.z, maxValue.z));

            Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.fixedDeltaTime);
            transform.position = smoothPosition;
        }
    }
}
