﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BGM : MonoBehaviour {

    public static int beats;

    int beatOffset = 0; // set to a higher number to freeze values every x beats
    int nextBeat = 0;

    String guiInfo = "";

    public static float [][] spectrum;
    FMOD.DSP fft;
    static int lowEnd = 12; // low band cutoff
    static int midEnd = 24; // mid band cutoff
    static int highEnd = 32; // high band cutoff
    static int bufferSize = 10;
    static int bufferPosition = 0;
    static float [] lowEndBuffer = new float[bufferSize];
    static float bufferedLows;
    static float [] midEndBuffer = new float[bufferSize];
    static float bufferedMids;
    static float [] highEndBuffer = new float[bufferSize];
    static float bufferedHighs;

    const int WindowSize = 64;

    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    // Use this for initialization
    void Start () {
        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Debug/Random Event");

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();

        StartFFT();
    }

    void Update()
    {
        UpdateFFT();
        BufferMultibandFFT();
    }

    void OnDestroy()
    {
        StopMusic();

    }

    void OnGUI()
    {
        if (BGM.beats >= nextBeat)
        {
            guiInfo = String.Format("BGM: Current Bar = {0}, Last Marker = {1}", timelineInfo.currentMusicBar, (string)timelineInfo.lastMarker);
            guiInfo += String.Format(", lowEnd = {0}dB, midEnd = {1}dB, highEnd = {2}dB", bufferedLows.ToString("0"), bufferedMids.ToString("0"), bufferedHighs.ToString("0"));
            //guiInfo += String.Format(", lowEnd = {0}dB, midEnd = {1}dB, highEnd = {2}dB", GetFFTLows().ToString("0.00"), GetFFTMids().ToString("0.00"), GetFFTHighs().ToString("0.00")); 
            nextBeat += beatOffset;
        }
        GUILayout.Box(guiInfo);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
    {
        // Retrieve the user data
        IntPtr timelineInfoPtr;
        instance.getUserData(out timelineInfoPtr);

        // Get the object to store beat and marker details

        if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;


            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = BGM.beats = parameter.bar;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }

        }
        return FMOD.RESULT.OK;
    }

    void UpdateFFT()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        spectrum = fftData.spectrum;
    }

    void StartFFT()
    {
        FMODUnity.RuntimeManager.LowlevelSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);

        FMOD.ChannelGroup channelGroup;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);
    }

    void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
        musicInstance.setUserData(IntPtr.Zero);
    }

    public void SwitchToGameOverMusic()
    {
        StopMusic();
        musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Debug/BadEnd");
        musicInstance.start();
    }

    static float GetFFTInRange(int min, int max)
    {
        float avg = 0;

        for (int i = min; i <= max; i++)
        {
            try
            {
                avg += spectrum[0][i];
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        return lin2dB(avg /= (max - min));
    }

    // Get the avg value of the high frequencies
    static float GetFFTHighs()
    {
        float avg = 0;
        int min = midEnd;
        int max = highEnd;

        for (int i = min; i <= max; i++)
        {
            try
            {
                avg += spectrum[0][i];
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return lin2dB(avg /= (max - min));
    }

    // Get the avg value of the mid frequencies
    static float GetFFTMids()
    {
        float avg = 0;
        int min = lowEnd;
        int max = midEnd;

        for (int i = min; i <= max; i++)
        {
            try
            {
                avg += spectrum[0][i];
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return lin2dB(avg /= (max - min));
    }

    // Get the avg value of the low frequencies
    static float GetFFTLows()
    {
        float avg = 0;
        int min = 0;
        int max = lowEnd;

        for (int i = min; i <= max; i++)
        {
            try
            {
                avg += spectrum[0][i];
            } catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return lin2dB(avg /= (max - min));
    }

    // Buffer Low, Mids, and Highs
    static void BufferMultibandFFT()
    {
        lowEndBuffer[bufferPosition] = BGM.GetFFTLows();
        midEndBuffer[bufferPosition] = BGM.GetFFTMids();
        highEndBuffer[bufferPosition] = BGM.GetFFTHighs();

        float lowAvg = 0;
        float midAvg = 0;
        float highAvg = 0;

        for (int i = 0; i < bufferSize - 1; i++)
        {
            lowAvg += lowEndBuffer[bufferPosition];
            midAvg += midEndBuffer[bufferPosition];
            highAvg += highEndBuffer[bufferPosition];
        }

        bufferedLows = lowAvg /= bufferSize;
        bufferedMids = midAvg /= bufferSize;
        bufferedHighs = highAvg /= bufferSize;

        bufferPosition++;
        if (bufferPosition >= bufferSize - 1) {
            bufferPosition = 0;
        }
    }

    // Linear scale to Decibel scale conversion
    static float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }


}
