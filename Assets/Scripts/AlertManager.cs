using UnityEngine;
using System.Collections.Generic;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }

    private List<EnemyAI> listaDeGuardias = new List<EnemyAI>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistrarGuardia(EnemyAI nuevoGuardia)
    {
        if (!listaDeGuardias.Contains(nuevoGuardia))
        {
            listaDeGuardias.Add(nuevoGuardia);
        }
    }

    public void EnviarAlertaGlobal(Vector3 posicionIntruso, EnemyAI guardiaQueAvisa)
    {
        foreach (EnemyAI guardia in listaDeGuardias)
        {
            if (guardia != guardiaQueAvisa)
            {
                guardia.RecibirAlertaDeCompañero(posicionIntruso);
            }
        }
    }

    public void ResetearAlertasGlobales()
    {
        foreach (EnemyAI guardia in listaDeGuardias)
        {
            if (guardia != null)
            {
                guardia.OlvidarIntrusoYVolverAPatrulla();
            }
        }
    }
}