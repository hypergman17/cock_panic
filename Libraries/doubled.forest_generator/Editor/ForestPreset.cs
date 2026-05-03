using Sandbox;
using System.Collections.Generic;

namespace Doubled.ForestGenerator
{
	/// <summary>
	/// Prefab entry with a probability weight.
	/// </summary>
	public struct ForestPrefabConfig
	{
		public ForestPrefabConfig( )
		{
			Prefab = null;
			Weight = 1f;
		}

		[Property] public PrefabFile Prefab { get; set; }
		[Property, Title( "Weight" )] public float Weight { get; set; }
	}

	/// <summary>
	/// A plain data class for saving/loading Forest Generator presets.
	/// Serialized as JSON via FileSystem.Data.
	/// </summary>
	public class ForestPresetData
	{
		public BBox AreaBounds { get; set; } = new BBox( new Vector3( -2000, -2000, 0 ), new Vector3( 2000, 2000, 1000 ) );
		public float Spacing { get; set; } = 100f;
		public float NoiseScale { get; set; } = 0.01f;
		public float NoiseThreshold { get; set; } = 0.4f;
		public float Seed { get; set; } = 0f;
		public float MinScale { get; set; } = 0.8f;
		public float MaxScale { get; set; } = 1.2f;
		public bool AlignToNormal { get; set; } = false;

		// Note: Prefab references (GameObjects) cannot be serialized directly to JSON.
		// To save a preset including prefab links, store their resource paths instead.
		public List<string> PrefabPaths { get; set; } = new();
		public List<float> PrefabWeights { get; set; } = new();
	}

	/// <summary>
	/// Preset data for the fixed-layer Terrain Painter (Base / Shore / Slope).
	/// Serialized as JSON via FileSystem.Data.
	/// </summary>
	public class TerrainLayerPresetData
	{
		public int   BaseMaterialIndex  { get; set; } = 0;
		// Shore
		public bool  EnableShore        { get; set; } = true;
		public int   ShoreMaterialIndex { get; set; } = 1;
		public float WaterLevel         { get; set; } = 0f;
		public float ShoreWidth         { get; set; } = 300f;
		public float ShoreBlend         { get; set; } = 0.3f;
		public float ShoreNoiseScale    { get; set; } = 0f;
		public float ShoreNoiseStrength { get; set; } = 0f;
		public float ShoreMaxHeight     { get; set; } = 0f;
		// Slope
		public bool  EnableSlope        { get; set; } = true;
		public int   SlopeMaterialIndex { get; set; } = 2;
		public float MinSlope           { get; set; } = 0.35f;
		public float SlopeBlend         { get; set; } = 0.15f;
		public float AvoidShoreDistance { get; set; } = 0f;
		public float AvoidShoreFade     { get; set; } = 0f;
		public float SlopeNoiseScale    { get; set; } = 0f;
		public float SlopeNoiseStrength { get; set; } = 0f;
	}
}

