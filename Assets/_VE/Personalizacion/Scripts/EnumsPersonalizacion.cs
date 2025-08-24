using UnityEngine;

// Enums para las 3 categorías
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
    TrajeNeopreno // (revisé la ortografía, si lo quieres "Neopreno" cámbialo aquí)
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


