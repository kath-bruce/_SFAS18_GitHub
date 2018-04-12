using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //------------------------------------------------
    
    [SerializeField]
    List<AudioClip> m_audioClips;

    //------------------------------------------------

    Dictionary<string, AudioClip> m_audioDict = new Dictionary<string, AudioClip>();
    
    //------------------------------------------------

    void Start()
    {
        //means that other parts of the code only need to know the name of the clip
        foreach (AudioClip clip in m_audioClips)
        {
            m_audioDict.Add(clip.name, clip);
        }
    }

    //-----------------------------------------------

    public AudioClip GetAudioClip(string clipName)
    {
        AudioClip clip;

        //ensures an entry is not created in the dictionary if clipName is present
        m_audioDict.TryGetValue(clipName, out clip);

        return clip;
    }
}
