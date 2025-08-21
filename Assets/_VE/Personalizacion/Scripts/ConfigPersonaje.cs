using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cfg_", menuName = "Personaje/Config de Combinación", order = 0)]
public class ConfigPersonaje : ScriptableObject
{
    [Header("Combinación")]
    public Raza raza;
    public Morfologia morfologia;
    public Atuendo atuendo;
    public Sexo sexo;

    [Header("Resultado")]
    [Tooltip("Prefab final que representa esta combinación exacta.")]
    public GameObject prefab;
}
