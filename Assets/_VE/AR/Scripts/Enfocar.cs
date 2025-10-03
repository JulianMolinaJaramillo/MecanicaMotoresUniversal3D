using UnityEngine;

public class Enfocar : MonoBehaviour
{
    public GameObject objetoActivar; 
    public GameObject objetoDesactivar; 
    public Transform posicionObjetivo;
    public PulsoEscala pulsoEscala;
    public bool btnCuenca;
    public bool btnCasas;
    public bool btnCuencaInterno;
    public bool btnCasasInterno;

    void OnMouseDown()
    {
        SimpleAudioManager.singleton.PlaySound(6);
        if (btnCuenca)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.DesastreCuenca();
            ManagerControlador.singleton.ActivarImagenCuenca();

            if (posicionObjetivo != null)
            {
                MirarCamara.singleton.DetenerRotacion();
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
            }
            if (ManagerControlador.singleton.desastreSecundarioActivo && !ManagerControlador.singleton.puntoEncuentroActivo)
            {
                SimpleAudioManager.singleton.Gritos();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                SimpleAudioManager.singleton.Hablando();
            }
        }
        else if (btnCasas)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.DesastreCasas();
            ManagerControlador.singleton.ActivarImagenCasas();

            if (posicionObjetivo != null)
            {
                MirarCamara.singleton.DetenerRotacion();
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
            }
            
            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.Gritos();
                SimpleAudioManager.singleton.Alarma();
            }
        }
        else if (btnCuencaInterno)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.ActivarImagenIguana();

            if (posicionObjetivo != null)
            {
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
                MirarCamara.singleton.ReanudarRotacion();
            }

            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.DesactivarGritos();
            }

            if (ManagerControlador.singleton.momentoReunionTerminado)
            {
                ManagerControlador.singleton.momentoReunionTerminado = false;
                ManagerControlador.singleton.botonReunion.SetActive(true);
            }

            if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
            {
                ManagerControlador.singleton.AntesDeNormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                ManagerControlador.singleton.NormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                SimpleAudioManager.singleton.DesactivarHablando();
            }

            pulsoEscala.RestablecerEscalaColor();
        }
        else if (btnCasasInterno)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.ActivarImagenIguana();

            if (posicionObjetivo != null)
            {
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
                MirarCamara.singleton.ReanudarRotacion();
            }

            if (ManagerControlador.singleton.momentoDosCasasTerminado)
            {
                ManagerControlador.singleton.DesactivarNPCCasas();
            }

            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.DesactivarGritos();
                SimpleAudioManager.singleton.DesactivarAlarma();
            }

            if (ManagerControlador.singleton.momentoReunionTerminado)
            {
                ManagerControlador.singleton.momentoReunionTerminado = false;
                ManagerControlador.singleton.botonReunion.SetActive(true);
            }

            if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
            {
                ManagerControlador.singleton.AntesDeNormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                ManagerControlador.singleton.NormalizarCuenca();
            }
            pulsoEscala.RestablecerEscalaColor();
        }
    }

    [ContextMenu("avticar")]
    public void Ejecutar()
    {
        SimpleAudioManager.singleton.PlaySound(6);
        if (btnCuenca)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.DesastreCuenca();
            ManagerControlador.singleton.ActivarImagenCuenca();

            if (posicionObjetivo != null)
            {
                MirarCamara.singleton.DetenerRotacion();
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
            }
            if (ManagerControlador.singleton.desastreSecundarioActivo && !ManagerControlador.singleton.puntoEncuentroActivo)
            {
                SimpleAudioManager.singleton.Gritos();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                SimpleAudioManager.singleton.Hablando();
            }
        }
        else if (btnCasas)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.DesastreCasas();
            ManagerControlador.singleton.ActivarImagenCasas();

            if (posicionObjetivo != null)
            {
                MirarCamara.singleton.DetenerRotacion();
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
            }

            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.Gritos();
                SimpleAudioManager.singleton.Alarma();
            }
        }
        else if (btnCuencaInterno)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.ActivarImagenIguana();

            if (posicionObjetivo != null)
            {
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
                MirarCamara.singleton.ReanudarRotacion();
            }

            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.DesactivarGritos();
            }

            if (ManagerControlador.singleton.momentoReunionTerminado)
            {
                ManagerControlador.singleton.momentoReunionTerminado = false;
                ManagerControlador.singleton.botonReunion.SetActive(true);
            }

            if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
            {
                ManagerControlador.singleton.AntesDeNormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                ManagerControlador.singleton.NormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                SimpleAudioManager.singleton.DesactivarHablando();
            }
        }
        else if (btnCasasInterno)
        {
            objetoActivar.SetActive(true);
            objetoDesactivar.SetActive(false);
            ManagerControlador.singleton.ActivarImagenIguana();

            if (posicionObjetivo != null)
            {
                MoverCamara.singleton.MoverHacia(posicionObjetivo);
                MirarCamara.singleton.ReanudarRotacion();
            }

            if (ManagerControlador.singleton.desastreSecundarioActivo)
            {
                SimpleAudioManager.singleton.DesactivarGritos();
                SimpleAudioManager.singleton.DesactivarAlarma();
            }

            if (ManagerControlador.singleton.momentoReunionTerminado)
            {
                ManagerControlador.singleton.momentoReunionTerminado = false;
                ManagerControlador.singleton.botonReunion.SetActive(true);
            }

            if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
            {
                ManagerControlador.singleton.AntesDeNormalizarCuenca();
            }

            if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
            {
                ManagerControlador.singleton.NormalizarCuenca();
            }
        }
    }
}

