using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letrero : PropertyAttribute
{
	public string etiqueta;
	public Color color;

	public Letrero(string _etiqueta)
	{
		etiqueta = _etiqueta;
	}

	public void Imprimir(string i)
	{
		
	}
}
