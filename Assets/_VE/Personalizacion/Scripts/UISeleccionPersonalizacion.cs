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

    // ===== BOTONES ESPECÍFICOS (ejemplos) =====
    // Asigna estas funciones a los OnClick de cada botón en el Inspector

    // RAZA
    public void BTN_Raza_Demonio() => gestor.SeleccionarRaza(Raza.Demonio);
    public void BTN_Raza_Humano() => gestor.SeleccionarRaza(Raza.Humano);
    public void BTN_Raza_Bestia() => gestor.SeleccionarRaza(Raza.Bestia);
    public void BTN_Raza_Superhumano() => gestor.SeleccionarRaza(Raza.Superhumano);
    public void BTN_Raza_Hibrido() => gestor.SeleccionarRaza(Raza.Hibrido);
    public void BTN_Raza_Extraterrestre() => gestor.SeleccionarRaza(Raza.Extraterrestre);
    // ...

    // MORFOLOGÍA
    public void BTN_Morfologia_Bruto() => gestor.SeleccionarMorfologia(Morfologia.Bruto);
    public void BTN_Morfologia_Heroe() => gestor.SeleccionarMorfologia(Morfologia.Heroe);
    public void BTN_Morfologia_Normal() => gestor.SeleccionarMorfologia(Morfologia.Normal);
    public void BTN_Morfologia_Gigante() => gestor.SeleccionarMorfologia(Morfologia.Gigante);
    public void BTN_Morfologia_Hechicero() => gestor.SeleccionarMorfologia(Morfologia.GenioDelMal);
    public void BTN_Morfologia_Antiheroe() => gestor.SeleccionarMorfologia(Morfologia.Antiheroe);
    // ...

    // ATUENDO
    public void BTN_Atuendo_Armadura() => gestor.SeleccionarAtuendo(Atuendo.Armadura);
    public void BTN_Atuendo_Tunica() => gestor.SeleccionarAtuendo(Atuendo.Tunica);
    public void BTN_Atuendo_SinAtuendo() => gestor.SeleccionarAtuendo(Atuendo.SinAtuendo);
    public void BTN_Atuendo_TrajeNeopreno() => gestor.SeleccionarAtuendo(Atuendo.TrajeNeopreno);
    public void BTN_Atuendo_Casual() => gestor.SeleccionarAtuendo(Atuendo.Casual);
    // ...

    // SEXO
    public void BTN_Sexo_Hombre() => gestor.SeleccionarSexo(Sexo.Hombre);
    public void BTN_Sexo_Mujer() => gestor.SeleccionarSexo(Sexo.Mujer);
    public void BTN_Sexo_Otro() => gestor.SeleccionarSexo(Sexo.otro);

    // TEXTURA
    public void BTN_SiguienteTextura() => gestor.BTN_SiguienteTextura();

}
