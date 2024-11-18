using UnityEngine;
using TMPro;
using System.Collections;

public class GroupBalls : MonoBehaviour
{
    public float velocidadElectricBall = 0.0f;

    public int ACUM = 5;
    public int ACUMCirculo = 25;
    public float shootcadency = 0.3f;
    public float cycles = 22.0f;
    public float radius = 4.0f;  
    public float rotacionIncremento = 15.0f;
    private float currentRotation = 0.0f; 




    public GameObject ElectricBall;
    public TextMeshProUGUI acumBallsText;



    private int ballsCount = 0;

    void Start()
    {
        StartCoroutine(SpawnBalls());
    }

    public void ballsKilled()
    {
        ballsCount--;
        UpdateText();
    }

    void UpdateText()
    {
        acumBallsText.text = $"ACUM de balls: {ballsCount}";
    }

    IEnumerator SpawnBalls()
    {

        yield return SpawnFase(Fase1, cycles / 1.5f, shootcadency * 3);
        yield return new WaitForSeconds(8.0f);

        yield return SpawnFase(Fase2, cycles, shootcadency);
        yield return new WaitForSeconds(8.0f);

        yield return SpawnFase(Fase3, cycles, shootcadency);
    }

    // Función auxiliar para manejar la lógica de cada fase
    private IEnumerator SpawnFase(System.Action fase, float cycle, float delay)
    {
        for (int i = 0; i < cycle; i++)
        {
            fase();
            yield return new WaitForSeconds(delay);
        }
    }




    void Fase1() {
            float anguloSeparacion = 360f / ACUMCirculo;
            Vector3 spawnCenter = transform.position;

            for (int i = 0; i < ACUMCirculo; i++) {
                float angulo = i * anguloSeparacion;
                Vector3 direccion = GetDirectionFromAngle(angulo);

                Vector3 posicionSpawn = spawnCenter + direccion * radius;
                SpawnBall(posicionSpawn, direccion);
            }
        }

        Vector3 GetDirectionFromAngle(float angulo) {
            float rad = angulo * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)).normalized;
        }

        void SpawnBall(Vector3 posicion, Vector3 direccion) {
            GameObject ball = Instantiate(ElectricBall, posicion, Quaternion.identity);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = direccion * velocidadElectricBall;

            plusBallACUM(ball);
        }

    void Fase2()
    {
        // Precalcular el valor de la separación angular.
        float anguloSeparacion = 360f / ACUM;
        
        // Obtener la dirección normalizada para la rotación.
        Vector3 spawnPosition = transform.position;

        // Reutilizar el mismo vector de dirección en cada iteración.
        Vector3 direction = Vector3.zero;

        for (int i = 0; i < ACUM; i++) {
            // Calcular el ángulo y convertir a radianes.
            float angulo = (i * anguloSeparacion + currentRotation) * Mathf.Deg2Rad;

            // Calcular la dirección en el plano XY.
            direction.x = Mathf.Cos(angulo);
            direction.z = Mathf.Sin(angulo);
            
            // Normalizar la dirección y calcular la posición de spawn.
            direction.Normalize();
            Vector3 posicionSpawn = spawnPosition + direction * radius;

            // Instanciar el objeto y asignar velocidad.
            GameObject ball = Instantiate(ElectricBall, posicionSpawn, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = direction * velocidadElectricBall;

            // Llamar a la función para incrementar el contador.
            plusBallACUM(ball);
        }

        // Actualizar la rotación actual.
        currentRotation = (currentRotation + rotacionIncremento) % 360f;
    }


    void Fase3() 
    {
        // Número de posiciones por chorro y su dispersión radial.
        float dispersionFactor = 1f / ACUM;
        
        // Generamos un valor de dispersión aleatorio para cada instanciación.
        for (int i = 0; i < ACUM; i++) 
        {
            // Generamos un ángulo aleatorio dentro de un rango de 360 grados.
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // Calculamos una dirección de spawn en un círculo radial.
            Vector3 direccion = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));

            // Aplicar dispersión basada en el índice de la iteración para crear el efecto de "flor".
            float radioAjustado = radius + dispersionFactor * i;

            // Calcular la posición de spawn en función de la dirección y el radio ajustado.
            Vector3 posicionSpawn = transform.position + direccion * radioAjustado;

            // Instanciar el objeto y asignar la velocidad.
            GameObject ball = Instantiate(ElectricBall, posicionSpawn, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = direccion * velocidadElectricBall;

            // Llamar a la función para incrementar el contador.
            plusBallACUM(ball);
        }

        // Actualizar la rotación actual.
        currentRotation = (currentRotation + rotacionIncremento) % 360f;
    }

    void plusBallACUM(GameObject ball)
    {
        if (ball == null) return; // Evita errores si el objeto ya fue destruido

        ballsCount++;
        UpdateText();

        ElectricBalls tfScript = ball.GetComponent<ElectricBalls>();
        if (tfScript != null)
        {
            tfScript.SetGroupBalls(this);
        }
    }

    
}
