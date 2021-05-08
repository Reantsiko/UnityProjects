using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { Local, Global };
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        if (mapWidth <= 0 || mapHeight <= 0) return default;
        
        
        Vector2[] octaveOffsets = CalculateOctaveOffsets(sampleCenter, settings, out float maxPossibleHeight);

        var noiseMap = CreateNoiseMap(mapWidth, mapHeight, settings, octaveOffsets, maxPossibleHeight, out float maxLocalNoiseHeight, out float minLocalNoiseHeight);
        if (noiseMap == null)
            return default;

        if (settings.normalizeMode == NormalizeMode.Local)
            UpdateToLocalMode(mapWidth, mapHeight, minLocalNoiseHeight, maxLocalNoiseHeight, ref noiseMap);
        return noiseMap;
    }

    private static float[,] CreateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2[] octaveOffsets, float maxPossibleHeight, out float maxLocalNoiseHeight, out float minLocalNoiseHeight)
    {
        maxLocalNoiseHeight = float.MinValue;
        minLocalNoiseHeight = float.MaxValue;
        float[,] noiseMap;

        if (mapWidth <= 0 || mapHeight <= 0 || ( noiseMap = new float[mapWidth, mapHeight]) == default)
            return default;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                    noiseMap[x, y] = GetGlobalCoordValue(maxPossibleHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }

    private static Vector2[] CalculateOctaveOffsets(Vector2 sampleCenter, NoiseSettings settings, out float maxPossibleHeight)
    {
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        System.Random prng = new System.Random(settings.seed);
        float amplitude = 1;
        maxPossibleHeight = 0;
        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-10000, 10000) + settings.offSet.x + sampleCenter.x;
            float offSetY = prng.Next(-10000, 10000) - settings.offSet.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offSetY);
            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }
        return octaveOffsets;
    }

    private static float GetGlobalCoordValue(float maxPossibleHeight, float value)
    {
        float normalizedHeight = (value + 1) / maxPossibleHeight;
        return Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
    }

    private static void UpdateToLocalMode(int mapWidth, int mapHeight, float minLocalNoiseHeight, float maxLocalNoiseHeight, ref float[,] noiseMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
        }
    }
}