using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class FMODToolbox : MonoBehaviour {

    float _bpm;

    FMOD.Studio.EventInstance musicInstance;
    FMOD.DSP fft;

    public LineRenderer lineRenderer;

    const int WindowSize = 64;

    const float WIDTH = 10.0f;
    const float HEIGHT = 0.1f;

    public float [][] _spectrum;

    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    // Use this for initialization
    void Start () {
        //_spectrum = new Array2D();

        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = WindowSize;
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .1f;

        musicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Debug/Random Event");


        FMODUnity.RuntimeManager.LowlevelSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fft);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING);
        fft.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, WindowSize * 2);

        FMOD.ChannelGroup channelGroup;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fft);

        musicInstance.start();
    }
	
	// Update is called once per frame
	void Update () {
        IntPtr unmanagedData;
        uint length;
        fft.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        FMOD.DSP_PARAMETER_FFT fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        var spectrum = _spectrum = fftData.spectrum;

        // Draw line
        if (fftData.numchannels > 0)
        {
            var pos = Vector3.zero;
            pos.x = WIDTH * -0.5f + 6;

            for (int i = 0; i < WindowSize - (int)(WindowSize*0.2); ++i)
            {
                pos.x += (WIDTH / WindowSize);

                float level = lin2dB(spectrum[0][i]);
                pos.y = (80 + level) * HEIGHT;

                lineRenderer.SetPosition(i, pos);
            }
        }
    }

    float lin2dB(float linear)
    {
        return Mathf.Clamp(Mathf.Log10(linear) * 20.0f, -80.0f, 0.0f);
    }
}
