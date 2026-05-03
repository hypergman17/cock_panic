using Sandbox;
using System.Collections.Generic;
using System.Diagnostics;
using Editor;
using Sandbox.Utility;
using System.Linq;
using FileSystem = Sandbox.FileSystem;

namespace Doubled.ForestGenerator.Editor
{
	[Dock( "Editor", "Forest Generator", "park" )]
	public class ForestGenerator : Widget
	{
		[Property] public BBox AreaBounds { get; set; } = new BBox( new Vector3( -2000, -2000, 0 ), new Vector3( 2000, 2000, 1000 ) );
		[Property] public List<ForestPrefabConfig> Prefabs { get; set; } = new();

		[Property, Range( 10, 500 )] public float Spacing { get; set; } = 100f;
		[Property, Range( 0.001f, 0.1f )] public float NoiseScale { get; set; } = 0.01f;
		[Property, Range( 0f, 1f )] public float NoiseThreshold { get; set; } = 0.4f;
		[Property] public float Seed { get; set; } = 0f;
		[Property, Range( 0.1f, 5f )] public float MinScale { get; set; } = 0.8f;
		[Property, Range( 0.1f, 5f )] public float MaxScale { get; set; } = 1.2f;
		[Property] public bool AlignToNormal { get; set; } = false;

		private const string PresetsDir = "forest_generator/presets";

		public ForestGenerator( Widget parent ) : base( parent, false )
		{
			Layout = Layout.Column();
			Layout.Margin = 10;
			Layout.Spacing = 5;

			var scroll = new ScrollArea( this );
			scroll.Canvas = new Widget( scroll );
			scroll.Canvas.Layout = Layout.Column();
			scroll.Canvas.Layout.Margin = 16;
			scroll.Canvas.Layout.Spacing = 8;
			Layout.Add( scroll );

			var body = scroll.Canvas.Layout;
			var so = this.GetSerialized();

			// ── Preset buttons ──────────────────────────────────────────
			var presetRow = body.AddRow();
			var loadPresetBtn = new Button( "Load Preset", "folder_open" );
			loadPresetBtn.Pressed = LoadPreset;
			presetRow.Add( loadPresetBtn );

			var savePresetBtn = new Button( "Save Preset", "save" );
			savePresetBtn.Pressed = SavePreset;
			presetRow.Add( savePresetBtn );

			body.AddSpacingCell( 12 );

			// ── Settings ─────────────────────────────────────────────────
			body.Add( new Label( "Area Bounds" ) );
			body.Add( new BBoxControlWidget( so.GetProperty( nameof( AreaBounds ) ) ) );

			body.Add( new Label( "Prefabs & Weights" ) );
			body.Add( new ListControlWidget( so.GetProperty( nameof( Prefabs ) ) ) );

			body.AddSpacingCell( 8 );

			body.Add( new Label( "Spacing" ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( Spacing ) ) ) );

			body.Add( new Label( "Noise Scale" ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( NoiseScale ) ) ) );

