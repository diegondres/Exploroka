using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

public class NoiseGeneration : MonoBehaviour
{

    public float escalaMontanhoso = 4;
    public float escalaCadenaMontanha = 3;
    public float escalaMontanha = 3;
    public float escalaPantanoso;
    public float escalaCadenaPantano;
    public float escalaBiomas;

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
    // función que vomite (bioma, altura)
    public Tuple<float,Color32> GetHeight(int x, int y)
    {
        float nAgua = 0.3f;
        Color32 color = new Color32(0,100,255,0);
        Color32 rojo = new Color32(255, 70, 70, 0);
        Color32 montanhos = new Color32(180, 80, 0, 0);
        Color32 nieve = new Color32(255, 255, 255, 0);
        Color32 pantano = new Color32(0, 155, 255, 0);


        Color32 desierto = new Color32(255, 255, 0, 0);
        Color32 bosque = new Color32(0, 80, 0, 0);
        Color32 tundra = new Color32(0, 200, 200, 0);
        Color32 jungla = new Color32(0, 80, 0, 0);



        float h = Perlinazo(x, y, 1, 0);
        float montanhaMisma = Perlinazo(x, y, escalaMontanha, 7);
        float montanhaCadena = EvaluarSpline(Perlinazo(x, y, escalaCadenaMontanha, 6), "cadena");
        float zonaMontanhosa = EvaluarSpline(Perlinazo(x, y, escalaMontanhoso, 8), "montañaDonde");

        float zonaCarcavosa = EvaluarSpline(Perlinazo(x, y, escalaPantanoso, 9), "carcavasDonde");
        float carcavasCadena = EvaluarSpline(Perlinazo(x, y, escalaCadenaPantano, 10), "carcavas");

        float pp = Perlinazo(x, y, escalaBiomas, 11);
        float temp = Perlinazo(x, y, escalaBiomas, 12);

        h += montanhaMisma * montanhaCadena * zonaMontanhosa - zonaCarcavosa*carcavasCadena;
        if (h > nAgua) {
            color = new Color32((byte)((h-nAgua)*155), 215, 0, 0);
            if(zonaMontanhosa > 0.3f) {
                color = montanhos;
            } else if (zonaMontanhosa > 0.25f) {
                color = Color.Lerp(montanhos, color, 0.5f);
            } else if (zonaCarcavosa > 0.3f) {
                color = Color.Lerp(pantano, color, 0.5f);
                if (carcavasCadena > 0.25f) {
                    color = pantano;
                }
            } else {
                if(pp>0.5f) {
                    color = Color.Lerp(color, bosque, (pp - 0.5f) * xColoracion);
                } else {
                    color = Color.Lerp(color, desierto, (0.5f - pp)* xColoracion);
                }
                /*
                if (temp > 0.5f) {
                    color = Color.Lerp(color, jungla, (temp - 0.5f)* xColoracion);
                } else {
                    color = Color.Lerp(color, tundra, (0.5f - temp)* xColoracion);
                }
                */
            }
            
            
            
            if (h > 0.85f) {
                color = nieve;
            }


        } else {
            h = nAgua;
        }

        return new Tuple<float, Color32> ( h, color );
    }

    public float Perlinazo(int x, int y, float escala, float capa)
    {
        // Multiplicador para que el valor de escala no sea tan bajo (sino sería 0.003 vs 0.002 etc)
        int Lfijo = 40;

        // Calculamos la coordenada x escalada y modificada con la semilla.
        // Una forma de cambiar de semilla, es mirar el ruido de perlin muy lejos del origen, eso hace como si el mapa fuese totalmente diferente
        float xCoord = (float)(x) / Lfijo * escala + capa * semilla * 3000 + semilla * 5000;

        // Calculamos la coordenada y escalada.
        float yCoord = (float)(y) / Lfijo * escala;

        // Cálculo del valor de ruido Perlin principal en las coordenadas (xCoord, yCoord).
        float val = Mathf.PerlinNoise(xCoord, yCoord);

        // Cálculo del segundo nivel de detalle de ruido Perlin en coordenadas escaladas y trasladadas.
        float detail1 = Mathf.PerlinNoise(xCoord * 2 + 10, yCoord * 2 - 100);
        // Restamos 0.5 y multiplicamos por 0.5 para ajustar el rango y agregarlo al valor principal.
        val += 0.5f * (0.5f - detail1);

        // Cálculo del tercer nivel de detalle de ruido Perlin en coordenadas escaladas y trasladadas.
        float detail2 = Mathf.PerlinNoise(xCoord * 4 + 88, yCoord * 4 + 99);
        // Restamos 0.25 y multiplicamos por 0.25 para ajustar el rango y agregarlo al valor principal.
        val += 0.25f * (0.5f - detail2);

        // Devolvemos el valor de ruido Perlin final.
        return Math.Clamp(val / 1.25f, 0, 1);
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
            print("ERROR: No se encontró espline " + cualEspline);
            return 0;
        }
        if (input >= segment.inputStart && input <= segment.inputEnd) {
            float normalizedInput = Mathf.InverseLerp(segment.inputStart, segment.inputEnd, input);
            return segment.curve.Evaluate(normalizedInput) * segment.escala;
        }
        return 0;
    }

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float offsetX, float offsetZ, Wave[] waves)
    {
        //    Debug.Log("offsetZ: " +  offsetZ + " offsetX: " + offsetX);
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++) {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                // calculate sample indices based on the coordinates, the scale and the offset
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;
                // generate noise value using PerlinNoise
                float noise = 0f;
                float normalization = 0f;

                foreach (Wave wave in waves) {
                    // generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }

                // normalize the noise value so that it is within 0 and 1
                noise /= normalization;

                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }

    public float[,] GenerateUniformNoiseMap(int mapDepth, int mapWidth, float centerVertexZ, float maxDistanceZ, float offsetZ)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int zIndex = 0; zIndex < mapDepth; zIndex++) {
            // calculate the sampleZ by summing the index and the offset
            float sampleZ = zIndex + offsetZ;
            // calculate the noise proportional to the distance of the sample to the center of the level
            float noise = Mathf.Abs(sampleZ - centerVertexZ) / maxDistanceZ;
            // apply the noise for all points with this Z coordinate
            for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                noiseMap[mapDepth - zIndex - 1, xIndex] = noise;
            }
        }
        return noiseMap;
    }


}
