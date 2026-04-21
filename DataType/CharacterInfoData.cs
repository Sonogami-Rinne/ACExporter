using System;
using System.Collections.Generic;

[Serializable]
internal class CharacterInfoData
{
	public string Name;

	public int Personality;

	public float VoiceRate;

	public float PupilWidth;

	public float PupilHeight;

	public float PupilY;

    public float HlUpX;

    public float HlUpY;

	public float HlDownX;

	public float HlDownY;

	public List<float> ShapeInfoFace = new List<float>();

	public List<float> ShapeInfoBody = new List<float>();

	public List<float> pupilOffset;

	public List<float> pupilScale;

	public List<float> highlight01Offset;

	public List<float> highlight01Scale;

    public List<float> highlight02Offset;

    public List<float> highlight02Scale;
    
	public List<float> highlight03Offset;

    public List<float> highlight03Scale;
}
