using System;
using System.IO;
using System.Linq;
using Editor;
using Sandbox;
using Sandbox.Utility;
using FileSystem = Sandbox.FileSystem;

namespace Doubled.ForestGenerator.Editor
{
	/// <summary>
	/// Procedural terrain material painter with three fixed, purpose-built layers:
	/// Base (grass), Shore (sand), Slope (rock). Applies them in order and resolves
	/// the correct two-material blend at every texel.
	/// </summary>
	[Dock( "Editor", "Terrain Painter", "brush" )]
	public class TerrainPainter : Widget
	{
		// ── Base layer ────────────────────────────────────────────────────────────
		/// <summary>Material index for the base ground (e.g. grass). Covers everything.</summary>
		[Property, Title( "Base Material Index" )] public int BaseMaterialIndex { get; set; } = 0;

		// ── Shore layer ───────────────────────────────────────────────────────────
		[Property, Title( "Enable Shore Layer" )] public bool EnableShore { get; set; } = true;
		/// <summary>Material index for the shore (e.g. sand).</summary>
		[Property, Title( "Shore Material Index" )] public int ShoreMaterialIndex { get; set; } = 1;
		/// <summary>World Z of the water surface.</summary>
		[Property, Title( "Water Level (world Z)" )] public float WaterLevel { get; set; } = 0f;
		/// <summary>How many world units from WaterLevel to paint shore.</summary>
		[Property, Title( "Shore Width (world units)" )] public float ShoreWidth { get; set; } = 300f;
		/// <summary>Fraction of ShoreWidth used as smooth fade into base (0=hard edge).</summary>
		[Property, Range( 0f, 1f ), Title( "Shore Blend (0=hard, 1=full fade)" )] public float ShoreBlend { get; set; } = 0.3f;
		/// <summary>Perlin noise scale that organically shifts the shore boundary.</summary>
		[Property, Range( 0f, 0.05f ), Title( "Shore Noise Scale" )] public float ShoreNoiseScale { get; set; } = 0f;
		/// <summary>How many world units of noise displacement on the shore edge.</summary>
		[Property, Title( "Shore Noise Strength (world units)" )] public float ShoreNoiseStrength { get; set; } = 0f;
		/// <summary>Don't paint shore above this world height (avoids painting cliffs). 0 = disabled.</summary>
		[Property, Title( "Shore Max Height (0=disable)" )] public float ShoreMaxHeight { get; set; } = 0f;

		// ── Slope layer ───────────────────────────────────────────────────────────
		[Property, Title( "Enable Slope Layer" )] public bool EnableSlope { get; set; } = true;
		/// <summary>Material index for the slope (e.g. rock).</summary>
		[Property, Title( "Slope Material Index" )] public int SlopeMaterialIndex { get; set; } = 2;
		/// <summary>Slope threshold at which rock starts appearing (0=flat, 1=vertical).</summary>
		[Property, Range( 0f, 1f ), Title( "Min Slope" )] public float MinSlope { get; set; } = 0.35f;
		/// <summary>Slope units of smooth fade below MinSlope (fade into grass/sand).</summary>
		[Property, Range( 0f, 0.5f ), Title( "Slope Blend Width" )] public float SlopeBlend { get; set; } = 0.15f;
		/// <summary>Rock won't paint within this world distance of the shore. 0 = disabled.</summary>
		[Property, Title( "Avoid Shore Distance (world units, 0=disable)" )] public float AvoidShoreDistance { get; set; } = 0f;
		/// <summary>Smooth fade width as rock approaches the shore exclusion zone.</summary>
		[Property, Title( "Avoid Shore Fade (world units)" )] public float AvoidShoreFade { get; set; } = 0f;
		/// <summary>Perlin noise scale to add organic variation to rocky boundaries.</summary>
		[Property, Range( 0f, 0.05f ), Title( "Slope Noise Scale" )] public float SlopeNoiseScale { get; set; } = 0f;
		/// <summary>How much Perlin noise shifts the effective slope value (adds/removes rock).</summary>
		[Property, Range( 0f, 0.3f ), Title( "Slope Noise Strength" )] public float SlopeNoiseStrength { get; set; } = 0f;

