using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Random = UnityEngine.Random;



[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

public class NoiseGeneration : MonoBehaviour
{

    public float nAgua = 0.3f;
    public float escalaMontanhoso = 4;
    public float escalaMetaMontanhoso = 4;
    public float escalaAgua = 4;
    public float escalaCadenaMontanha = 3;
    public float escalaMontanha = 3;
    public float escalaPantanoso;
    public float escalaCadenaPantano;
    public float escalaBiomas;
    public float escalaWrapChico;
    public float escalaRuidoMontanha = 10;
    public float escalaPlaya = 10;
    public float escalaWrap;
    public float limiteWrap;
    public float barra = 0.4f;
    public float hieloterma = 0.75f;
    public float xColoracion;

    [SerializeField]
    private float scale = 10;

    public float semilla = 0;
    public SplineSegment[] Esplines;

    [System.Serializable]
    public class SplineSegment
    {
        public string nombre;
        public float inputStart = 0.0f;
        public float inputEnd = 1.0f;
        public float escala = 1;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    }
    //
    // funci�n que vomite (bioma, altura, recurso)
    //
    public Tuple<float,Color32, string> GetHeight(int x, int y)
    {
        float h;
        Color32 color;
        string recurso = "";


        color = new Color32(0,100,255,0);
        Color32 rojo = new Color32(255, 70, 70, 0);
        Color32 montanhos = new Color32(180, 80, 0, 0);
        Color32 nieve = new Color32(255, 255, 255, 0);
        Color32 pantano = new Color32(0, 155, 255, 0);


        Color32 desierto = new Color32(255, 255, 0, 0);
        Color32 bosque = new Color32(0, 80, 0, 0);
        Color32 tundra = new Color32(0, 200, 200, 0);
        Color32 jungla = new Color32(0, 80, 0, 0);


        h = Perlinazo(x, y, escalaAgua, 0);
        float ruidoMontanhoso = Perlinazo(x, y, escalaRuidoMontanha, -7);
        float montanhaMisma = Perlinazo(x, y, escalaMontanha, 7);
        float montanhaCadena = EvaluarSpline(PerlinWrap2(x, y, escalaWrap, -12), "cadena");
        float zonaMontanhosa = Mathf.Max(EvaluarSpline(Perlinazo(x, y, escalaMontanhoso, 8), "montanhaDonde"),EvaluarSpline(Perlinazo(x, y, escalaMetaMontanhoso, -8), "montanhaDonde"));

        float zonaCarcavosa = EvaluarSpline(Perlinazo(x, y, escalaPantanoso, 9), "carcavasDonde");
        float carcavasCadena = EvaluarSpline(Perlinazo(x, y, escalaCadenaPantano, 10), "carcavas");

        float pp = Perlinazo(x, y, escalaBiomas, 11);
        float temp = Perlinazo(x, y, escalaBiomas, 120);


        float playa = Perlinazo(x, y, escalaPlaya, -13);

        h += montanhaMisma * montanhaCadena * zonaMontanhosa * (ruidoMontanhoso*0.8f + 0.3f) - zonaCarcavosa*carcavasCadena;
        if (h > nAgua) {
            color = new Color32((byte)((h-nAgua)*155), 215, 0, 0);
            if(zonaMontanhosa > 0.5f && false) {
                color = montanhos;
            } else if (zonaCarcavosa > 0.2f) {
                color = Color.Lerp(pantano, color, 0.5f);
                if (carcavasCadena > 0.25f) {
                    color = pantano;
                    if (Random.value < 0.2f)
                    {
                        recurso = "junco";
                    }
                }
            } else {
                pp+= EvaluarSpline(h, "alturaAfectaPp");
                temp += EvaluarSpline(h, "alturaAfectaT");
                if (pp > 1-barra) {
                    if(temp> 1 - barra) {
                        color = new Color32(60, 100, 0, 0);   //jungla
                        if (Random.value < 0.5f)
                        {
                            recurso = "selva";
                        }
                    } else if(temp>0.4f) {
                        color = new Color32(0, 80, 0, 0);   //bosque
                        if (Random.value < 0.5f)
                        {
                            recurso = "bambu";
                        }
                    } else {
                        color = new Color32(0, 100, 70, 0);   //pinos
                        if (Random.value < 0.5f)
                        {
                            recurso = "pino";
                        }
                    }
                    //color = bosque;// Color.Lerp(color, bosque, Mathf.Clamp((pp - 0.5f)/0.2f,0,1) * xColoracion);
                } else if (pp > barra) {
                    if (temp > 1 - barra) {
                        color = new Color32(250, 211, 133, 0);   //sabana
                        if (Random.value < 0.05f)
                        {
                            recurso = "acacia";
                        }
                    } else if (temp > 0.4f) {
                        float z = PerlinWrap(x, y, escalaWrapChico, 3);
                        if (z > limiteWrap) {
                            color = new Color32(50, 100, 50, 0);    //bosque chico
                            if (Random.value < 0.3f)
                            {
                                recurso = "arbol";
                                if (Random.value < 0.05f)
                                {
                                    recurso = "honguito";
                                }
                            }
                        } else {
                            color = new Color32(155, 215, 0, 0);    //pasto
                            if(Random.value < 0.05f)
                            {
                                recurso = "flor";
                            }
                        }
                    } else {
                        color = new Color32(192, 233, 186, 0);    //nieve
                        if (Random.value < 0.3f)
                        {
                            recurso = "pinoNevado";
                            if (Random.value < 0.05f)
                            {
                                recurso = "mamut";
                            }
                        }
                    }
                } else {
                    if (temp > 1 - barra) {
                        color = new Color32(255, 255, 0, 0);    //desierto
                        if (Random.value < 0.05f)
                        {
                            recurso = "palmera";
                        }
                    } else if (temp > barra) {
                        color = new Color32(200, 200, 100, 0);   //sabana;// Color.Lerp(color, desierto, Mathf.Clamp((-pp + 0.5f) / 0.2f, 0, 1) * xColoracion);
                        if (Random.value < 0.05f)
                        {
                            recurso = "cactus";
                        }
                    } else {
                        float z = PerlinWrap(x, y, escalaWrapChico, 3);
                        if (z > limiteWrap) {
                            color = new Color32(50, 100, 50, 0);    //bosque chico
                            recurso = "arbol";
                            if (Random.value < 0.05f)
                            {
                                recurso = "pudu";
                            }
                        } else {
                            color = new Color32(155, 215, 0, 0);    //pasto
                            if (Random.value < 0.05f)
                            {
                                if (Random.value < 0.3f)
                                {
                                    recurso = "menta";
                                }
                                else
                                {
                                    recurso = "alamo";
                                }
                            }
                        }
                    }
                }
                //if(playa>0.5f && h < nAgua + 0.1f) {
                //    color = desierto;
                //}
                //if(PerlinWrap2(x, y, escalaWrap, -12)>0.48f && PerlinWrap2(x, y, escalaWrap, -12)<0.52f) {
                 //   color = rojo;
                //}
            }

            if (h > hieloterma) {
                color = nieve;
                recurso = "";
                if (Random.value < 0.05f)
                {
                    recurso = "obsidiana";
                }
            } else if (h > 0.71f) {
                color = montanhos;// Color.Lerp(montanhos, nieve, (h-0.71f)/(hieloterma-0.71f));
                recurso = "";
            }


        } else {
            h = nAgua;
        }

        return new Tuple<float, Color32, string> ( h, color, recurso);
    }
    
    public float PerlinWrap(int i, int j, float escala, float capa)
    {
        Vector2 q = new Vector2(Perlinazo(i, j, escala, -4), Perlinazo(i + 5.2f + capa * 3, j + 1.3f + capa * 3, escala, -4));

        return Perlinazo(i + 4 * q.x, j + 4 * q.y, escala, -3);
        /*
        vec2 q = vec2(fbm(p + vec2(0.0, 0.0)),
                        fbm(p + vec2(5.2, 1.3)));

        return fbm(p + 4.0 * q);
        */
    }
    public float PerlinWrap2(int i, int j, float escala, float capa)
    {
        Vector2 q = new Vector2(Perlinazo(i, j, escala, -4), Perlinazo(i + 5.2f+capa*3, j + 1.3f + capa * 3, escala, -4));
        Vector2 r = new Vector2(Perlinazo(i+4*q.x+1.7f + capa * 3, j + 4*q.y + 9.2f + capa * 3, escala, -5), Perlinazo(i + 4 * q.x + 8.3f + capa * 3, j + 4 * q.y + 2.8f + capa * 3, escala, -5));

        return Perlinazo(i + 4 * q.x, j + 4 * q.y, escala, -3);
        /*
        vec2 q = vec2(fbm(p + vec2(0.0, 0.0)),
                        fbm(p + vec2(5.2, 1.3)));

        return fbm(p + 4.0 * q);
        */
    }
    public float Perlinazo(float x, float y, float escala, float capa)
    {
        // Multiplicador para que el valor de escala no sea tan bajo (sino ser�a 0.003 vs 0.002 etc)
        int Lfijo = 40;

        // Calculamos la coordenada x escalada y modificada con la semilla.
        // Una forma de cambiar de semilla, es mirar el ruido de perlin muy lejos del origen, eso hace como si el mapa fuese totalmente diferente
        float xCoord = (float)(x) / Lfijo * escala + capa * 30 + semilla * 50;

        // Calculamos la coordenada y escalada.
        float yCoord = (float)(y) / Lfijo * escala + capa * 17 + semilla * 11; ;

        // C�lculo del valor de ruido Perlin principal en las coordenadas (xCoord, yCoord).
        float val = Mathf.PerlinNoise(xCoord, yCoord);

        // C�lculo del segundo nivel de detalle de ruido Perlin en coordenadas escaladas y trasladadas.
        float detail1 = Mathf.PerlinNoise(xCoord * 2 + 10, yCoord * 2 - 100);
        // Restamos 0.5 y multiplicamos por 0.5 para ajustar el rango y agregarlo al valor principal.
        val += 0.5f * (0.5f - detail1);

        // C�lculo del tercer nivel de detalle de ruido Perlin en coordenadas escaladas y trasladadas.
        float detail2 = Mathf.PerlinNoise(xCoord * 4 + 88, yCoord * 4 + 99);
        // Restamos 0.25 y multiplicamos por 0.25 para ajustar el rango y agregarlo al valor principal.
        val += 0.25f * (0.5f - detail2);

        // Devolvemos el valor de ruido Perlin final.
        float ex = 0.75f;
        val = (val + ex) / (1 + 2 * ex);
        return Math.Clamp(val, 0, 1);
    }


    public float EvaluarSpline(float input, string cualEspline)
    {
        SplineSegment segment = null;
        foreach (SplineSegment segmento in Esplines) {
            if (segmento.nombre == cualEspline) {
                segment = segmento;
            }
        }
        if(segment == null) {
            print("ERROR: No se encontro espline " + cualEspline);
            return 0;
        }
        if (input >= segment.inputStart && input <= segment.inputEnd) {
            float normalizedInput = Mathf.InverseLerp(segment.inputStart, segment.inputEnd, input);
            return segment.curve.Evaluate(normalizedInput) * segment.escala;
        }
        return 0;
    }

}
