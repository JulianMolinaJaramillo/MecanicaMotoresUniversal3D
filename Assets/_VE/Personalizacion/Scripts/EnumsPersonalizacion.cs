using UnityEngine;

// Enums para las 4 categorías. Raza, Morfologia, Atuendo y sexo
public enum Raza
{
    Bestia,
    Extraterrestre,
    Hibrido,
    Demonio,
    Superhumano,
    Humano
}

public enum Morfologia
{
    Normal,
    Heroe,
    Gigante,
    GenioDelMal,
    Bruto,
    Antiheroe
}

public enum Atuendo
{
    SinAtuendo,
    Armadura,
    Tunica,
    Casual,
    TrajeNeopreno
}

public enum Sexo
{
    Hombre,
    Mujer,
    otro
}
public class EnumsPersonalizacion : MonoBehaviour
{

}

//Clases serializables para los porcentajes
[System.Serializable]
public class OpcionRaza
{
    public Raza raza;
    [Range(0, 100)] public float porcentaje;
}

[System.Serializable]
public class OpcionMorfologia
{
    public Morfologia morfologia;
    [Range(0, 100)] public float porcentaje;
}

[System.Serializable]
public class OpcionAtuendo
{
    public Atuendo atuendo;
    [Range(0, 100)] public float porcentaje;
}

[System.Serializable]
public class OpcionSexo
{
    public Sexo sexo;
    [Range(0, 100)] public float porcentaje;
}