		// ── Preset ────────────────────────────────────────────────────────────────
		private static string PresetsFolder => "terrain_painter";

		public TerrainPainter( Widget parent ) : base( parent, false )
		{
			Layout = Layout.Column();
			Layout.Margin = 10;
			Layout.Spacing = 6;

			var scroll = new ScrollArea( this );
			scroll.Canvas = new Widget( scroll );
			scroll.Canvas.Layout = Layout.Column();
			scroll.Canvas.Layout.Margin = 16;
			scroll.Canvas.Layout.Spacing = 8;
			Layout.Add( scroll );

			var body = scroll.Canvas.Layout;
			var so = this.GetSerialized();

			// Helper: label + control in one call
			void Field( string label, Widget ctrl ) { body.Add( new Label( label ) ); body.Add( ctrl ); }

			// Preset row
			var presetRow = body.AddRow();
			var loadBtn = new Button( "Load Preset", "folder_open" ); loadBtn.Pressed = LoadPreset; presetRow.Add( loadBtn );
			var saveBtn = new Button( "Save Preset", "save" );        saveBtn.Pressed = SavePreset; presetRow.Add( saveBtn );
			body.AddSpacingCell( 12 );

			// Base layer
			body.Add( new Label( "── Base Layer ───────────────────" ) );
			Field( "Material Index",       new IntegerControlWidget( so.GetProperty( nameof( BaseMaterialIndex ) ) ) );
			body.AddSpacingCell( 8 );

			// Shore layer
			body.Add( new Label( "── Shore Layer (Sand) ───────────" ) );
			Field( "Enable",               new BoolControlWidget   ( so.GetProperty( nameof( EnableShore        ) ) ) );
			Field( "Material Index",       new IntegerControlWidget( so.GetProperty( nameof( ShoreMaterialIndex ) ) ) );
			Field( "Water Level (world Z)", new FloatControlWidget ( so.GetProperty( nameof( WaterLevel         ) ) ) );
			Field( "Shore Width",          new FloatControlWidget  ( so.GetProperty( nameof( ShoreWidth         ) ) ) );
			Field( "Shore Blend",          new FloatControlWidget  ( so.GetProperty( nameof( ShoreBlend         ) ) ) );
			Field( "Noise Scale",          new FloatControlWidget  ( so.GetProperty( nameof( ShoreNoiseScale    ) ) ) );
			Field( "Noise Strength",       new FloatControlWidget  ( so.GetProperty( nameof( ShoreNoiseStrength ) ) ) );
			Field( "Max Height (0=off)",   new FloatControlWidget  ( so.GetProperty( nameof( ShoreMaxHeight     ) ) ) );
			body.AddSpacingCell( 8 );

			// Slope layer
			body.Add( new Label( "── Slope Layer (Rock) ───────────" ) );
			Field( "Enable",               new BoolControlWidget   ( so.GetProperty( nameof( EnableSlope        ) ) ) );
			Field( "Material Index",       new IntegerControlWidget( so.GetProperty( nameof( SlopeMaterialIndex ) ) ) );
			Field( "Min Slope",            new FloatControlWidget  ( so.GetProperty( nameof( MinSlope           ) ) ) );
			Field( "Slope Blend Width",    new FloatControlWidget  ( so.GetProperty( nameof( SlopeBlend         ) ) ) );
			Field( "Avoid Shore Distance", new FloatControlWidget  ( so.GetProperty( nameof( AvoidShoreDistance ) ) ) );
			Field( "Avoid Shore Fade",     new FloatControlWidget  ( so.GetProperty( nameof( AvoidShoreFade     ) ) ) );
			Field( "Noise Scale",          new FloatControlWidget  ( so.GetProperty( nameof( SlopeNoiseScale    ) ) ) );
			Field( "Noise Strength",       new FloatControlWidget  ( so.GetProperty( nameof( SlopeNoiseStrength ) ) ) );
			body.AddSpacingCell( 12 );

			var paintBtn = new Button( "Paint Terrain", "brush" ); paintBtn.Pressed = PaintTerrain; body.Add( paintBtn );
			var resetBtn = new Button( "Reset to Base", "clear" );  resetBtn.Pressed = ResetTerrain; body.Add( resetBtn );
		}

