#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VibeCooking.Editor
{
    public static class AudioAssetGenerator
    {
        private const string AudioPath = "Assets/_Project/Audio";
        private const int SampleRate = 44100;

        [MenuItem("Vibe Cooking/Generate Audio Assets")]
        public static void GenerateAudioAssets()
        {
            if (!Directory.Exists(AudioPath))
                Directory.CreateDirectory(AudioPath);

            CreateBtnClickWav($"{AudioPath}/sfx_btn_click.wav");
            CreateBellDingWav($"{AudioPath}/sfx_bell_ding.wav");
            CreateCoinCollectWav($"{AudioPath}/sfx_coin_collect.wav");

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[VibeCooking] Procedural SFX Audio files generated successfully in " + AudioPath + "!</color>");
        }

        private static void CreateBtnClickWav(string filePath)
        {
            // Cozy wooden/ceramic click (~0.06s)
            int sampleCount = (int)(SampleRate * 0.06f);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / SampleRate;
                float env = Mathf.Exp(-t * 90f); // Fast decay
                // Frequency sweep from 600 Hz down to 220 Hz + mild noise click
                float freq = Mathf.Lerp(600f, 220f, (float)i / sampleCount);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                float noise = (float)(new System.Random(i).NextDouble() * 2.0 - 1.0) * 0.15f;

                samples[i] = Mathf.Clamp((wave + noise) * env * 0.8f, -1f, 1f);
            }

            WriteWavFile(filePath, samples);
        }

        private static void CreateBellDingWav(string filePath)
        {
            // Resonant warm brass bell chime (~1.2s)
            int sampleCount = (int)(SampleRate * 1.2f);
            float[] samples = new float[sampleCount];

            float f1 = 1760f; // A6 brass fundamental
            float f2 = 2640f; // Perfect fifth overtone (E7)
            float f3 = 3520f; // High octave shimmer (A7)

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / SampleRate;
                // Double exponential decay: sharp strike followed by long singing brass sustain
                float strikeEnv = Mathf.Exp(-t * 30f);
                float ringEnv = Mathf.Exp(-t * 3.5f);

                float wave1 = Mathf.Sin(2f * Mathf.PI * f1 * t) * 0.55f;
                float wave2 = Mathf.Sin(2f * Mathf.PI * f2 * t) * 0.30f;
                float wave3 = Mathf.Sin(2f * Mathf.PI * f3 * t) * 0.15f;

                float sample = (wave1 + wave2 + wave3) * (ringEnv * 0.75f + strikeEnv * 0.25f);
                samples[i] = Mathf.Clamp(sample * 0.85f, -1f, 1f);
            }

            WriteWavFile(filePath, samples);
        }

        private static void CreateCoinCollectWav(string filePath)
        {
            // Crisp dual-coin clink (~0.35s)
            int sampleCount = (int)(SampleRate * 0.35f);
            float[] samples = new float[sampleCount];

            float coin1Time = 0f;
            float coin2Time = 0.07f; // Clink 2 slightly delayed

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / SampleRate;
                float sample = 0f;

                // Coin 1 (2200 Hz)
                if (t >= coin1Time)
                {
                    float dt1 = t - coin1Time;
                    float env1 = Mathf.Exp(-dt1 * 22f);
                    sample += Mathf.Sin(2f * Mathf.PI * 2200f * dt1) * env1 * 0.5f;
                    sample += Mathf.Sin(2f * Mathf.PI * 3300f * dt1) * env1 * 0.2f;
                }

                // Coin 2 (2780 Hz)
                if (t >= coin2Time)
                {
                    float dt2 = t - coin2Time;
                    float env2 = Mathf.Exp(-dt2 * 20f);
                    sample += Mathf.Sin(2f * Mathf.PI * 2780f * dt2) * env2 * 0.6f;
                    sample += Mathf.Sin(2f * Mathf.PI * 4150f * dt2) * env2 * 0.25f;
                }

                samples[i] = Mathf.Clamp(sample * 0.8f, -1f, 1f);
            }

            WriteWavFile(filePath, samples);
        }

        private static void WriteWavFile(string filePath, float[] samples)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                int subchunk2Size = samples.Length * 2; // 16-bit = 2 bytes per sample
                int chunkSize = 36 + subchunk2Size;

                // RIFF header
                writer.Write(new char[] { 'R', 'I', 'F', 'F' });
                writer.Write(chunkSize);
                writer.Write(new char[] { 'W', 'A', 'V', 'E' });

                // fmt subchunk
                writer.Write(new char[] { 'f', 'm', 't', ' ' });
                writer.Write(16);             // Subchunk1Size for PCM
                writer.Write((short)1);        // AudioFormat 1 = PCM
                writer.Write((short)1);        // NumChannels = 1 (Mono)
                writer.Write(SampleRate);      // SampleRate
                writer.Write(SampleRate * 2);  // ByteRate = SampleRate * 1 * 16/8
                writer.Write((short)2);        // BlockAlign = 1 * 16/8
                writer.Write((short)16);       // BitsPerSample = 16

                // data subchunk
                writer.Write(new char[] { 'd', 'a', 't', 'a' });
                writer.Write(subchunk2Size);

                for (int i = 0; i < samples.Length; i++)
                {
                    short pcmSample = (short)(samples[i] * 32767f);
                    writer.Write(pcmSample);
                }
            }
        }
    }
}
#endif
