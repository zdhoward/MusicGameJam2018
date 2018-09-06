using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BGM : MonoBehaviour {

    public static int beats;

    public static float [][] spectrum;
    FMOD.DSP fft;
    static int lowEnd = 12;
    static int midEnd = 24;
    static int highEnd = 32;
    static float [] lowEndBuffer;
    static float bufferedLows;
    static float [] midEndBuffer;
    static float bufferedMids;
    static float [] highEndBuffer;
    static float bufferedHighs;
    static int bufferSize = 5;
    static int bufferPosition = 0;

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
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
    }

    void OnGUI()
    {
        String guiInfo = String.Format("BGM: Current Bar = {0}, Last Marker = {1}", timelineInfo.currentMusicBar, (string)timelineInfo.lastMarker);
        guiInfo += String.Format(", lowEnd = {0}dB, midEnd = {1}dB, highEnd = {2}dB", GetFFTLows().ToString("0.00"), GetFFTMids().ToString("0.00"), GetFFTHighs().ToString("0.00")); //bufferedLows, bufferedMids, bufferedHighs);// 
        //guiInfo += ", lowEnd = " + GetAvgFFTLows().ToString("0.00") + ", midEnd = " + GetAvgFFTMids().ToString("0.00") + ", highEnd = " + GetAvgFFTHighs().ToString("0.00");
        GUILayout.Box(guiInfo);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
    {
        // Retrieve the user data
        IntPtr timelineInfoPtr;
        instance.getUserData(out timelineInfoPtr);

        // Get the object to store beat and marker details
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
        return FMOD.RESULT.OK;
    }

    void UpdateFFT()
    {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        spectrum = fftData.spectrum;
        BufferMultiband();
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

    static float GetFFTInRange(int min, int max)
    {
        float avg = 0;

        for (int i = min; i <= max; i++)
        {
            avg += spectrum[0][i];
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
            avg += spectrum[0][i];
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
            avg += spectrum[0][i];
        }

        return lin2dB(avg /= (max - min));
    }

    // Get the avg value of the low frequencies
    static float GetFFTLows()
    {
        float avg = 0;
        int min = 0;
        int max = lowEnd - 1;

        for (int i = min; i <= max; i++)
        {
            avg += spectrum[0][i];
        }

        return lin2dB(avg /= (max - min));
    }

    // Buffer Low, Mids, and Highs
    static void BufferMultiband()
    {
        lowEndBuffer[bufferPosition] = GetFFTLows();
        midEndBuffer[bufferPosition] = GetFFTMids();
        highEndBuffer[bufferPosition] = GetFFTHighs();

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