		// ─────────────────────────────────── Painting ───────────────────────────

		private void PaintTerrain()
		{
			var scene = SceneEditorSession.Active?.Scene;
			if ( scene == null ) { Log.Warning( "No active editor scene." ); return; }

			var terrain = scene.GetAllComponents<Terrain>().FirstOrDefault();
			if ( terrain == null ) { Log.Warning( "No Terrain component found in scene." ); return; }
			if ( terrain.Storage == null ) { Log.Warning( "Terrain has no Storage assigned." ); return; }

			int   res           = terrain.Storage.Resolution;
			float terrainSize   = terrain.Storage.TerrainSize;
			float terrainHeight = terrain.Storage.TerrainHeight;
			float texelSize     = terrainSize / res;

			for ( int y = 0; y < res; y++ )
			for ( int x = 0; x < res; x++ )
			{
				int   idx       = y * res + x;
				float worldZ    = terrain.Storage.HeightMap[idx] / 65535f * terrainHeight;
				float heightNorm = worldZ / terrainHeight;
				float dist      = Math.Abs( worldZ - WaterLevel );
				float wx        = x * texelSize;
				float wy        = y * texelSize;
				float slope     = ComputeSlope( terrain.Storage.HeightMap, x, y, res, terrainHeight, texelSize );

				// ── Shore fraction ────────────────────────────────────────────────
				float sandFrac = 0f;
				if ( EnableShore && ShoreWidth > 0f )
				{
					// Optional noise displacement on the shore edge
					float noiseDelta = ShoreNoiseStrength > 0f && ShoreNoiseScale > 0f
						? ShoreNoiseStrength * (Noise.Perlin( wx * ShoreNoiseScale, wy * ShoreNoiseScale ) * 2f - 1f)
						: 0f;
					float effectiveWidth = ShoreWidth + noiseDelta;

					// Optional height cap (cliffs going straight into water)
					bool heightOk = ShoreMaxHeight <= 0f || worldZ <= ShoreMaxHeight;

					if ( heightOk && dist <= effectiveWidth )
					{
						float transStart = effectiveWidth * (1f - ShoreBlend);
						if ( ShoreBlend > 0f && dist > transStart )
						{
							float t = (dist - transStart) / (effectiveWidth - transStart);
							t = t * t * (3f - 2f * t); // smoothstep
							sandFrac = 1f - t;
						}
						else
							sandFrac = 1f;
					}
				}

				// ── Slope (rock) fraction ─────────────────────────────────────────
				float rockFrac = 0f;
				if ( EnableSlope )
				{
					// Optional noise on slope value
					float effectiveSlope = slope;
					if ( SlopeNoiseStrength > 0f && SlopeNoiseScale > 0f )
						effectiveSlope += SlopeNoiseStrength * (Noise.Perlin( wx * SlopeNoiseScale + 100f, wy * SlopeNoiseScale + 100f ) * 2f - 1f);

					float slopeMin = MinSlope - SlopeBlend;
					if ( effectiveSlope >= slopeMin )
					{
						if ( SlopeBlend > 0f && effectiveSlope < MinSlope )
						{
							float t = (MinSlope - effectiveSlope) / SlopeBlend;
							t = t * t * (3f - 2f * t);
							rockFrac = 1f - t;
						}
						else
							rockFrac = 1f;

						// Shore avoidance: rock fades out near the shore
						if ( AvoidShoreDistance > 0f )
						{
							float hardEdge = AvoidShoreDistance - AvoidShoreFade;
							if ( dist < hardEdge )
							{
								rockFrac = 0f;
							}
							else if ( AvoidShoreFade > 0f && dist < AvoidShoreDistance )
							{
								float t = (AvoidShoreDistance - dist) / (AvoidShoreDistance - hardEdge);
								t = t * t * (3f - 2f * t);
								rockFrac *= 1f - t;
							}
						}
					}
				}

				// ── Resolve best two materials ────────────────────────────────────
				// CompactTerrainMaterial supports Base + Overlay + BlendFactor.
				// blend=0 → pure base; blend=255 → pure overlay.
				int  baseIdx, overlayIdx;
				byte blend;

				if ( rockFrac > 0.004f && sandFrac > 0.004f )
				{
					// Both rock and sand present → rock over sand
					baseIdx    = ShoreMaterialIndex;
					overlayIdx = SlopeMaterialIndex;
					blend      = (byte)MathX.Clamp( rockFrac * 255f, 0, 255 );
				}
				else if ( rockFrac > 0.004f )
				{
					// Rock over grass
					baseIdx    = BaseMaterialIndex;
					overlayIdx = SlopeMaterialIndex;
					blend      = (byte)MathX.Clamp( rockFrac * 255f, 0, 255 );
				}
				else if ( sandFrac > 0.004f )
				{
					// Sand over grass
					baseIdx    = BaseMaterialIndex;
					overlayIdx = ShoreMaterialIndex;
					blend      = (byte)MathX.Clamp( sandFrac * 255f, 0, 255 );
				}
				else
				{
					// Pure base
					baseIdx    = BaseMaterialIndex;
					overlayIdx = BaseMaterialIndex;
					blend      = 0;
				}

				terrain.Storage.ControlMap[idx] = new CompactTerrainMaterial(
					(byte)MathX.Clamp( baseIdx,    0, 31 ),
					(byte)MathX.Clamp( overlayIdx, 0, 31 ),
					blend
				).Packed;
			}

			terrain.SyncGPUTexture();
			Log.Info( $"Terrain Painter: paint complete ({res}×{res} texels)." );
		}

