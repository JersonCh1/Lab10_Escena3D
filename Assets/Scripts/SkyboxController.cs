using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [Header("Materiales de Skybox")]
    public Material skyboxDia;
    public Material skyboxNoche;

    [Header("Estado inicial")]
    public bool esDeNoche = false;

    void Update()
    {
        // Presionar la tecla N para alternar entre Skybox de dia y de noche
        if (Input.GetKeyDown(KeyCode.N))
        {
            AlternarSkybox();
        }
    }

    void AlternarSkybox()
    {
        esDeNoche = !esDeNoche;

        if (esDeNoche)
        {
            RenderSettings.skybox = skyboxNoche;
        }
        else
        {
            RenderSettings.skybox = skyboxDia;
        }

        // Recalcular la iluminacion ambiental con el nuevo Skybox
        DynamicGI.UpdateEnvironment();

        Debug.Log("Skybox cambiado a: " + (esDeNoche ? "Noche" : "Dia"));
    }
}