			body.Add( new Label( "Noise Threshold" ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( NoiseThreshold ) ) ) );

			body.Add( new Label( "Seed" ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( Seed ) ) ) );

			body.AddSpacingCell( 8 );

			body.Add( new Label( "Min/Max Scale" ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( MinScale ) ) ) );
			body.Add( new FloatControlWidget( so.GetProperty( nameof( MaxScale ) ) ) );

			body.Add( new Label( "Align To Normal" ) );
			body.Add( new BoolControlWidget( so.GetProperty( nameof( AlignToNormal ) ) ) );

			body.AddSpacingCell( 16 );

			// ── Actions ──────────────────────────────────────────────────
			var generateBtn = new Button( "Generate Forest", "park" );
			generateBtn.Pressed = GenerateForest;
			body.Add( generateBtn );

			var clearBtn = new Button( "Clear Forest", "delete" );
			clearBtn.Pressed = ClearForest;
			body.Add( clearBtn );
		}

		private static string PresetsFolder => "forest_generator";

		private void LoadPreset()
		{
			// List existing presets
			FileSystem.Data.CreateDirectory( PresetsFolder );
			var files = FileSystem.Data.FindFile( PresetsFolder, "*.json" ).ToArray();

			if ( files.Length == 0 )
			{
				Log.Warning( "No saved presets found in FileSystem.Data/forest_generator/" );
				return;
			}

			// Show a popup menu with available presets
			var menu = new Menu( this );
			foreach ( var file in files )
			{
				var name = System.IO.Path.GetFileNameWithoutExtension( file );
				var path = $"{PresetsFolder}/{file}";
				menu.AddOption( name, "folder_open", () =>
				{
					var data = FileSystem.Data.ReadJson<ForestPresetData>( path );
					if ( data != null ) { ApplyPreset( data ); Log.Info( $"Loaded preset: {name}" ); }
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

			var input = new LineEdit( popup );
			input.PlaceholderText = "my_preset";
			popup.Layout.Add( input );

			var confirmBtn = new Button( "Save", "save" );
			confirmBtn.Pressed = () =>
			{
				var name = input.Text.Trim();
				if ( string.IsNullOrEmpty( name ) ) return;

				FileSystem.Data.CreateDirectory( PresetsFolder );
				var path = $"{PresetsFolder}/{name}.json";
				FileSystem.Data.WriteJson( path, BuildPresetData() );
				Log.Info( $"Saved preset: {path}" );
				popup.Visible = false;
			};
			popup.Layout.Add( confirmBtn );
			popup.OpenAtCursor();
		}

		private ForestPresetData BuildPresetData()
		{
			var data = new ForestPresetData
			{
				AreaBounds     = AreaBounds,
				Spacing        = Spacing,
				NoiseScale     = NoiseScale,
				NoiseThreshold = NoiseThreshold,
				Seed           = Seed,
				MinScale       = MinScale,
				MaxScale       = MaxScale,
				AlignToNormal  = AlignToNormal,
			};

			// Store prefab resource paths
			foreach ( var cfg in Prefabs )
			{
				data.PrefabPaths.Add( cfg.Prefab?.ResourcePath ?? "" );
				data.PrefabWeights.Add( cfg.Weight );
			}

			return data;
		}

		private void ApplyPreset( ForestPresetData data )
		{
			AreaBounds     = data.AreaBounds;
			Spacing        = data.Spacing;
			NoiseScale     = data.NoiseScale;
			NoiseThreshold = data.NoiseThreshold;
			Seed           = data.Seed;
			MinScale       = data.MinScale;
			MaxScale       = data.MaxScale;
			AlignToNormal  = data.AlignToNormal;

			// Restore prefabs from resource paths
			Prefabs = new List<ForestPrefabConfig>();
			for ( int i = 0; i < data.PrefabPaths.Count; i++ )
			{
				var path   = data.PrefabPaths[i];
				var weight = i < data.PrefabWeights.Count ? data.PrefabWeights[i] : 1f;
				var prefab = string.IsNullOrEmpty( path ) ? null : ResourceLibrary.Get<PrefabFile>( path );
				Prefabs.Add( new ForestPrefabConfig { Prefab = prefab, Weight = weight } );
			}
		}

		private void GenerateForest()
		{
			if ( Prefabs == null || Prefabs.Count == 0 ) return;

			var scene = SceneEditorSession.Active.Scene;
			if ( scene == null ) return;

			ClearForest();

			var container = scene.CreateObject();
			container.Name = "Generated Forest";

			float totalWeight = Prefabs.Sum( x => x.Weight );
			if ( totalWeight <= 0 ) totalWeight = 1;

			int spawnCount = 0;

			using ( scene.Push() )
			{
				for ( float x = AreaBounds.Mins.x; x <= AreaBounds.Maxs.x; x += Spacing )
				{
					for ( float y = AreaBounds.Mins.y; y <= AreaBounds.Maxs.y; y += Spacing )
					{
						float noiseVal = Noise.Perlin( (x + Seed) * NoiseScale, (y + Seed) * NoiseScale );
						if ( noiseVal < NoiseThreshold ) continue;

						var rayStart = new Vector3( x + Game.Random.Float( -Spacing, Spacing ) * 0.4f, y + Game.Random.Float( -Spacing, Spacing ) * 0.4f, AreaBounds.Maxs.z );
						var rayEnd   = new Vector3( rayStart.x, rayStart.y, AreaBounds.Mins.z );
						var trace    = scene.Trace.Ray( rayStart, rayEnd ).HitTriggers().Run();

						if ( trace.Hit )
						{
							if ( !trace.Tags.Contains( "forest" ) ) continue;
							var config = GetRandomPrefab( totalWeight );
							if ( config.Prefab == null ) continue;

							var rotation = Rotation.FromYaw( Game.Random.Float( 0, 360 ) );
							if ( AlignToNormal ) rotation = Rotation.LookAt( rotation.Forward, trace.Normal );

							var go = SceneUtility.GetPrefabScene( config.Prefab ).Clone( new CloneConfig
							{
								Parent = container,
								StartEnabled = true
							} );

							go.WorldPosition = trace.HitPosition;
							go.WorldRotation  = rotation;
							go.WorldScale     = Game.Random.Float( MinScale, MaxScale );
							spawnCount++;
						}
					}
				}
			}

			Log.Info( $"Generated {spawnCount} trees." );
		}

		private ForestPrefabConfig GetRandomPrefab( float totalWeight )
		{
			float roll = Game.Random.Float( 0, totalWeight );
			float cumulative = 0;
			foreach ( var cfg in Prefabs )
			{
				cumulative += cfg.Weight;
				if ( roll <= cumulative ) return cfg;
			}
			return Prefabs[0];
		}

		private void ClearForest()
		{
			var scene = SceneEditorSession.Active.Scene;
			var existing = scene?.GetAllObjects( false ).FirstOrDefault( x => x.Name == "Generated Forest" );
			existing?.Destroy();
		}
	}
}