		private void ResetTerrain()
		{
			var scene = SceneEditorSession.Active?.Scene;
			if ( scene == null ) return;
			var terrain = scene.GetAllComponents<Terrain>().FirstOrDefault();
			if ( terrain?.Storage == null ) return;
			var zero = new CompactTerrainMaterial( (byte)BaseMaterialIndex, (byte)BaseMaterialIndex, 0 ).Packed;
			for ( int i = 0; i < terrain.Storage.ControlMap.Length; i++ )
				terrain.Storage.ControlMap[i] = zero;
			terrain.SyncGPUTexture();
			Log.Info( "Terrain Painter: reset to base material." );
		}

		// ─────────────────────────────────── Presets ────────────────────────────

		private void LoadPreset()
		{
			FileSystem.Data.CreateDirectory( PresetsFolder );
			var files = FileSystem.Data.FindFile( PresetsFolder, "*.json" ).ToArray();
			if ( files.Length == 0 ) { Log.Warning( "No terrain presets found." ); return; }

			var menu = new Menu( this );
			foreach ( var file in files )
			{
				var name = Path.GetFileNameWithoutExtension( file );
				var path = $"{PresetsFolder}/{file}";
				menu.AddOption( name, "folder_open", () =>
				{
					var data = FileSystem.Data.ReadJson<TerrainLayerPresetData>( path );
					if ( data != null ) { ApplyPreset( data ); Log.Info( $"Loaded terrain preset: {name}" ); }
					else Log.Warning( $"Could not read preset: {path}" );
				} );
			}
			menu.OpenAtCursor();
		}

