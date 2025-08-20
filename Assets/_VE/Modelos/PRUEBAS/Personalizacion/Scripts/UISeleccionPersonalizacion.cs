using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script puente entre la UI y el Gestor. 
/// Sirve tanto para botones con funciones específicas como para Dropdowns genéricos.
/// </summary>
public class UISeleccionPersonalizacion : MonoBehaviour
{
    [Header("Referencia")]
    public GestorPersonalizacion gestor;

    public Texture[] texturasDisponibles;
    private int indiceTextura = 0;

    // ===== BOTONES ESPECÍFICOS (ejemplos) =====
    // Asigna estas funciones a los OnClick de cada botón en el Inspector

    // RAZA
    public void BTN_Raza_Demonio() => gestor.SeleccionarRaza(Raza.Demonio);
    public void BTN_Raza_Humano() => gestor.SeleccionarRaza(Raza.Humano);
    public void BTN_Raza_Bestia() => gestor.SeleccionarRaza(Raza.Bestia);
    // ... crea los que necesites

    // MORFOLOGÍA
    public void BTN_Morfologia_Bruto() => gestor.SeleccionarMorfologia(Morfologia.Bruto);
    public void BTN_Morfologia_Heroe() => gestor.SeleccionarMorfologia(Morfologia.Heroe);
    // ...

    // ATUENDO
    public void BTN_Atuendo_Armadura() => gestor.SeleccionarAtuendo(Atuendo.Armadura);
    public void BTN_Atuendo_Tunica() => gestor.SeleccionarAtuendo(Atuendo.Tunica);
    public void BTN_Atuendo_SinAtuendo() => gestor.SeleccionarAtuendo(Atuendo.SinAtuendo);
    // ...


    // SEXO
    public void BTN_Sexo_Hombre() => gestor.SeleccionarSexo(Sexo.Hombre);
    public void BTN_Sexo_Mujer() => gestor.SeleccionarSexo(Sexo.Mujer);

    // COLOR
    public void BTN_ColorRojo() => gestor.CambiarColor(Color.red);
    public void BTN_ColorAzul() => gestor.CambiarColor(Color.blue);

    // TEXTURA
    public void BTN_SiguienteTextura() => gestor.BTN_SiguienteTextura();

}