		private void SavePreset()
		{
			var popup = new PopupWidget( this );
			popup.Layout = Layout.Column();
			popup.Layout.Margin = 12;
			popup.Layout.Spacing = 8;
			popup.Layout.Add( new Label( "Preset name:" ) );
			var input = new LineEdit( popup ) { PlaceholderText = "my_terrain" };
			popup.Layout.Add( input );
			var confirmBtn = new Button( "Save", "save" );
			confirmBtn.Pressed = () =>
			{
				var name = input.Text.Trim();
				if ( string.IsNullOrEmpty( name ) ) return;
				FileSystem.Data.CreateDirectory( PresetsFolder );
				FileSystem.Data.WriteJson( $"{PresetsFolder}/{name}.json", BuildPresetData() );
				Log.Info( $"Saved terrain preset: {name}" );
				popup.Visible = false;
			};
			popup.Layout.Add( confirmBtn );
			popup.OpenAtCursor();
		}

		private TerrainLayerPresetData BuildPresetData() => new TerrainLayerPresetData
		{
			BaseMaterialIndex  = BaseMaterialIndex,
			EnableShore        = EnableShore,
			ShoreMaterialIndex = ShoreMaterialIndex,
			WaterLevel         = WaterLevel,
			ShoreWidth         = ShoreWidth,
			ShoreBlend         = ShoreBlend,
			ShoreNoiseScale    = ShoreNoiseScale,
			ShoreNoiseStrength = ShoreNoiseStrength,
			ShoreMaxHeight     = ShoreMaxHeight,
			EnableSlope        = EnableSlope,
			SlopeMaterialIndex = SlopeMaterialIndex,
			MinSlope           = MinSlope,
			SlopeBlend         = SlopeBlend,
			AvoidShoreDistance = AvoidShoreDistance,
			AvoidShoreFade     = AvoidShoreFade,
			SlopeNoiseScale    = SlopeNoiseScale,
			SlopeNoiseStrength = SlopeNoiseStrength,
		};

		private void ApplyPreset( TerrainLayerPresetData d )
		{
			BaseMaterialIndex  = d.BaseMaterialIndex;
			EnableShore        = d.EnableShore;
			ShoreMaterialIndex = d.ShoreMaterialIndex;
			WaterLevel         = d.WaterLevel;
			ShoreWidth         = d.ShoreWidth;
			ShoreBlend         = d.ShoreBlend;
			ShoreNoiseScale    = d.ShoreNoiseScale;
			ShoreNoiseStrength = d.ShoreNoiseStrength;
			ShoreMaxHeight     = d.ShoreMaxHeight;
			EnableSlope        = d.EnableSlope;
			SlopeMaterialIndex = d.SlopeMaterialIndex;
			MinSlope           = d.MinSlope;
			SlopeBlend         = d.SlopeBlend;
			AvoidShoreDistance = d.AvoidShoreDistance;
			AvoidShoreFade     = d.AvoidShoreFade;
			SlopeNoiseScale    = d.SlopeNoiseScale;
			SlopeNoiseStrength = d.SlopeNoiseStrength;
		}

		// ─────────────────────────────────── Helpers ─────────────────────────────

		/// <summary>Slope 0 (flat) → 1 (vertical) from HeightMap partial derivatives.</summary>
		private static float ComputeSlope( ushort[] hm, int x, int y, int res, float terrainHeight, float texelSize )
		{
			float h  = hm[y * res + x]                                              / 65535f * terrainHeight;
			float hR = hm[y * res + (int)MathX.Clamp( x + 1, 0, res - 1 )]         / 65535f * terrainHeight;
			float hU = hm[(int)MathX.Clamp( y + 1, 0, res - 1 ) * res + x]         / 65535f * terrainHeight;
			var   n  = new Vector3( -(hR - h) / texelSize, -(hU - h) / texelSize, 1f ).Normal;
			return 1f - MathX.Clamp( n.z, 0f, 1f );
		}
	}
}
